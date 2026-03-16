using SimpleTable.IO;

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

    [Test]
    public void Test_StringTable_AddColumn_ValuesOnly()
    {
        StringTable table = SampleData.UsersTable();
        table.AddColumn(["Dog", "Cat", "Fish", "Bird"], "Animal");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(4));
            Assert.That(string.Join(",", table.GetColumnValues("Animal")), Is.EqualTo("Dog,Cat,Fish,Bird"));
        }
    }

    [Test]
    public void Test_StringTable_AddColumn_TableColumn()
    {
        StringTable table = SampleData.UsersTable();
        TableColumn col = new() { ColumnName = "Animal", Values = ["Dog", "Cat", "Fish", "Bird"] };
        table.AddColumn(col);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(4));
            Assert.That(string.Join(",", table.GetColumnValues("Animal")), Is.EqualTo("Dog,Cat,Fish,Bird"));
        }
    }

    [Test]
    public void Test_StringTable_AddColumns_Enumerable()
    {
        StringTable table = SampleData.UsersTable();
        List<TableColumn> cols =
        [
            new() { ColumnName = "Animal", Values = ["Dog", "Cat", "Fish", "Bird"] },
            new() { ColumnName = "Vehicle", Values = ["Car", "Bike", "Bus", "Train"] },
        ];
        table.AddColumns(cols);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(5));
            Assert.That(table[0, "Animal"], Is.EqualTo("Dog"));
            Assert.That(table[3, "Vehicle"], Is.EqualTo("Train"));
        }
    }

    [Test]
    public void Test_StringTable_AddColumns_Table()
    {
        StringTable source = new(["X", "Y"]);
        source.AppendRow(["1", "2"]);
        source.AppendRow(["3", "4"]);

        StringTable table = new(["A"]);
        table.AppendRow(["10"]);
        table.AppendRow(["20"]);

        table.AddColumns(source);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.RowCount, Is.EqualTo(2));
            Assert.That(table[0, "X"], Is.EqualTo("1"));
            Assert.That(table[1, "Y"], Is.EqualTo("4"));
        }
    }

    [Test]
    public void Test_StringTable_GetColumn_ByIndex()
    {
        StringTable table = SampleData.UsersTable();
        TableColumn col = table.GetColumn(2); // Color

        using (Assert.EnterMultipleScope())
        {
            Assert.That(col.ColumnIndex, Is.EqualTo(2));
            Assert.That(col.ColumnName, Is.EqualTo("Color"));
            Assert.That(col.Values.Count, Is.EqualTo(4));
            Assert.That(col.Values[0], Is.EqualTo("red"));
        }
    }

    [Test]
    public void Test_StringTable_GetColumnValues_ByIndex()
    {
        StringTable table = SampleData.UsersTable();

        Assert.That(
            actual: string.Join(",", table.GetColumnValues(0)),
            expression: Is.EqualTo("Scott,James,Ben,Rob"));
    }

    [Test]
    public void Test_StringTable_SetColumnName()
    {
        StringTable table = SampleData.UsersTable();
        table.SetColumnName(1, "EmailAddress");

        Assert.That(table.ColumnNames[1], Is.EqualTo("EmailAddress"));
        Assert.That(table.IndexOf("EmailAddress"), Is.EqualTo(1));
        Assert.Throws<KeyNotFoundException>(() => table.IndexOf("Email"));
    }

    [Test]
    public void Test_StringTable_DeleteColumn_ByIndex()
    {
        StringTable table = SampleData.UsersTable(); // Name, Email, Color
        table.DeleteColumn(1); // Email

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(2));
            Assert.That(string.Join(",", table.ColumnNames), Is.EqualTo("Name,Color"));
            Assert.That(table.IndexOf("Color"), Is.EqualTo(1));
            Assert.That(table[0, "Color"], Is.EqualTo("red"));
        }
    }

    [Test]
    public void Test_StringTable_DeleteColumn_ByName()
    {
        StringTable table = SampleData.UsersTable();
        table.DeleteColumn("Email");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(2));
            Assert.That(string.Join(",", table.ColumnNames), Is.EqualTo("Name,Color"));
        }
    }

    [Test]
    public void Test_StringTable_DeleteColumn_TableColumn()
    {
        StringTable table = SampleData.UsersTable();
        TableColumn emailCol = table.GetColumn(1);
        table.DeleteColumn(emailCol);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(2));
            Assert.That(table.ColumnNames, Does.Not.Contain("Email"));
        }
    }
}
