namespace SimpleTable;

public static class SampleData
{
    public static StringTable UsersTable()
    {
        StringTable table = new(["Name", "Email", "Color"]);
        table.AppendRow(["Scott", "scott@hotmail.com", "red"]);
        table.AppendRow(["James", "james@gmail.com", "green"]);
        table.AppendRow(["Ben", "ben@yahoo.com", "blue"]);
        table.AppendRow(["Rob", "rob@aol.com", "gray"]);
        return table;
    }
}
