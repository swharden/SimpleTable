using System.Text;

namespace SimpleTable;

// TODO: modify void methods to return 'this' to support fluent API

/// <summary>
/// A simple table that stores all cell values as strings.
/// </summary>
public sealed class StringTable
{
    private List<List<string?>> ValuesByRow { get; } = []; // TODO: make list of lists
    private List<string> ColumnNamesList { get; }
    private Dictionary<string, int> ColumnIndexesByName { get; }
    public int ColumnCount => ColumnNamesList.Count;
    public int RowCount => ValuesByRow.Count;
    public IReadOnlyList<string> ColumnNames => ColumnNamesList;
    public TableMetadata Metadata { get; set; } = new();
    public TableDimensions Dimensions => new(RowCount, ColumnCount);

    public int LastRowIndex => RowCount - 1;
    public int LastColumnIndex => ColumnCount - 1;

    public StringTable(int rowCount, int columnCount)
    {
        ColumnNamesList = [];
        ColumnIndexesByName = new Dictionary<string, int>(ColumnNamesList.Count, StringComparer.Ordinal);

        while (ColumnCount < columnCount)
            AddColumn();

        while (RowCount < rowCount)
            AddRow();
    }

    public StringTable(params IEnumerable<string> columnNames)
    {
        ColumnNamesList = columnNames.ToList();
        ColumnIndexesByName = new Dictionary<string, int>(ColumnNamesList.Count, StringComparer.Ordinal);

        for (int i = 0; i < ColumnNamesList.Count; i++)
        {
            if (!ColumnIndexesByName.TryAdd(ColumnNamesList[i], i))
                throw new ArgumentException($"Duplicate column name '{ColumnNamesList[i]}'.", nameof(columnNames));
        }
    }

    public string? this[int row, int col]
    {
        get => ValuesByRow[row][col];
        set => ValuesByRow[row][col] = value;
    }

    public string? this[int row, string columnName]
    {
        get => this[row, IndexOf(columnName)];
        set => this[row, IndexOf(columnName)] = value;
    }

    public string? GetValue(int row, int column) => this[row, column];
    public string? GetValue(int row, string columnName) => this[row, columnName];
    public List<string?> GetValues(bool byRow = true) => byRow
            ? Rows.SelectMany(r => r.Values ?? Enumerable.Empty<string?>()).ToList()
            : Columns.SelectMany(r => r.Values ?? Enumerable.Empty<string?>()).ToList();

    public void SetValue(TableRow row, TableColumn column, string? value) => this[row.RowIndex, column.ColumnIndex] = value;
    public void SetValue(int row, int column, string? value) => this[row, column] = value;
    public void SetValue(int row, string columnName, string? value) => this[row, columnName] = value;

    public List<TableRow> Rows => Enumerable.Range(0, RowCount).Select(GetRow).ToList();
    public List<TableColumn> Columns => Enumerable.Range(0, ColumnCount).Select(GetColumn).ToList();

    public TableRow GetRow(int rowIndex)
    {
        return new TableRow()
        {
            RowIndex = rowIndex,
            Values = GetRowValues(rowIndex),
        };
    }

    public TableColumn GetColumn(int columnIndex)
    {
        return new TableColumn()
        {
            ColumnIndex = columnIndex,
            ColumnName = ColumnNamesList[columnIndex],
            Values = GetColumnValues(columnIndex),
        };
    }

    public override string ToString()
    {
        return $"{Metadata.Title} {nameof(StringTable)} with {RowCount} rows and {ColumnCount} columns";
    }

    /// <summary>Returns the zero-based index of a named column.</summary>
    /// <exception cref="KeyNotFoundException">Unknown column name.</exception>
    public int IndexOf(string columnName)
    {
        if (!ColumnIndexesByName.TryGetValue(columnName, out int idx))
            throw new KeyNotFoundException($"No column named '{columnName}'.");
        return idx;
    }

    /// <summary>Appends a row. Must supply exactly <see cref="ColumnCount"/> values.</summary>
    public void AppendRow(params string?[] values)
    {
        if (values.Length != ColumnNamesList.Count)
            throw new ArgumentException(
                $"Expected {ColumnNamesList.Count} values, got {values.Length}.", nameof(values));

        ValuesByRow.Add(values.ToList());
    }

    public void ClearRows()
    {
        ValuesByRow.Clear();
    }

    public void Clear()
    {
        ValuesByRow.Clear();
        ColumnNamesList.Clear();
        ColumnIndexesByName.Clear();
    }

    /// <summary>
    /// Returns the table as an aligned plain-text string with box-drawing borders.
    /// <code>
    /// +---------+-----+--------+
    /// | Name    | Age | City   |
    /// +---------+-----+--------+
    /// | Alice   | 30  | Boston |
    /// +---------+-----+--------+
    /// </code>
    /// </summary>
    public string ToDisplayString()
    {
        if (ColumnCount == 0)
            return string.Empty;

        int[] widths = MeasureColumnWidths();
        string border = BuildBorder(widths, '+', '-');
        string headerSep = border;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(border);
        sb.AppendLine(BuildColumnNameRow(ColumnNamesList, widths));
        sb.AppendLine(headerSep);
        foreach (var row in ValuesByRow)
            sb.AppendLine(BuildCellsRow(row, widths));
        sb.AppendLine(border);
        return sb.ToString();
    }

    private static string EscapeCsvField(string? value)
    {
        if (value is null)
            return string.Empty;

        // Escape double quotes by doubling them
        string escaped = value.Replace("\"", "\"\"");

        // If the field contains a comma, quote, CR or LF then wrap in double quotes
        if (escaped.IndexOfAny([',', '"', '\r', '\n']) >= 0)
            return '"' + escaped + '"';

        return escaped;
    }

    /// <summary>
    /// Returns the table as a Markdown-formatted string.
    /// <code>
    /// | Name  | Age | City   |
    /// |-------|-----|--------|
    /// | Alice | 30  | Boston |
    /// </code>
    /// </summary>
    public string ToMarkdownString()
    {
        if (ColumnCount == 0)
            return string.Empty;

        int[] widths = MeasureColumnWidths();
        string headerSep = BuildBorder(widths, '|', '-');

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(BuildColumnNameRow(ColumnNamesList, widths));
        sb.AppendLine(headerSep);
        foreach (var row in ValuesByRow)
            sb.AppendLine(BuildCellsRow(row, widths));
        return sb.ToString();
    }

    public string ToCsvString()
    {
        var sb = new StringBuilder();

        // Prefix with a UTF-8 BOM so Excel on Windows will usually detect UTF-8 encoding.
        sb.Append('\uFEFF');

        for (int c = 0; c < ColumnCount; c++)
        {
            if (c > 0)
                sb.Append(',');
            sb.Append(EscapeCsvField(ColumnNamesList[c]));
        }
        sb.Append("\r\n");

        foreach (TableRow row in Rows)
        {
            for (int c = 0; c < row.Values.Count; c++)
            {
                if (c > 0)
                    sb.Append(',');
                sb.Append(EscapeCsvField(row.Values[c]));
            }
            sb.Append("\r\n");
        }

        return sb.ToString();
    }

    private int[] MeasureColumnWidths()
    {
        int[] widths = new int[ColumnCount];
        for (int c = 0; c < ColumnCount; c++)
            widths[c] = ColumnNamesList[c].Length;
        foreach (var row in ValuesByRow)
            for (int c = 0; c < ColumnCount; c++)
                widths[c] = Math.Max(widths[c], row[c]?.Length ?? NullDisplayString.Length);
        return widths;
    }

    private static string BuildBorder(int[] widths, char corner, char fill)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(corner);
        foreach (int w in widths)
            sb.Append(fill, w + 2).Append(corner);
        return sb.ToString();
    }

    /// <summary>
    /// Used for null values when displaying tables as text
    /// </summary>
    public static string NullDisplayString { get; set; } = "--";

    private static string BuildCellsRow(List<string?> cells, int[] widths)
    {
        StringBuilder sb = new("|");

        for (int c = 0; c < cells.Count; c++)
        {
            sb.Append(' ')
                .Append((cells[c] ?? NullDisplayString)
                .PadRight(widths[c]))
                .Append(" |");
        }

        return sb.ToString();
    }

    private static string BuildColumnNameRow(List<string> columnNames, int[] widths)
    {
        StringBuilder sb = new("|");

        for (int c = 0; c < columnNames.Count; c++)
        {
            sb.Append(' ')
                .Append((columnNames[c] ?? string.Empty)
                .PadRight(widths[c]))
                .Append(" |");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Return an Excel style column name (A, B, C, ..., AA, AB, AC, ...)
    /// </summary>
    public static string DefaultColumnName(int columnIndex)
    {
        string columnName = "";

        while (columnIndex > 0)
        {
            columnIndex--;
            columnName = (char)('A' + (columnIndex % 26)) + columnName;
            columnIndex /= 26;
        }

        return columnName;
    }

    public void AddColumn(string? columnName = null)
    {
        columnName ??= DefaultColumnName(ColumnCount + 1);

        int newIndex = ColumnCount;
        ColumnNamesList.Add(columnName);
        ColumnIndexesByName[columnName] = newIndex;

        for (int i = 0; i < RowCount; i++)
        {
            ValuesByRow[i].Add(null);
        }
    }

    public void AddColumn(IList<string?> values, string? columnName = null)
    {
        AddColumn(columnName);

        while (RowCount < values.Count)
            AddRow();

        for (int i = 0; i < values.Count; i++)
        {
            this[i, LastColumnIndex] = values[i];
        }
    }

    public void AddColumn(string columnName, IList<string?> values)
    {
        int newIndex = ColumnCount;
        ColumnNamesList.Add(columnName);
        ColumnIndexesByName[columnName] = newIndex;

        // pad existing rows with an empty cell
        for (int i = 0; i < RowCount; i++)
        {
            ValuesByRow[i].Add(string.Empty);
        }

        // add more rows if the data is bigger than the existing table
        while (RowCount < values.Count)
        {
            AddRow();
        }

        // populate cells with values passed in
        int lastColumnIndex = ColumnCount - 1;
        for (int i = 0; i < values.Count; i++)
        {
            ValuesByRow[i][lastColumnIndex] = values[i];
        }
    }

    public void AddColumn(TableColumn column)
    {
        AddColumn(column.ColumnName, column.Values);
    }

    public void AddColumns(IEnumerable<string> columnNames)
    {
        foreach (string columnName in columnNames)
            AddColumn(columnName);
    }

    public void AddColumns(IEnumerable<TableColumn> columns)
    {
        foreach (TableColumn column in columns)
            AddColumn(column.ColumnName, column.Values);
    }

    public void AddColumns(StringTable table)
    {
        AddColumns(table.Columns);
    }

    public void AddRow()
    {
        var emptyRow = Enumerable.Repeat<string?>(null, ColumnCount).ToList();
        ValuesByRow.Add(emptyRow);
    }

    public void AddRow(IList<string?> values)
    {
        while (ColumnCount < values.Count)
            AddColumn();

        AddRow();

        int lastRowIndex = RowCount - 1;
        for (int i = 0; i < values.Count; i++)
        {
            this[lastRowIndex, i] = values[i];
        }
    }

    public void AddRow(TableRow column)
    {
        AddRow(column.Values);
    }

    public void AddRows(IEnumerable<TableRow> rows)
    {
        foreach (TableRow row in rows)
            AddRow(row.Values);
    }

    public void AddRows(StringTable table)
    {
        AddRows(table.Rows);
    }

    public List<string?> GetColumnValues(string columnName)
    {
        int columnIndex = ColumnIndexesByName[columnName];
        return GetColumnValues(columnIndex);
    }

    public List<string?> GetColumnValues(int columnIndex)
    {
        return Enumerable.Range(0, RowCount).Select(x => this[x, columnIndex]).ToList();
    }

    public List<string?> GetRowValues(int rowIndex)
    {
        return Enumerable.Range(0, ColumnCount).Select(x => this[rowIndex, x]).ToList();
    }

    public void SetColumnName(int index, string name)
    {
        string oldName = ColumnNamesList[index];
        ColumnNamesList[index] = name;
        ColumnIndexesByName.Remove(oldName);
        ColumnIndexesByName[name] = index;
    }

    public void SetColumnNames(IList<string?> names)
    {
        while (ColumnCount < names.Count)
            AddColumn();

        for (int i = 0; i < names.Count; i++)
            SetColumnName(i, names[i] ?? DefaultColumnName(i));
    }

    public void LaunchInDefaultBrowser(string? saveAs = null)
    {
        saveAs ??= $"{DateTime.UtcNow.Ticks}.html";
        saveAs = Path.GetFullPath(saveAs);

        string html = Exporters.HtmlExporter.GetStyledHtml(this);
        string html2 = Exporters.Html.WrapInHtml(html);
        File.WriteAllText(saveAs, html2);
        Launch.DefaultBrowser(saveAs);
    }

    public void Rotate90()
    {
        // NOTE: since we don't have row names, column names become lost
        //List<string> columnNames = ColumnNames.ToList();

        List<string?> values = GetValues();
        int initialRowCount = RowCount;
        int initialColumnCount = ColumnCount;
        int newRowCount = ColumnCount;
        int newColumnCount = RowCount;

        Clear();

        while (ColumnCount < newColumnCount)
            AddColumn();

        while (RowCount < newRowCount)
            AddRow();

        for (int r = 0; r < newRowCount; r++)
            for (int c = 0; c < newColumnCount; c++)
                this[r, c] = values[(initialRowCount - 1 - c) * initialColumnCount + r];
    }

    public void DeleteRow(TableRow row) => DeleteRow(row.RowIndex);

    public void DeleteRow(int rowIndex)
    {
        ValuesByRow.RemoveAt(rowIndex);
    }

    public void DeleteColumn(string columnName) => DeleteColumn(ColumnIndexesByName[columnName]);

    public void DeleteColumn(TableColumn column) => DeleteColumn(column.ColumnIndex);

    public void DeleteColumn(int columnIndex)
    {
        string columnName = ColumnNamesList[columnIndex];
        ColumnNamesList.RemoveAt(columnIndex);
        ColumnIndexesByName.Remove(columnName);

        for (int i = columnIndex; i < ColumnNamesList.Count; i++)
            ColumnIndexesByName[ColumnNamesList[i]] = i;

        foreach (var row in ValuesByRow)
            row.RemoveAt(columnIndex);
    }

    public void SetColumnNamesFromFirstRow()
    {
        SetColumnNames(GetRow(0).Values);
        DeleteRow(0);
    }
}