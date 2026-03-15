namespace SimpleTable;

public static class Testing
{
    public static void RandomlySetNull(StringTable table, double nullDensity = 0.5)
    {
        for (int i = 0; i < table.RowCount; i++)
        {
            for (int j = 0; j < table.ColumnCount; j++)
            {
                if (Random.Shared.NextDouble() < nullDensity)
                {
                    table[i, j] = null;
                }
            }
        }
    }
}