using SimpleTable;

StringTable table = SampleData.UsersTable();
string saveAs = Path.GetFullPath("test.html");

string html = SimpleTable.Exporters.HtmlExporter.GetHtml(table);
html = SimpleTable.Exporters.Html.WrapInHtml(html);
File.WriteAllText(saveAs, html);
Console.WriteLine(saveAs);