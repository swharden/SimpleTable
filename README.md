# SimpleTable

[![Build and Deploy](https://github.com/swharden/SimpleTable/actions/workflows/cicd.yml/badge.svg)](https://github.com/swharden/SimpleTable/actions/workflows/cicd.yml)

**SimpleTable is a lightweight library for working with tabular data in .NET applications.** It provides a simple API designed for quick adoption and easy integration into existing applications. SimpleTable has no external dependencies, simplifying security review and deployment. Its intentionally small API surface area makes the codebase easy to understand while still allowing developers to extend functionality without modifying the core library.

<p align="center">
<img src="dev/logo/simpletable.png" width="50%">
</p>

## Quickstart

### Create a table and add rows

```cs
StringTable table = new(["First", "Last", "Email"]);
table.AddRow(["Scott", "Harden", "gmail"]);
table.AddRow(["Bob", "Ross", "hotmail"]);
table.AddRow(["Grace", "Hopper", "aol"]);
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email   |
|-------|--------|---------|
| Scott | Harden | gmail   |
| Bob   | Ross   | hotmail |
| Grace | Hopper | aol     |

### Add columns and rows

```cs
table.AddColumn("Pet", ["Fish", "Dog", "Cat"]);
table.AddRow(["Bob", "Martin", "yahoo", "Bird"]);
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email   | Pet  |
|-------|--------|---------|------|
| Scott | Harden | gmail   | Fish |
| Bob   | Ross   | hotmail | Dog  |
| Grace | Hopper | aol     | Cat  |
| Bob   | Martin | yahoo   | Bird |

### Delete columns and rows

```cs
table.DeleteColumn("Pet");
table.DeleteRow(table.LastRowIndex);
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email   |
|-------|--------|---------|
| Scott | Harden | gmail   |
| Bob   | Ross   | hotmail |
| Grace | Hopper | aol     |

### Index cells individually to read or modify their values

```cs
table[row: 1, col: 2] = "outlook";
Console.WriteLine(table.ToMarkdownString());
```

| First | Last   | Email   |
|-------|--------|---------|
| Scott | Harden | gmail   |
| Bob   | Ross   | outlook |
| Grace | Hopper | aol     |

### Rotate table data

```cs
table.Rotate90();
table.SetColumnNamesFromFirstRow();
```

| Grace  | Bob     | Scott  |
|--------|---------|--------|
| Hopper | Ross    | Harden |
| aol    | outlook | gmail  |

### Clear the table and repopulate it dynamically

```cs
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

### Export as CSV

```cs
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

### Export as a HTML table and show it in the browser

```cs
table.LaunchInDefaultBrowser();
```

![](/dev/screenshots/quickstart-browser-table.png)

### Prepare a second table with sample data

```cs
StringTable table2 = SampleData.Consecutive(4, 3);
Console.WriteLine(table2.ToMarkdownString());
```

| A  | B  | C  |
|----|----|----|
| 1  | 2  | 3  |
| 4  | 5  | 6  |
| 7  | 8  | 9  |
| 10 | 11 | 12 |

### Combine two tables horizontally 

```cs
table.AddColumns(table2);
Console.WriteLine(table.ToMarkdownString());
```
| Date      | Price   | Volume      | A  | B  | C  |
|-----------|---------|-------------|----|----|----|
| 3/14/2026 | $172.00 | 817,508,034 | 1  | 2  | 3  |
| 3/15/2026 | $176.00 | 558,603,030 | 4  | 5  | 6  |
| 3/16/2026 | $120.00 | 559,325,909 | 7  | 8  | 9  |
| 3/17/2026 | $190.00 | 442,735,695 | 10 | 11 | 12 |
| 3/18/2026 | $197.00 | 274,430,753 | -- | -- | -- |
| 3/19/2026 | $129.00 | 467,847,385 | -- | -- | -- |
| 3/20/2026 | $163.00 | 470,042,366 | -- | -- | -- |