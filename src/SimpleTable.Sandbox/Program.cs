using SimpleTable;

StringTable table = SampleData.UsersTable();
Console.WriteLine(table.ToDisplayString());
Testing.RandomlySetNull(table);
Console.WriteLine(table.ToMarkdownString());
