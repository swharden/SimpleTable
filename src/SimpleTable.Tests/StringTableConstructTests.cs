namespace SimpleTable.Tests;

public class StringTableConstructTests
{
    [Test]
    public void Test_StringTable_Construct()
    {
        StringTable table = new(["Name", "Email", "Color"]);
        table.AppendRow(["Scott", "scott@hotmail.com", "red"]);
        table.AppendRow(["James", "james@gmail.com", "green"]);
        table.AppendRow(["Ben", "ben@yahoo.com", "blue"]);
        table.AppendRow(["Rob", "rob@aol.com", "gray"]);

        Console.WriteLine(table.ToDisplayString());
        Console.WriteLine(table.ToMarkdownString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.RowCount, Is.EqualTo(4));
        }
    }

    [Test]
    public void Test_StringTable_Construct_Empty()
    {
        StringTable table = new();
        Console.WriteLine(table.ToDisplayString());
        Console.WriteLine(table.ToMarkdownString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(0));
            Assert.That(table.RowCount, Is.EqualTo(0));
        }
    }

    [Test]
    public void Test_StringTable_Construct_Clear()
    {
        StringTable table = SampleData.UsersTable();
        table.Clear();

        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.RowCount, Is.EqualTo(0));
        }
    }
}
