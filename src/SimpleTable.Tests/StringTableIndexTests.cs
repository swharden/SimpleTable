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

    [Test]
    public void Test_StringTable_GetValue_ByIndex()
    {
        StringTable table = SampleData.UsersTable();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.GetValue(0, 0), Is.EqualTo("Scott"));
            Assert.That(table.GetValue(3, 2), Is.EqualTo("gray"));
        }
    }

    [Test]
    public void Test_StringTable_GetValue_ByName()
    {
        StringTable table = SampleData.UsersTable();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.GetValue(1, "Name"), Is.EqualTo("James"));
            Assert.That(table.GetValue(2, "Color"), Is.EqualTo("blue"));
        }
    }

    [Test]
    public void Test_StringTable_SetValue_ByIndex()
    {
        StringTable table = SampleData.UsersTable();
        table.SetValue(0, 0, "ALICE");
        Assert.That(table[0, 0], Is.EqualTo("ALICE"));
    }

    [Test]
    public void Test_StringTable_SetValue_ByName()
    {
        StringTable table = SampleData.UsersTable();
        table.SetValue(1, "Color", "yellow");
        Assert.That(table[1, "Color"], Is.EqualTo("yellow"));
    }

    [Test]
    public void Test_StringTable_SetValue_ByTableRowAndColumn()
    {
        StringTable table = SampleData.UsersTable();
        TableRow row = table.GetRow(0);
        TableColumn column = table.GetColumn(2); // Color
        table.SetValue(row, column, "orange");
        Assert.That(table[0, "Color"], Is.EqualTo("orange"));
    }

    [Test]
    public void Test_StringTable_IndexOf()
    {
        StringTable table = SampleData.UsersTable();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.IndexOf("Name"), Is.EqualTo(0));
            Assert.That(table.IndexOf("Email"), Is.EqualTo(1));
            Assert.That(table.IndexOf("Color"), Is.EqualTo(2));
        }
    }

    [Test]
    public void Test_StringTable_IndexOf_UnknownName_Throws()
    {
        StringTable table = SampleData.UsersTable();
        Assert.Throws<KeyNotFoundException>(() => table.IndexOf("Unknown"));
    }

    [Test]
    public void Test_StringTable_GetValues_ByRow()
    {
        StringTable table = SampleData.UsersTable();
        List<string?> values = table.GetValues(byRow: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(values.Count, Is.EqualTo(12));
            Assert.That(values[0], Is.EqualTo("Scott")); // row 0, col 0
            Assert.That(values[3], Is.EqualTo("James")); // row 1, col 0
        }
    }

    [Test]
    public void Test_StringTable_GetValues_ByColumn()
    {
        StringTable table = SampleData.UsersTable();
        List<string?> values = table.GetValues(byRow: false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(values.Count, Is.EqualTo(12));
            Assert.That(values[0], Is.EqualTo("Scott"));           // col 0, row 0
            Assert.That(values[4], Is.EqualTo("scott@hotmail.com")); // col 1, row 0
        }
    }

    [Test]
    public void Test_StringTable_LastRowIndex()
    {
        StringTable table = SampleData.UsersTable(); // 4 rows
        Assert.That(table.LastRowIndex, Is.EqualTo(3));
    }

    [Test]
    public void Test_StringTable_LastColumnIndex()
    {
        StringTable table = SampleData.UsersTable(); // 3 columns
        Assert.That(table.LastColumnIndex, Is.EqualTo(2));
    }
}
