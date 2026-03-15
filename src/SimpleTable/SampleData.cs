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

    public static StringTable Consecutive(int rowCount, int colCount, double initial = 1.0, double step = 1.0)
    {
        double value = initial;

        StringTable table = new(rowCount, colCount);
        foreach (var row in table.Rows)
        {
            foreach (var col in table.Columns)
            {
                table.SetValue(row.RowIndex, col.ColumnIndex, value.ToString());
                value += step;
            }
        }
        return table;
    }
}
