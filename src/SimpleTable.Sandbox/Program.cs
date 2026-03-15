using SimpleTable;

StringTable table = SampleData.UsersTable();

foreach (var col in table.Columns)
{
    Console.WriteLine(col);
    foreach(var cell in col)
    {
        Console.WriteLine(cell);
    }
}