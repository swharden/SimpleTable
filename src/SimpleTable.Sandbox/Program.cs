using SimpleTable;

StringTable table = SampleData.UsersTable();
Console.WriteLine(table.ToDisplayString());

table.AddRow(table.GetRowValues(table.RowCount - 1));
Console.WriteLine(table.ToDisplayString());
