using System.Reflection.Emit;
using System.Text;

namespace SimpleTable;

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

    public void SetValue(int row, int column, string value) => this[row, column] = value;
    public void SetValue(int row, string columnName, string value) => this[row, columnName] = value;

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

    /// <summary>Removes all rows.</summary>
    public void Clear() => ValuesByRow.Clear();

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

        ColumnNamesList.Add(columnName);

        for (int i = 0; i < RowCount; i++)
        {
            ValuesByRow[i].Add(string.Empty);
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

    public void AddColumn(string columnName, IList<string> values)
    {
        ColumnNamesList.Add(columnName);

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

    public string ToCsvString()
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Join(", ", ColumnNames));

        foreach (var row in ValuesByRow)
            sb.AppendLine(string.Join(", ", row));

        return sb.ToString();
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

    public void SetColumnNames(string[] names)
    {
        while (ColumnCount < names.Length)
            AddColumn();

        for (int i = 0; i < names.Length; i++)
            SetColumnName(i, names[i]);
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
}