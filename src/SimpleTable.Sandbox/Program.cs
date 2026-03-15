using SimpleTable;

StringTable table = SampleData.UsersTable();
Console.WriteLine(table);
Console.WriteLine(table.ToDisplayString());
Console.WriteLine(table.Dimensions);

Console.WriteLine(string.Join(",", table.GetColumnValues("Color")));

Console.WriteLine(string.Join(",", table.GetRowValues(1)));