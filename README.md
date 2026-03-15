# SimpleTable

[![Build and Deploy](https://github.com/swharden/SimpleTable/actions/workflows/cicd.yml/badge.svg)](https://github.com/swharden/SimpleTable/actions/workflows/cicd.yml)

**SimpleTable is a lightweight .NET library for working with tabular data in .NET Applications.** It provides a simple API designed for quick adoption and easy integration into existing applications. SimpleTable has no external dependencies, simplifying security review and deployment. Its intentionally small API surface area makes the codebase easy to understand while still allowing developers to extend functionality without modifying the core library.

<p></p>
<p></p>
<img src="dev/logo/simpletable.png" width="50%">
<p></p>
<p></p>

## Quickstart

```cs
// Create a table and add rows
StringTable table = new(["First", "Last", "Email"]);
table.AddRow(["Scott", "Harden", "scott@gmail.com"]);
table.AddRow(["Bob", "Ross", "bob@hotmail.com"]);
table.AddRow(["Grace", "Hopper", "grace@aol.com"]);
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email           |
|-------|--------|-----------------|
| Scott | Harden | scott@gmail.com |
| Bob   | Ross   | bob@hotmail.com |
| Grace | Hopper | grace@aol.com   |

```cs
// Add columns and rows
table.AddColumn("Pet", ["Fish", "Dog", "Cat"]);
table.AddRow(["Bob", "Martin", "solid@yahoo.com", "Bird"]);
Console.WriteLine(table.ToMarkdownString());
```

```cs
| First | Last   | Email           | Pet  |
|-------|--------|-----------------|------|
| Scott | Harden | scott@gmail.com | Fish |
| Bob   | Ross   | bob@hotmail.com | Dog  |
| Grace | Hopper | grace@aol.com   | Cat  |
| Bob   | Martin | solid@yahoo.com | Bird |
```

```cs
// Delete columns and rows
table.DeleteColumn("Pet");
table.DeleteRow(table.LastRowIndex);
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email           |
|-------|--------|-----------------|
| Scott | Harden | scott@gmail.com |
| Bob   | Ross   | bob@hotmail.com |
| Grace | Hopper | grace@aol.com   |

```cs
// Index cells individually to read or modify their values
table[row: 1, col: 2] = "bob@outlook.com";
Console.WriteLine(table.ToMarkdownString());
```

bob@hotmail.com
| First | Last   | Email           |
|-------|--------|-----------------|
| Scott | Harden | scott@gmail.com |
| Bob   | Ross   | bob@outlook.com |
| Grace | Hopper | grace@aol.com   |

```cs
// Rotate table data
table.Rotate90();
table.SetColumnNamesFromFirstRow();
```

| First | Last   | Email           |
|-------|--------|-----------------|
| Scott | Harden | scott@gmail.com |
| Bob   | Ross   | bob@outlook.com |
| Grace | Hopper | grace@aol.com   |

```cs
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
```

| Date      | Price   | Volume      |
|-----------|---------|-------------|
| 3/14/2026 | $172.00 | 817,508,034 |
| 3/15/2026 | $176.00 | 558,603,030 |
| 3/16/2026 | $120.00 | 559,325,909 |
| 3/17/2026 | $190.00 | 442,735,695 |
| 3/18/2026 | $197.00 | 274,430,753 |
| 3/19/2026 | $129.00 | 467,847,385 |
| 3/20/2026 | $163.00 | 470,042,366 |

```cs
// Export as CSV
Console.WriteLine(table.ToCsvString());
```

```txt
Date,Price,Volume
3/14/2026,$172.00,"817,508,034"
3/15/2026,$176.00,"558,603,030"
3/16/2026,$120.00,"559,325,909"
3/17/2026,$190.00,"442,735,695"
3/18/2026,$197.00,"274,430,753"
3/19/2026,$129.00,"467,847,385"
3/20/2026,$163.00,"470,042,366"
```

```cs
// Export as a HTML table and show it in the browser
table.LaunchInDefaultBrowser();
```

![](/dev/screenshots/quickstart-browser-table.png)