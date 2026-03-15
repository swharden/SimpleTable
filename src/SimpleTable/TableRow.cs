using System.Collections;

namespace SimpleTable;

public struct TableRow : IEnumerable<string?>
{
    public int RowIndex { get; set; }
    public List<string?> Values;
    public readonly string ValuesString => string.Join(", ", Values);
    public override readonly string ToString() => $"Row {RowIndex}: {ValuesString}";

    public readonly IEnumerator<string?> GetEnumerator() => Values.GetEnumerator();
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}