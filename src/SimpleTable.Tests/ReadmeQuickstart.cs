using SimpleTable.IO;
using System.Diagnostics;

namespace SimpleTable.Tests;

internal class ReadmeQuickstart
{
    [Test]
    public void Test_Readme_Quickstart()
    {
        // using SimpleTable;

        // Create a table and add rows
        StringTable table = new(["First", "Last", "Email"]);
        table.AddRow(["Scott", "Harden", "gmail"]);
        table.AddRow(["Bob", "Ross", "hotmail"]);
        table.AddRow(["Grace", "Hopper", "aol"]);
        Console.WriteLine(table.ToMarkdownString());

        // Add columns and rows
        table.AddColumn("Pet", ["Fish", "Dog", "Cat"]);
        table.AddRow(["Bob", "Martin", "yahoo", "Bird"]);
        Console.WriteLine(table.ToMarkdownString());

        // Delete columns and rows
        table.DeleteColumn("Pet");
        table.DeleteRow(table.LastRowIndex);
        Console.WriteLine(table.ToMarkdownString());

        // Index cells individually to read or modify their values
        Console.WriteLine(table[row: 1, col: 2]);
        table[row: 1, col: 2] = "outlook";
        Console.WriteLine(table.ToMarkdownString());

        // Rotate table data
        table.Rotate90();
        table.SetColumnNamesFromFirstRow();
        Console.WriteLine(table.ToMarkdownString());

        // Clear the table and repopulate it dynamically
        table.Clear();
        table.AddColumns(["Date", "Price", "Volume"]);
        Random rand = new(0);
        for (int i = 0; i < 7; i++)
        {
            table.AddRow([
                    new DateTime(2026,03,14).AddDays(i).ToShortDateString(),
                    rand.Next(100,200).ToString("$#,##0.00"),
                    rand.Next((int)1e6, (int)1e9).ToString("N0"),
                ]);
        }
        Console.WriteLine(table.ToMarkdownString());

        // Export as CSV
        Console.WriteLine(table.ToCsvString());

        // Export as a HTML table and show it in the browser
        if (Debugger.IsAttached)
            table.LaunchInDefaultBrowser();

        // Prepare a second table with sample data
        StringTable table2 = SampleData.Consecutive(4, 3);
        Console.WriteLine(table2.ToMarkdownString());

        // Combine two tables horizontally (jagged shapes are okay)
        table.AddColumns(table2);
        Console.WriteLine(table.ToMarkdownString());
    }
}
