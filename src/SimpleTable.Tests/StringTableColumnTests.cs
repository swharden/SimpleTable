namespace SimpleTable.Tests;

public class StringTableColumnTests
{
    [Test]
    public void Test_StringTable_AddColumn_Empty()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        table.AddColumn("Animal");
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount + 1));
        }
    }

    [Test]
    public void Test_StringTable_AddColumn_Partial()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        table.AddColumn("Animal", ["Dog", "Cat"]);
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount + 1));
        }
    }

    [Test]
    public void Test_StringTable_AddColumn_Large()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        string[] large = Enumerable.Range(0, 10).Select(x => $"TEST{x:00}").ToArray();
        table.AddColumn("Animal", large);
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(large.Length));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount + 1));
        }
    }

    [Test]
    public void Test_StringTable_GetColumnValues()
    {
        StringTable table = SampleData.UsersTable();

        Assert.That(
            actual: string.Join(",", table.GetColumnValues("Color")),
            expression: Is.EqualTo("red,green,blue,gray"));
    }

    [Test]
    public void Test_StringTable_SetColumnNames()
    {
        StringTable table = SampleData.UsersTable();
        table.SetColumnNames(["test1", "test2"]);
        Assert.That(string.Join(",", table.ColumnNames), Is.EqualTo("test1,test2,Color"));
    }

    [Test]
    public void Test_StringTable_IterateColumns()
    {
        StringTable table = SampleData.UsersTable();
        foreach (var col in table.Columns)
        {
            Console.WriteLine(col);
            foreach (var cell in col.Values)
            {
                Console.WriteLine($"  {cell}");
            }
        }
    }
}
