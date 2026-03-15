namespace SimpleTable.Tests;

public class StringTableIndexTests
{
    [Test]
    public void Test_StringTable_Index()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        table[1, 2] = "HELLO"; // [row index, column index]
        table[0, "Email"] = "WORLD"; // [row index, column name]
        Assert.That(table.Dimensions, Is.EqualTo(initialDimensions));

        Console.WriteLine(table.ToDisplayString());
    }
}
