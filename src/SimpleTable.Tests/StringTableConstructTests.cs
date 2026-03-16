using SimpleTable.IO;

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
            Assert.That(table.ColumnCount, Is.EqualTo(0));
            Assert.That(table.RowCount, Is.EqualTo(0));
        }
    }

    [Test]
    public void Test_StringTable_Construct_ClearRows()
    {
        StringTable table = SampleData.UsersTable();
        table.ClearRows();

        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.RowCount, Is.EqualTo(0));
        }
    }

    [Test]
    public void Test_StringTable_Construct_WithDimensions()
    {
        StringTable table = new(3, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(3));
            Assert.That(table.ColumnCount, Is.EqualTo(4));
            Assert.That(table.ColumnNames[0], Is.EqualTo("A"));
            Assert.That(table.ColumnNames[3], Is.EqualTo("D"));
        }
    }

    [Test]
    public void Test_StringTable_Construct_DuplicateColumnName_Throws()
    {
        Assert.Throws<ArgumentException>(() => _ = new StringTable(["Name", "Email", "Name"]));
    }

    [Test]
    public void Test_StringTable_ClearRows()
    {
        StringTable table = SampleData.UsersTable();
        table.ClearRows();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.RowCount, Is.EqualTo(0));
            Assert.That(string.Join(",", table.ColumnNames), Is.EqualTo("Name,Email,Color"));
        }
    }

    [Test]
    public void Test_StringTable_DefaultColumnName()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(StringTable.DefaultColumnName(1), Is.EqualTo("A"));
            Assert.That(StringTable.DefaultColumnName(2), Is.EqualTo("B"));
            Assert.That(StringTable.DefaultColumnName(26), Is.EqualTo("Z"));
            Assert.That(StringTable.DefaultColumnName(27), Is.EqualTo("AA"));
            Assert.That(StringTable.DefaultColumnName(28), Is.EqualTo("AB"));
        }
    }

    [Test]
    public void Test_StringTable_ToString()
    {
        StringTable table = SampleData.UsersTable();
        string str = table.ToString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(str, Does.Contain("StringTable"));
            Assert.That(str, Does.Contain("4 rows"));
            Assert.That(str, Does.Contain("3 columns"));
        }
    }
}
