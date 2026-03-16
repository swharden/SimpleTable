using SimpleTable.IO;

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

    [Test]
    public void Test_StringTable_GetRow_ByIndex()
    {
        StringTable table = SampleData.UsersTable();
        TableRow row = table.GetRow(2); // Ben

        using (Assert.EnterMultipleScope())
        {
            Assert.That(row.RowIndex, Is.EqualTo(2));
            Assert.That(row.Values.Count, Is.EqualTo(3));
            Assert.That(row.Values[0], Is.EqualTo("Ben"));
        }
    }

    [Test]
    public void Test_StringTable_AppendRow_WrongCount_Throws()
    {
        StringTable table = SampleData.UsersTable();
        Assert.Throws<ArgumentException>(() => table.AppendRow(["A", "B"]));
    }

    [Test]
    public void Test_StringTable_AddRow_TableRow()
    {
        StringTable table = SampleData.UsersTable();
        TableRow firstRow = table.GetRow(0);
        table.AddRow(firstRow);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(5));
            Assert.That(table[4, "Name"], Is.EqualTo("Scott"));
        }
    }

    [Test]
    public void Test_StringTable_AddRows_Enumerable()
    {
        StringTable table = SampleData.UsersTable();
        List<TableRow> newRows =
        [
            new() { RowIndex = 0, Values = ["Alice", "alice@test.com", "purple"] },
            new() { RowIndex = 0, Values = ["Bob", "bob@test.com", "orange"] },
        ];
        table.AddRows(newRows);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(6));
            Assert.That(table[4, "Name"], Is.EqualTo("Alice"));
            Assert.That(table[5, "Name"], Is.EqualTo("Bob"));
        }
    }

    [Test]
    public void Test_StringTable_AddRows_Table()
    {
        StringTable table = SampleData.UsersTable();
        StringTable extra = new(["Name", "Email", "Color"]);
        extra.AppendRow(["Alice", "alice@test.com", "purple"]);
        table.AddRows(extra);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(5));
            Assert.That(table[4, "Name"], Is.EqualTo("Alice"));
        }
    }

    [Test]
    public void Test_StringTable_DeleteRow_ByIndex()
    {
        StringTable table = SampleData.UsersTable();
        table.DeleteRow(1); // James

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(3));
            Assert.That(table[1, "Name"], Is.EqualTo("Ben"));
        }
    }

    [Test]
    public void Test_StringTable_DeleteRow_TableRow()
    {
        StringTable table = SampleData.UsersTable();
        TableRow benRow = table.GetRow(2); // Ben
        table.DeleteRow(benRow);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(3));
            Assert.That(table[2, "Name"], Is.EqualTo("Rob"));
        }
    }
}
