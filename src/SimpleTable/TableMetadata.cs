namespace SimpleTable;

public class TableMetadata
{
    public string Name { get; set; } = "Unnamed";
    public string Description { get; set; } = string.Empty;
    public List<string> Details { get; set; } = [];
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}
