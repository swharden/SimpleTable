using System.Text;

namespace SimpleTable.IO;

public static class TextWriter
{
    public static void WriteLineDisplay(this StringTable table)
    {
        Console.WriteLine(table.ToDisplayString());
    }

    public static void WriteLineMarkdown(this StringTable table)
    {
        Console.WriteLine(table.ToMarkdownString());
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
    public static string ToDisplayString(this StringTable table)
    {
        if (table.ColumnCount == 0)
            return string.Empty;

        int[] widths = MeasureColumnWidths(table);
        string border = BuildBorder(widths, '+', '-');
        string headerSep = border;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(border);
        sb.AppendLine(BuildColumnNameRow(table.ColumnNames, widths));
        sb.AppendLine(headerSep);
        foreach (var row in table.Rows)
            sb.AppendLine(BuildCellsRow(table, row, widths));
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
    public static string ToMarkdownString(this StringTable table)
    {
        if (table.ColumnCount == 0)
            return string.Empty;

        int[] widths = MeasureColumnWidths(table);
        string headerSep = BuildBorder(widths, '|', '-');

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(BuildColumnNameRow(table.ColumnNames, widths));
        sb.AppendLine(headerSep);
        foreach (var row in table.Rows)
            sb.AppendLine(BuildCellsRow(table, row, widths));
        return sb.ToString();
    }

    private static string BuildColumnNameRow(IReadOnlyList<string> columnNames, int[] widths)
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

    private static int[] MeasureColumnWidths(StringTable table)
    {
        int[] widths = new int[table.ColumnCount];
        for (int c = 0; c < table.ColumnCount; c++)
            widths[c] = table.ColumnNames[c].Length;
        foreach (var row in table.Rows)
            for (int c = 0; c < row.Values.Count; c++)
                widths[c] = Math.Max(widths[c], row.Values[c]?.Length ?? table.NullDisplayString.Length);
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

    private static string BuildCellsRow(StringTable table, TableRow row, int[] widths)
    {
        StringBuilder sb = new("|");

        for (int c = 0; c < row.Values.Count; c++)
        {
            sb.Append(' ')
                .Append((row.Values[c] ?? table.NullDisplayString)
                .PadRight(widths[c]))
                .Append(" |");
        }

        return sb.ToString();
    }
}
