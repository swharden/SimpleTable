namespace SimpleTable.Tests;

public class StringTableOutputTests
{
    [Test]
    public void Test_StringTable_ToCsvString()
    {
        StringTable table = SampleData.UsersTable();
        string csv = table.ToCsvString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(csv, Does.Contain("Name, Email, Color"));
            Assert.That(csv, Does.Contain("Scott, scott@hotmail.com, red"));
            Assert.That(csv, Does.Contain("Rob, rob@aol.com, gray"));
        }
    }

    [Test]
    public void Test_StringTable_NullDisplayString()
    {
        StringTable table = new(["A", "B"]);
        table.AddRow(["hello", null]);

        string original = StringTable.NullDisplayString;
        try
        {
            Assert.That(StringTable.NullDisplayString, Is.EqualTo("--"));
            Assert.That(table.ToDisplayString(), Does.Contain("--"));

            StringTable.NullDisplayString = "N/A";
            Assert.That(table.ToDisplayString(), Does.Contain("N/A"));
        }
        finally
        {
            StringTable.NullDisplayString = original;
        }
    }

    [Test]
    public void Test_StringTable_ToDisplayString()
    {
        StringTable table = SampleData.UsersTable();
        string display = table.ToDisplayString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(display, Does.Contain("Name"));
            Assert.That(display, Does.Contain("Email"));
            Assert.That(display, Does.Contain("Scott"));
            Assert.That(display, Does.Contain("+"));
        }
    }

    [Test]
    public void Test_StringTable_ToMarkdownString()
    {
        StringTable table = SampleData.UsersTable();
        string md = table.ToMarkdownString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(md, Does.Contain("| Name"));
            Assert.That(md, Does.Contain("| Scott"));
            Assert.That(md, Does.Contain("|---"));
        }
    }
}
