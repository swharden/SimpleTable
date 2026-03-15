using SimpleTable;

StringTable table = SampleData.Consecutive(3, 4);
Console.WriteLine(table.ToDisplayString());

table.Rotate90();
Console.WriteLine(table.ToDisplayString());