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
    public IReadOnlyList<string> ColumnNames => ColumnNames;


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
                widths[c] = Math.Max(widths[c], row[c]?.Length ?? 0);
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

    private static string BuildCellsRow(List<string?> cells, int[] widths)
    {
        StringBuilder sb = new("|");

        for (int c = 0; c < cells.Count; c++)
        {
            sb.Append(' ')
                .Append((cells[c] ?? string.Empty)
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

    public void AddColumn(string columnName)
    {
        int originalColumnCount = ColumnCount;

        ColumnNamesList.Add(columnName);
        for (int i = 0; i < RowCount; i++)
        {

        }
    }
}