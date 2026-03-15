namespace SimpleTable.Tests;

public class StringTableTransformTests
{
    [Test]
    public void Test_StringTable_Rotate90()
    {
        // UsersTable: 4 rows x 3 columns
        // After 90-degree rotation: 3 rows x 4 columns
        // Row 0 after rotation should read: Rob, Ben, James, Scott (last row becomes first column)
        StringTable table = SampleData.UsersTable();
        table.Rotate90();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(table.RowCount, Is.EqualTo(3));
            Assert.That(table.ColumnCount, Is.EqualTo(4));
            Assert.That(table[0, 0], Is.EqualTo("Rob"));
            Assert.That(table[0, 3], Is.EqualTo("Scott"));
            Assert.That(table[2, 0], Is.EqualTo("gray"));
            Assert.That(table[2, 3], Is.EqualTo("red"));
        }
    }
}
