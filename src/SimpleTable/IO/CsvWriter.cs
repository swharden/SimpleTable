using System.Text;

namespace SimpleTable.IO;

public static class CsvWriter
{
    /// <summary>
    /// Returns the table as an Excel-compatible CSV string.
    /// <code>
    /// Date,Price,Volume
    /// 3/14/2026,$172.00,"817,508,034"
    /// 3/15/2026,$176.00,"558,603,030"
    /// 3/16/2026,$120.00,"559,325,909"
    /// </code>
    /// </summary>
    public static string ToCsvString(this StringTable table)
    {
        StringBuilder sb = new();

        // Prefix with a UTF-8 BOM so Excel on Windows will usually detect UTF-8 encoding.
        sb.Append('\uFEFF');

        for (int c = 0; c < table.ColumnCount; c++)
        {
            if (c > 0)
                sb.Append(',');
            sb.Append(EscapeCsvField(table.ColumnNames[c]));
        }
        sb.Append("\r\n");

        foreach (TableRow row in table.Rows)
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
}
