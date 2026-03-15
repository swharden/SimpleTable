using SimpleTable;

StringTable table = SampleData.UsersTable();

foreach (var row in table.Rows)
{
    Console.WriteLine(row);
    foreach (var cell in row)
    {
        Console.WriteLine("  " + cell);
    }
}