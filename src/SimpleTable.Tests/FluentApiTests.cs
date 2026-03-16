namespace SimpleTable.Tests;

internal class FluentApiTests
{
    [Test]
    public void Test_FluentApi_AddColumns()
    {
        StringTable table = new StringTable()
            .AddColumn("Name")
            .AddColumn("Age")
            .AddColumn("City");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(3));
            Assert.That(table.ColumnNames, Is.EqualTo(["Name", "Age", "City"]));
        }
    }

    [Test]
    public void Test_FluentApi_AddRows()
    {
        StringTable table = new StringTable(["Name", "Age", "City"])
            .AddRow(["Alice", "30", "New York"])
            .AddRow(["Bob", "25", "London"])
            .AddRow(["Carol", "35", "Paris"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(3));
            Assert.That(table[0, "Name"], Is.EqualTo("Alice"));
            Assert.That(table[1, "Name"], Is.EqualTo("Bob"));
            Assert.That(table[2, "City"], Is.EqualTo("Paris"));
        }
    }

    [Test]
    public void Test_FluentApi_SetValues()
    {
        StringTable table = new StringTable(2, 2)
            .SetValue(0, 0, "A1")
            .SetValue(0, 1, "B1")
            .SetValue(1, 0, "A2")
            .SetValue(1, 1, "B2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table[0, 0], Is.EqualTo("A1"));
            Assert.That(table[0, 1], Is.EqualTo("B1"));
            Assert.That(table[1, 0], Is.EqualTo("A2"));
            Assert.That(table[1, 1], Is.EqualTo("B2"));
        }
    }

    [Test]
    public void Test_FluentApi_SetColumnNames()
    {
        StringTable table = new StringTable(0, 3)
            .SetColumnNames(["First", "Second", "Third"])
            .AddRow(["x", "y", "z"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnNames, Is.EqualTo(["First", "Second", "Third"]));
            Assert.That(table[0, "Second"], Is.EqualTo("y"));
        }
    }

    [Test]
    public void Test_FluentApi_DeleteColumnAndRow()
    {
        StringTable table = new StringTable(["A", "B", "C"])
            .AddRow(["1", "2", "3"])
            .AddRow(["4", "5", "6"])
            .DeleteColumn("B")
            .DeleteRow(0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.ColumnCount, Is.EqualTo(2));
            Assert.That(table.RowCount, Is.EqualTo(1));
            Assert.That(table[0, "A"], Is.EqualTo("4"));
            Assert.That(table[0, "C"], Is.EqualTo("6"));
        }
    }

    [Test]
    public void Test_FluentApi_ExpandAndClear()
    {
        StringTable table = new StringTable()
            .Expand(3, 3)
            .SetValue(1, 1, "center")
            .ClearRows();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(0));
            Assert.That(table.ColumnCount, Is.EqualTo(3));
        }
    }

    [Test]
    public void Test_FluentApi_ComplexChain()
    {
        StringTable table = new StringTable()
            .AddColumn("Product")
            .AddColumn("Price")
            .AddRow(["Widget", "9.99"])
            .AddRow(["Gadget", "19.99"])
            .AddRow(["Doohickey", "4.99"])
            .SetValue(0, "Price", "10.00")
            .DeleteRow(2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(2));
            Assert.That(table[0, "Product"], Is.EqualTo("Widget"));
            Assert.That(table[0, "Price"], Is.EqualTo("10.00"));
            Assert.That(table[1, "Product"], Is.EqualTo("Gadget"));
        }
    }
}
