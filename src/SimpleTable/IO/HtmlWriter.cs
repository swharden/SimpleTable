using System.Text;

namespace SimpleTable.IO;

public static class HtmlWriter
{
    public static void LaunchInDefaultBrowser(this StringTable table, string? saveAs = null)
    {
        saveAs ??= $"{DateTime.UtcNow.Ticks}.html";
        saveAs = Path.GetFullPath(saveAs);

        string html = WrapInHtml(GetStyledHtml(table));
        File.WriteAllText(saveAs, html);
        Launch.DefaultBrowser(saveAs);
    }

    public static string WrapInHtml(string html, string title = "")
    {
        return $"""
            <!doctype html>
            <html lang="en">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>{title}</title>
              </head>
              <body>
                {html}
              </body>
            </html>
            """;
    }

    public static string WrapInBootstrap(string html, string title = "")
    {
        return $"""
            <!doctype html>
            <html lang="en">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>{title}</title>
                <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB" crossorigin="anonymous">
              </head>
              <body>
                {html}
                <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js" integrity="sha384-FKyoEForCGlyvwx9Hj09JcYn3nv7wiPVlz7YYwJrWVcXK/BmnVDxM+D2scQbITxI" crossorigin="anonymous"></script>
              </body>
            </html>
            """;
    }

    public static string GetUnstyledHtml(StringTable table)
    {
        StringBuilder sb = new();
        sb.AppendLine("<table>");

        // header
        sb.AppendLine("<thead>");
        sb.AppendLine("<tr>");
        foreach (string columnName in table.ColumnNames)
        {
            sb.AppendLine($"<th>{columnName}</th>");
        }
        sb.AppendLine("</tr>");
        sb.AppendLine("</thead>");

        // rows
        sb.AppendLine("<tbody>");
        foreach (var row in table.Rows)
        {
            sb.AppendLine("<tr>");
            foreach (var cell in row.Values)
            {
                sb.AppendLine($"<td>{cell}</td>");
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

        if (table.Metadata.Title != "Unnamed")
        {
            sb.AppendLine($"<h1>{table.Metadata.Title}</h1>");
            sb.AppendLine($"<div>{table.Metadata.DateTime}</div>");
        }

        if (!string.IsNullOrWhiteSpace(table.Metadata.Description))
        {
            sb.AppendLine($"<div>{table.Metadata.Description}</div>");
        }

        if (table.Metadata.Details.Count > 0)
        {
            sb.AppendLine("<ul>");
            foreach (string detail in table.Metadata.Details)
            {
                sb.AppendLine($"<li>{detail}</li>");
            }
            sb.AppendLine("</ul>");
        }

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
