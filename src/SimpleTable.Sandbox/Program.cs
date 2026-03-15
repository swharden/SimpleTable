using SimpleTable;

StringTable table = SampleData.Consecutive(3, 4);
Console.WriteLine(table.ToDisplayString());

table.DeleteRow(1);
table.DeleteColumn(2);
table.DeleteColumn("D");
Console.WriteLine(table.ToDisplayString());