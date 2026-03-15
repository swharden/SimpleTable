using System.Text;

namespace SimpleTable;

/// <summary>
/// A simple table that stores all cell values as strings.
/// </summary>
public sealed class StringTable
{
    private readonly List<string?[]> _rows = [];
    private readonly string[] _columnNames;
    private readonly Dictionary<string, int> _columnIndex;

    public StringTable(params string[] columnNames)
    {
        _columnNames = (string[])columnNames.Clone();
        _columnIndex = new Dictionary<string, int>(columnNames.Length, StringComparer.Ordinal);

        for (int i = 0; i < columnNames.Length; i++)
        {
            if (!_columnIndex.TryAdd(columnNames[i], i))
                throw new ArgumentException($"Duplicate column name '{columnNames[i]}'.", nameof(columnNames));
        }
    }

    public int ColumnCount => _columnNames.Length;
    public int RowCount => _rows.Count;
    public IReadOnlyList<string> ColumnNames => _columnNames;

    public string? this[int row, int col]
    {
        get => _rows[row][col];
        set => _rows[row][col] = value;
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
        if (!_columnIndex.TryGetValue(columnName, out int idx))
            throw new KeyNotFoundException($"No column named '{columnName}'.");
        return idx;
    }

    /// <summary>Appends a row. Must supply exactly <see cref="ColumnCount"/> values.</summary>
    public void AppendRow(params string?[] values)
    {
        if (values.Length != _columnNames.Length)
            throw new ArgumentException(
                $"Expected {_columnNames.Length} values, got {values.Length}.", nameof(values));

        _rows.Add((string?[])values.Clone());
    }

    /// <summary>Removes all rows.</summary>
    public void Clear() => _rows.Clear();

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
        sb.AppendLine(BuildRow(_columnNames, widths));
        sb.AppendLine(headerSep);
        foreach (string?[] row in _rows)
            sb.AppendLine(BuildRow(row, widths));
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
        sb.AppendLine(BuildRow(_columnNames, widths));
        sb.AppendLine(headerSep);
        foreach (string?[] row in _rows)
            sb.AppendLine(BuildRow(row, widths));
        return sb.ToString();
    }

    private int[] MeasureColumnWidths()
    {
        int[] widths = new int[ColumnCount];
        for (int c = 0; c < ColumnCount; c++)
            widths[c] = _columnNames[c].Length;
        foreach (string?[] row in _rows)
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

    private static string BuildRow(string?[] cells, int[] widths)
    {
        var sb = new System.Text.StringBuilder("|");
        for (int c = 0; c < cells.Length; c++)
            sb.Append(' ').Append((cells[c] ?? "").PadRight(widths[c])).Append(" |");
        return sb.ToString();
    }
}