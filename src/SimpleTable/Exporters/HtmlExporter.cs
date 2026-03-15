using System.Text;

namespace SimpleTable.Exporters;

public static class HtmlExporter
{
    public static string GetHtml(StringTable table)
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
}
