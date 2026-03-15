using SimpleTable;

StringTable table1 = SampleData.Consecutive(2, 3);
Console.WriteLine(table1.ToDisplayString());

StringTable table2 = SampleData.Consecutive(3, 4);
Console.WriteLine(table2.ToDisplayString());

Console.WriteLine(string.Join(",", table1.GetValues(byRow: true)));
Console.WriteLine(string.Join(",", table1.GetValues(byRow: false)));

//Console.WriteLine(table1.ToDisplayString());