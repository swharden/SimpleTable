using SimpleTable;

StringTable table = SampleData.UsersTable();
Console.WriteLine(table.ToDisplayString());

table.AddRow(table.GetRowValues(table.LastRowIndex));
var lastColumnValues = table.GetColumnValues(table.LastColumnIndex);

Console.WriteLine(string.Join(",", lastColumnValues));
table.AddColumn(lastColumnValues);

Console.WriteLine(table.ToDisplayString());
