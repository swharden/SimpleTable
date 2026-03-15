using System.Text;

namespace SimpleTable.Exporters;

public static class HtmlExporter
{
    public static string GetUnstyledHtml(StringTable table)
    {
        StringBuilder sb = new();
        sb.AppendLine("<table>");

        // header
        sb.AppendLine("<thead>");
        sb.AppendLine("<tr>");
        for (int j = 0; j < table.ColumnCount; j++)
        {
            sb.AppendLine($"<th>{table.ColumnNames[j]}</th>");
        }
        sb.AppendLine("</tr>");
        sb.AppendLine("</thead>");

        // rows
        sb.AppendLine("<tbody>");
        for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
        {
            sb.AppendLine("<tr>");
            for (int columnIndex = 0; columnIndex < table.ColumnCount; columnIndex++)
            {
                sb.AppendLine($"<td>{table[rowIndex, columnIndex]}</td>");
            }
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody>");

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    public static string GetStyledHtml(StringTable table)
    {
        StringBuilder sb = new();
        sb.AppendLine("<table style=\"border-collapse: collapse; font-family: Arial, Helvetica, sans-serif; font-size: 14px;\">");

        // header
        sb.AppendLine("<thead>");
        sb.AppendLine("<tr>");
        for (int j = 0; j < table.ColumnCount; j++)
        {
            sb.AppendLine($"<th style=\"background-color: #1a3a5c; color: #ffffff; padding: 10px 16px; text-align: left; border: 1px solid #14304d; font-weight: 600;\">{table.ColumnNames[j]}</th>");
        }
        sb.AppendLine("</tr>");
        sb.AppendLine("</thead>");

        // rows
        sb.AppendLine("<tbody>");
        for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
        {
            string rowBg = rowIndex % 2 == 0 ? "#ffffff" : "#f2f6fa";
            sb.AppendLine($"<tr style=\"background-color: {rowBg};\">");
            for (int columnIndex = 0; columnIndex < table.ColumnCount; columnIndex++)
            {
                sb.AppendLine($"<td style=\"padding: 8px 16px; border: 1px solid #d4dce6; color: #333333;\">{table[rowIndex, columnIndex]}</td>");
            }
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody>");

        sb.AppendLine("</table>");
        return sb.ToString();
    }
}
