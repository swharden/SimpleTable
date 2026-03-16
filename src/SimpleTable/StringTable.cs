namespace SimpleTable;

#pragma warning disable IDE1006 // Ignore properties named with an underscore

/// <summary>
/// A simple table that stores all cell values as nullable strings
/// </summary>
public sealed class StringTable
{
    #region Private Data

    private List<List<string?>> _ValuesByRow { get; } = []; // TODO: make list of lists
    private List<string> _ColumnNames { get; }
    private Dictionary<string, int> _ColumnIndexes { get; }

    #endregion

    #region Public Properties

    /// <summary>
    /// Number of columns in the table
    /// </summary>
    public int ColumnCount => _ColumnNames.Count;

    /// <summary>
    /// Number of rows in the table
    /// </summary>
    public int RowCount => _ValuesByRow.Count;

    /// <summary>
    /// Collection of column names
    /// </summary>
    public IReadOnlyList<string> ColumnNames => _ColumnNames;

    /// <summary>
    /// Additional information about the table (title, description etc.)
    /// </summary>
    public TableMetadata Metadata { get; set; } = new();

    /// <summary>
    /// A snapshot of the current dimensions of the table
    /// </summary>
    public TableDimensions Dimensions => new(RowCount, ColumnCount);

    /// <summary>
    /// Index of the bottom row of the table
    /// </summary>
    public int LastRowIndex => RowCount - 1;

    /// <summary>
    /// Index of the far right column of the table
    /// </summary>
    public int LastColumnIndex => ColumnCount - 1;

    /// <summary>
    /// Display this value for null values when displaying tables as text
    /// </summary>
    public string NullDisplayString { get; set; } = "--";

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new table of the given size, initializing all cells with null values
    /// </summary>
    public StringTable(int rowCount = 0, int columnCount = 0)
    {
        _ColumnNames = [];
        _ColumnIndexes = new Dictionary<string, int>(_ColumnNames.Count, StringComparer.Ordinal);
        Expand(rowCount, columnCount);
    }

    /// <summary>
    /// Create a new table with the given collection of column names and no rows.
    /// </summary>
    public StringTable(IEnumerable<string> columnNames)
    {
        _ColumnNames = columnNames.ToList();
        _ColumnIndexes = new Dictionary<string, int>(_ColumnNames.Count, StringComparer.Ordinal);

        for (int i = 0; i < _ColumnNames.Count; i++)
        {
            if (!_ColumnIndexes.TryAdd(_ColumnNames[i], i))
                throw new ArgumentException($"Duplicate column name '{_ColumnNames[i]}'.", nameof(columnNames));
        }
    }

    #endregion

    #region Column Operations

    /// <summary>
    /// Return an Excel style column name for a given column index.
    /// <code>
    /// A, B, C, ..., AA, AB, AC, ..., etc.
    /// </code>
    /// </summary>
    public static string DefaultColumnName(int columnIndex)
    {
        string columnName = "";

        while (columnIndex > 0)
        {
            columnIndex--;
            columnName = (char)('A' + (columnIndex % 26)) + columnName;
            columnIndex /= 26;
        }

        return columnName;
    }

    /// <summary>
    /// Add a column to the right side of the table filled with null values
    /// </summary>
    public StringTable AddColumn(string? columnName = null)
    {
        columnName ??= DefaultColumnName(ColumnCount + 1);

        int newIndex = ColumnCount;
        _ColumnNames.Add(columnName);
        _ColumnIndexes[columnName] = newIndex;

        for (int i = 0; i < RowCount; i++)
        {
            _ValuesByRow[i].Add(null);
        }

        return this;
    }

    /// <summary>
    /// Add a column to the right side of the table and populate it with the given collection of values.
    /// The number of values does not have to equal the number of rows: 
    /// the table may be partially filled or expanded to accommodate whatever values are passed in.
    /// </summary>
    public StringTable AddColumn(string columnName, IReadOnlyList<string?> values)
    {
        AddColumn(values, columnName);
        return this;
    }

    /// <summary>
    /// Add a column to the right side of the table and populate it with the given collection of values.
    /// The number of values does not have to equal the number of rows: 
    /// the table may be partially filled or expanded to accommodate whatever values are passed in.
    /// </summary>
    public StringTable AddColumn(IReadOnlyList<string?> values, string? columnName = null)
    {
        AddColumn(columnName);

        Expand(values.Count, ColumnCount);

        for (int i = 0; i < values.Count; i++)
        {
            this[i, LastColumnIndex] = values[i];
        }

        return this;
    }

    /// <summary>
    /// Add a column to the right side of the table and populate it with the given values.
    /// The number of values does not have to equal the number of rows: 
    /// the table may be partially filled or expanded to accommodate whatever values are passed in.
    /// </summary>
    public StringTable AddColumn(TableColumn column)
    {
        AddColumn(column.Values, column.ColumnName);
        return this;
    }

    /// <summary>
    /// Add multiple columns to the right side of the table filled with null values
    /// </summary>
    public StringTable AddColumns(IEnumerable<string> columnNames)
    {
        foreach (string columnName in columnNames)
            AddColumn(columnName);

        return this;
    }

    /// <summary>
    /// Add multiple columns to the right side of the table filled with the given values
    /// </summary>
    public StringTable AddColumns(IEnumerable<TableColumn> columns)
    {
        foreach (TableColumn column in columns)
            AddColumn(column.Values, column.ColumnName);

        return this;
    }

    /// <summary>
    /// Copy all columns from the given table to the right side of the table
    /// </summary>
    public StringTable AddColumns(StringTable table)
    {
        AddColumns(table.Columns);
        return this;
    }

    /// <summary>
    /// Delete the column with the given name from the table
    /// </summary>
    public StringTable DeleteColumn(string columnName) => DeleteColumn(_ColumnIndexes[columnName]);

    /// <summary>
    /// Delete the given column from the table
    /// </summary>
    public StringTable DeleteColumn(TableColumn column) => DeleteColumn(column.ColumnIndex);

    /// <summary>
    /// Delete the given column index (starting at 0) from the table
    /// </summary>
    public StringTable DeleteColumn(int columnIndex)
    {
        string columnName = _ColumnNames[columnIndex];
        _ColumnNames.RemoveAt(columnIndex);
        _ColumnIndexes.Remove(columnName);

        for (int i = columnIndex; i < _ColumnNames.Count; i++)
            _ColumnIndexes[_ColumnNames[i]] = i;

        foreach (var row in _ValuesByRow)
            row.RemoveAt(columnIndex);

        return this;
    }

    #endregion

    #region Row Operations

    /// <summary>
    /// Add a row to the bottom of the table filled with null values
    /// </summary>
    public StringTable AddRow()
    {
        var emptyRow = Enumerable.Repeat<string?>(null, ColumnCount).ToList();
        _ValuesByRow.Add(emptyRow);
        return this;
    }

    /// <summary>
    /// Add a row to the bottom of the table filled with the given values.
    /// The length does not have to be exact: the table may be partially filled or expanded to fit the collection.
    /// </summary>
    public StringTable AddRow(IReadOnlyList<string?> values)
    {
        while (ColumnCount < values.Count)
        {
            AddColumn();
        }

        AddRow();

        int lastRowIndex = RowCount - 1;
        for (int i = 0; i < values.Count; i++)
        {
            this[lastRowIndex, i] = values[i];
        }

        return this;
    }

    /// <summary>
    /// Add a row to the bottom of the table filled with the given values.
    /// </summary>
    public StringTable AddRow(TableRow column)
    {
        AddRow(column.Values);
        return this;
    }

    /// <summary>
    /// Add rows to the bottom of the table filled with the given values.
    /// </summary>
    public StringTable AddRows(IEnumerable<TableRow> rows)
    {
        foreach (TableRow row in rows)
            AddRow(row.Values);

        return this;
    }

    /// <summary>
    /// Copy all rows from the given table to the right side of the table
    /// </summary>
    public StringTable AddRows(StringTable table)
    {
        AddRows(table.Rows);
        return this;
    }

    /// <summary>
    /// Delete the given row from the table
    /// </summary>
    public StringTable DeleteRow(TableRow row) => DeleteRow(row.RowIndex);

    /// <summary>
    /// Delete the given row (starting at 0) from the table
    /// </summary>
    public StringTable DeleteRow(int rowIndex)
    {
        _ValuesByRow.RemoveAt(rowIndex);
        return this;
    }

    #endregion

    #region Table Operations

    /// <summary>
    /// Delete all rows but preserve column names.
    /// </summary>
    public StringTable ClearRows()
    {
        _ValuesByRow.Clear();
        return this;
    }

    /// <summary>
    /// Delete all rows and columns to rest this table to an empty state.
    /// </summary>
    public StringTable Clear()
    {
        _ValuesByRow.Clear();
        _ColumnNames.Clear();
        _ColumnIndexes.Clear();
        return this;
    }

    /// <summary>
    /// Increase the size of the table as needed to ensure the given number of rows and columns are present.
    /// New cells will be populated with null values.
    /// Default column names will be used (A, B, C, ... AA, AB, AC, ... etc)
    /// </summary>
    public StringTable Expand(int rowCount, int columnCount)
    {
        while (ColumnCount < columnCount)
            AddColumn();

        while (RowCount < rowCount)
            AddRow();

        return this;
    }

    /// <summary>
    /// Rotate the entire table clockwise 90 degrees.
    /// This will lose column names (they will be reset to A, B, C, ...).
    /// Column names may be added after rotation with methods like 
    /// <see cref="SetColumnNames(IReadOnlyList{string?})"/> 
    /// and <see cref="SetColumnNamesFromFirstRow"/>.
    /// </summary>
    public StringTable Rotate90()
    {
        List<string?> values = GetValues();
        int initialRowCount = RowCount;
        int initialColumnCount = ColumnCount;
        int newRowCount = ColumnCount;
        int newColumnCount = RowCount;

        Clear();

        Expand(newRowCount, newColumnCount);

        for (int r = 0; r < newRowCount; r++)
        {
            for (int c = 0; c < newColumnCount; c++)
            {
                this[r, c] = values[(initialRowCount - 1 - c) * initialColumnCount + r];

            }
        }

        return this;
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Returns a string summarizing the name and dimensions of this table
    /// </summary>
    public override string ToString() => $"{Metadata.Title} {nameof(StringTable)} " +
        $"with {RowCount} rows and {ColumnCount} columns";

    /// <summary>
    /// Return the value of the cell at the given row and column index (starting at 0)
    /// </summary>
    public string? this[int row, int col]
    {
        get => _ValuesByRow[row][col];
        set => _ValuesByRow[row][col] = value;
    }

    /// <summary>
    /// Return the value of the cell at the given row (starting at 0) for the given column name
    /// </summary>
    public string? this[int row, string columnName]
    {
        get => this[row, GetNamedColumnIndex(columnName)];
        set => this[row, GetNamedColumnIndex(columnName)] = value;
    }

    /// <summary>
    /// Return the value of the cell at the given row and column index (starting at 0)
    /// </summary>
    public string? GetValue(int row, int column) => this[row, column];

    /// <summary>
    /// Return the value of the cell at the given row (starting at 0) for the given column name
    /// </summary>
    public string? GetValue(int row, string columnName) => this[row, columnName];

    /// <summary>
    /// Return a serialized collection of all values from the table.
    /// If by row is true, data will be returned as row1, row2, row3, etc.
    /// If by row is false, data will be returned as col1, col2, col3, etc.
    /// </summary>
    public List<string?> GetValues(bool byRow = true) => byRow
            ? Rows.SelectMany(r => r.Values ?? Enumerable.Empty<string?>()).ToList()
            : Columns.SelectMany(r => r.Values ?? Enumerable.Empty<string?>()).ToList(); // TODO: yield return?

    /// <summary>
    /// Set the value of the cell at the given row and column
    /// </summary>
    public StringTable SetValue(TableRow row, TableColumn column, string? value)
    {
        this[row.RowIndex, column.ColumnIndex] = value;
        return this;
    }

    /// <summary>
    /// Set the value of the cell at the given row (starting at 0) and column name
    /// </summary>
    public StringTable SetValue(int row, int column, string? value)
    {
        this[row, column] = value;
        return this;
    }

    /// <summary>
    /// Set the value of the cell at the given row and column (starting at 0)
    /// </summary>
    public StringTable SetValue(int row, string columnName, string? value)
    {
        this[row, columnName] = value;
        return this;
    }

    /// <summary>
    /// Returns a collection of all rows
    /// </summary>
    public List<TableRow> Rows => Enumerable.Range(0, RowCount).Select(GetRow).ToList(); // TODO: yield return?

    /// <summary>
    /// Returns a collection of all columns
    /// </summary>
    public List<TableColumn> Columns => Enumerable.Range(0, ColumnCount).Select(GetColumn).ToList(); // TODO: yield return?

    /// <summary>
    /// Returns the row at the given position (starting at 0)
    /// </summary>
    public TableRow GetRow(int rowIndex)
    {
        return new TableRow()
        {
            RowIndex = rowIndex,
            Values = GetRowValues(rowIndex),
        };
    }

    /// <summary>
    /// Returns the column at the given position (starting at 0)
    /// </summary>
    public TableColumn GetColumn(int columnIndex)
    {
        return new TableColumn()
        {
            ColumnIndex = columnIndex,
            ColumnName = _ColumnNames[columnIndex],
            Values = GetColumnValues(columnIndex),
        };
    }

    /// <summary>
    /// Return values for the given column (starting at 0)
    /// </summary>
    public List<string?> GetColumnValues(int columnIndex)
    {
        return Enumerable.Range(0, RowCount).Select(x => this[x, columnIndex]).ToList();
    }

    /// <summary>
    /// Return values for the given column
    /// </summary>
    public List<string?> GetColumnValues(string columnName)
    {
        return GetColumnValues(GetNamedColumnIndex(columnName));
    }

    /// <summary>
    /// Return values for the given row (starting at 0)
    /// </summary>
    public List<string?> GetRowValues(int rowIndex)
    {
        return Enumerable.Range(0, ColumnCount).Select(x => this[rowIndex, x]).ToList();
    }

    #endregion

    #region Column Names

    /// <summary>
    /// Set the name of the column at the given position (starting at 0)
    /// </summary>
    public StringTable SetColumnName(int columnIndex, string name)
    {
        string oldName = _ColumnNames[columnIndex];
        _ColumnNames[columnIndex] = name;
        _ColumnIndexes.Remove(oldName);
        _ColumnIndexes[name] = columnIndex;
        return this;
    }

    /// <summary>
    /// Set the names for all columns.
    /// The table may be expanded to accommodate the length of names.
    /// </summary>
    public StringTable SetColumnNames(IReadOnlyList<string?> names)
    {
        Expand(RowCount, names.Count);

        for (int i = 0; i < names.Count; i++)
        {
            SetColumnName(i, names[i] ?? DefaultColumnName(i));
        }

        return this;
    }

    /// <summary>
    /// <summary>Returns the zero-based index of a named column.</summary>
    /// </summary>
    public int GetNamedColumnIndex(string columnName)
    {
        if (!_ColumnIndexes.TryGetValue(columnName, out int index))
            throw new KeyNotFoundException($"No column named '{columnName}'.");

        return index;
    }

    /// <summary>
    /// Delete the first row from the table and apply its values as column names.
    /// Default column names (A, B, C, ...) will be used for cells with null values.
    /// </summary>
    public StringTable SetColumnNamesFromFirstRow()
    {
        SetColumnNames(GetRow(0).Values);
        DeleteRow(0);
        return this;
    }

    #endregion
}