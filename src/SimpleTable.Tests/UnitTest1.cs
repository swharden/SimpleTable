namespace SimpleTable.Tests;

public class Tests
{
    [Test]
    public void Test1()
    {
        StringTable table = new(["Name", "Email", "Color"]);

        table.AppendRow(["Scott", "scott@hotmail.com", "red"]);
        table.AppendRow(["James", "james@gmail.com", "green"]);
        table.AppendRow(["Ben", "ben@yahoo.com", "blue"]);

        Console.WriteLine(table.ToDisplayString());
    }
}
