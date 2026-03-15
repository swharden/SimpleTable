using System.Collections;

namespace SimpleTable;

public struct TableColumn : IEnumerable<string?>
{
    public int ColumnIndex { get; set; }
    public string ColumnName { get; set; }
    public List<string?> Values;
    public readonly string ValuesString => string.Join(", ", Values);
    public override readonly string ToString() => $"Column {ColumnName} with {Values.Count} values";

    public readonly IEnumerator<string?> GetEnumerator() => Values.GetEnumerator();
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
