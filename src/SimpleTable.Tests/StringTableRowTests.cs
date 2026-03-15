namespace SimpleTable.Tests;

public class StringTableRowTests
{
    [Test]
    public void Test_StringTable_AddRow_Empty()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        table.AddRow();
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount + 1));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount));
        }
    }

    [Test]
    public void Test_StringTable_AddRow_Last()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        var lastRow = table.GetRowValues(table.RowCount - 1);
        table.AddRow(lastRow);
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount + 1));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount));
        }
    }

    [Test]
    public void Test_StringTable_AddRow_Partial()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        table.AddRow(["test"]);
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount + 1));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(initialDimensions.ColumnCount));
        }
    }

    [Test]
    public void Test_StringTable_AddRow_Large()
    {
        StringTable table = SampleData.UsersTable();
        TableDimensions initialDimensions = table.Dimensions;

        string[] large = Enumerable.Range(0, 10).Select(x => $"TEST{x:00}").ToArray();
        table.AddRow(large);
        Console.WriteLine(table.ToDisplayString());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.Dimensions.RowCount, Is.EqualTo(initialDimensions.RowCount + 1));
            Assert.That(table.Dimensions.ColumnCount, Is.EqualTo(large.Length));
        }
    }

    [Test]
    public void Test_StringTable_GetRowValues()
    {
        StringTable table = SampleData.UsersTable();

        Assert.That(
            actual: string.Join(",", table.GetRowValues(1)),
            expression: Is.EqualTo("James,james@gmail.com,green"));
    }

    [Test]
    public void Test_StringTable_IterateRows()
    {
        StringTable table = SampleData.UsersTable();
        foreach (var row in table.Rows)
        {
            Console.WriteLine(row);
            foreach (var cell in row.Values)
            {
                Console.WriteLine($"  {cell}");
            }
        }
    }
}
