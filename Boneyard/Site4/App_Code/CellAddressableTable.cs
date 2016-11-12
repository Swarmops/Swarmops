using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Reflection;

namespace CellAddressable
{
    public struct CellCoordinate
    {
        public int row;
        public int column;
        public CellCoordinate (int r, int c)
        {
            row = r;
            column = c;
        }

        public bool IsEmpty ()
        {
            return (row < 0 || column < 0);
        }
    }

    public class CellAddressableTable
    {
        CellCollection _cells = null;
        public CellAddressableTable ()
        {
            _cells = new CellCollection(this);
        }

        internal CellCollection _Cells
        {
            get
            {
                return _cells;
            }
        }


        public CellAdressableCell this[int row, int column]
        {
            get
            {
                return _cells[row, column];
            }
            private set
            {
                _cells[row, column] = value;
            }
        }

        public ColumnAccessor Columns
        {
            get
            {
                return new ColumnAccessor(this);
            }
            set
            {
            }
        }
        public RowAccessor Rows
        {
            get
            {
                return new RowAccessor(this);
            }
            set
            {
            }
        }
        public CellCoordinate FindCell (CellAdressableCell cell)
        {
            return _cells.FindCell(cell);
        }

        public void InsertColumnAt (int newColNumber)
        {
            _cells.InsertColumnAt(newColNumber);
        }

        public void InsertRowAt (int newRowNumber)
        {
            _cells.InsertRowAt(newRowNumber);
        }

        public void GetHTMLTable (ref HtmlTable tab)
        {
            GetHTMLTable(ref  tab, false);
        }

        public void GetHTMLTable (ref HtmlTable tab, bool merge)
        {
            SetUpSpans();

            for (int r = 0; r < _cells.RowCount; ++r)
            {
                HtmlTableRow row = null;
                if (merge == false || (tab.Rows.Count - 1 < r))
                {
                    row = new HtmlTableRow();
                    tab.Rows.Add(row);
                }
                else
                {
                    row = tab.Rows[r];
                }
                row.Cells.Clear();
                for (int c = 0; c < _cells.ColCount; ++c)
                {
                    CellAdressableCell cell = _cells[r, c];
                    if ((cell.SkippedCell) == false)
                    {
                        if (cell.Cell.InnerHtml == "")
                            cell.Cell.InnerHtml = "&nbsp;";
                        row.Cells.Add(cell.Cell);
                    }
                }
            }
        }

        internal void SetUpSpans ()
        {
            // set up skipped by row or columnspan
            for (int r = 0; r < _cells.RowCount; ++r)
            {
                int c = 0;
                while (c < _cells.ColCount)
                {
                    CellAdressableCell cell = _cells[r, c];
                    int colSpan = 1;
                    if (cell.Cell.ColSpan > 1)
                        colSpan = cell.Cell.ColSpan;
                    for (int c2 = 1; c2 <= colSpan; ++c2)
                    {
                        if (c2 > 1)
                        {
                            CellAdressableCell cell2 = _cells[r, c + c2 - 1];
                            if (cell2.Cell.ColSpan > 1)
                            {
                                if (cell2.Cell.ColSpan + c2 - 1 > colSpan)
                                {
                                    colSpan = cell2.Cell.ColSpan + c2 - 1;
                                    cell.Cell.ColSpan = colSpan;
                                }
                            }
                            cell2.Cell.ColSpan = 0;
                            cell2.SkippedByColspan = true;
                        }
                        int rowSpan = 1;
                        if (cell.Cell.RowSpan > 1)
                            rowSpan = cell.Cell.RowSpan;
                        for (int r2 = 1; r2 <= rowSpan; ++r2)
                        {
                            if (r2 > 1)
                            {
                                CellAdressableCell cell2 = _cells[r + r2 - 1, c + c2 - 1];
                                if (cell2.Cell.RowSpan > 1)
                                {
                                    if (cell2.Cell.RowSpan + r2 - 1 > rowSpan)
                                    {
                                        rowSpan = cell2.Cell.RowSpan + r2 - 1;
                                        cell.Cell.RowSpan = rowSpan;
                                    }
                                }
                                cell2.Cell.RowSpan = 0;
                                cell2.SkippedByRowspan = true;
                            }
                        }
                    }
                    c = c + colSpan;
                }
            }
        }

        public void LoadTable (HtmlTable table)
        {
            int currRow = 0;
            foreach (HtmlTableRow r in table.Rows)
            {
                int currCol = 0;
                foreach (HtmlTableCell c in r.Cells)
                {
                    while (this[currRow, currCol].SkippedCell)
                        currCol++;
                    this[currRow, currCol] = new CellAdressableCell(this, c);
                    currCol += c.ColSpan > 0 ? c.ColSpan : 1;
                }
                this.SetUpSpans();
                ++currRow;
            }
        }

        public IEnumerable<CellAdressableCell> Cells
        {
            get
            {
                CellAdressableCell currentCell = this[0, 0];
                for (int r = 0; r < this._Cells.RowCount; ++r)
                {
                    for (int c = 0; c < this._Cells.ColCount; ++c)
                    {
                        currentCell = this[r, c];
                        yield return currentCell;
                        if (currentCell != this[r, c])
                        {
                            CellCoordinate myCoord = this.FindCell(currentCell);
                            r = myCoord.row;
                            c = myCoord.column;
                        }
                    }
                }
            }
        }
    }


    public class CellCollection
    {
        Dictionary<int, Dictionary<int, CellAdressableCell>> _cells = new Dictionary<int, Dictionary<int, CellAdressableCell>>();

        internal CellCollection (CellAddressableTable parent)
        {
            parentTable = parent;
        }
        CellAddressableTable _parentTable;
        internal CellAddressableTable parentTable
        {
            get { return _parentTable; }
            private set { _parentTable = value; }
        }
        int _RowCount;
        public int RowCount
        {
            get { return _RowCount; }
            protected set { _RowCount = value; }
        }

        int _ColCount = 0;
        public int ColCount
        {
            get { return _ColCount; }
            protected set { _ColCount = value; }
        }

        internal CellAdressableCell this[int row, int column]
        {
            get
            {
                if (_cells.ContainsKey(row))
                    if (_cells[row].ContainsKey(column))
                        return _cells[row][column];
                if (column >= ColCount)
                    ColCount = column + 1;
                if (row >= RowCount)
                    RowCount = row + 1;
                this[row, column] = new CellAdressableCell(parentTable);
                return this[row, column];
            }
            set
            {
                if (!_cells.ContainsKey(row))
                    _cells.Add(row, new Dictionary<int, CellAdressableCell>());

                if (!_cells[row].ContainsKey(column))
                    _cells[row].Add(column, value);
                else
                    _cells[row][column] = value;
                if (column >= ColCount)
                    ColCount = column + 1;
                if (row >= RowCount)
                    RowCount = row + 1;
            }
        }

        public bool CellExists (int row, int column)
        {
            if (_cells.ContainsKey(row))
                if (_cells[row].ContainsKey(column))
                    return true;
            return false;

        }

        internal CellCoordinate FindCell (CellAdressableCell cell)
        {
            foreach (int r in _cells.Keys)
                foreach (int c in _cells[r].Keys)
                {
                    if (cell == _cells[r][c])
                    {
                        return new CellCoordinate(r, c);
                    }
                }
            return new CellCoordinate(-1, -1);
        }

        internal void InsertRowAt (int newRowNumber)
        {

            parentTable.SetUpSpans();

            if (newRowNumber <= RowCount)
                RowCount++;

            for (int i = this.RowCount - 1; i > -1 && i >= newRowNumber; --i)
            {
                if (_cells.ContainsKey(i))
                {
                    Dictionary<int,CellAdressableCell> movedRow = _cells[i];
                    _cells.Remove(i);
                    _cells.Add(i + 1, movedRow);

                }
            }

            if (newRowNumber < this.RowCount - 1)
            {
                Row oldRow = parentTable.Rows[newRowNumber + 1];
                Row newRow = parentTable.Rows[newRowNumber];
                for (int i = 0; i < this.ColCount - 1; ++i)
                {
                    if (oldRow[i].SkippedByRowspan)
                    {
                        newRow[i].JoinCell(CellJoinDirection.UP);
                        //newRow[i].JoinCell(CellJoinDirection.DOWN);
                    }
                }
            }
        }

        internal void InsertColumnAt (int newColNumber)
        {
            parentTable.SetUpSpans();
            foreach (int rowNo in _cells.Keys)
            {
                int maxCol = -1;
                foreach (int i in _cells[rowNo].Keys)
                    if (i > maxCol)
                        maxCol = i;
                for (int i = maxCol; i > -1 && i >= newColNumber; --i)
                {
                    if (_cells[rowNo].ContainsKey(i))
                    {
                        CellAdressableCell  movedCell = _cells[rowNo][i];
                        _cells[rowNo].Remove(i);
                        this[rowNo, i + 1] = movedCell;
                    }
                }
            }

            if (newColNumber < this.ColCount - 1)
            {
                Column oldCol = parentTable.Columns[newColNumber + 1];
                Column newCol = parentTable.Columns[newColNumber];
                for (int i = 0; i < this.RowCount - 1; ++i)
                {
                    if (oldCol[i].SkippedByColspan)
                    {
                        newCol[i].JoinCell(CellJoinDirection.LEFT);
                        //newCol[i].JoinCell(CellJoinDirection.RIGHT);
                    }
                }
            }

        }
    }

    public class CellAdressableCell
    {
        bool skippedByRowspan = false;
        bool skippedByColspan = false;
        HtmlTableCell _Cell;
        public HtmlTableCell Cell
        {
            get{return _Cell;}
            private set{_Cell=value;}
        }

        internal CellAdressableCell (CellAddressableTable parent)
            : base()
        {
            parentTable = parent;
            Cell = new HtmlTableCell();
        }

        internal CellAdressableCell (CellAddressableTable parent, HtmlTableCell c)
            : base()
        {
            parentTable = parent;
            Cell = c;

        }

        internal bool SkippedCell
        {
            get
            {
                return skippedByColspan || skippedByRowspan;
            }
        }
        CellAddressableTable _parentTable;
        internal CellAddressableTable parentTable
        {
            get { return _parentTable; }
            private set { _parentTable = value; }
        }

        internal bool SkippedByRowspan
        {
            get
            {
                return this.skippedByRowspan;
            }
            set
            {
                this.skippedByRowspan = value;
            }
        }

        internal bool SkippedByColspan
        {
            get
            {
                return this.skippedByColspan;
            }
            set
            {
                this.skippedByColspan = value;
            }
        }
        public CellAdressableCell JoinCell (CellJoinDirection direction)
        {

            CellCoordinate myCoord = parentTable.FindCell(this);
            CellAdressableCell otherCell = null;
            try
            {
                switch (direction)
                {
                    case CellJoinDirection.DOWN:
                        otherCell = parentTable[myCoord.row + 1, myCoord.column];
                        if (this.Cell.RowSpan < 2)
                            this.Cell.RowSpan = 2;
                        else
                            this.Cell.RowSpan++;

                        break;
                    case CellJoinDirection.LEFT:
                        otherCell = parentTable[myCoord.row, myCoord.column - 1];
                        if (otherCell.Cell.ColSpan < 2)
                            otherCell.Cell.ColSpan = 2;
                        else
                            otherCell.Cell.ColSpan++;

                        break;
                    case CellJoinDirection.RIGHT:
                        otherCell = parentTable[myCoord.row, myCoord.column + 1];
                        if (this.Cell.ColSpan < 2)
                            this.Cell.ColSpan = 2;
                        else
                            this.Cell.ColSpan++;

                        break;
                    case CellJoinDirection.UP:
                        otherCell = parentTable[myCoord.row - 1, myCoord.column];
                        if (otherCell.Cell.RowSpan < 2)
                            otherCell.Cell.RowSpan = 2;
                        else
                            otherCell.Cell.RowSpan++;

                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Cant Join Cell in that direction.", e);
            }
            return otherCell;
        }

        public  CellAdressableCell AddClass ( string Class)
        {
            if (this.Cell.Attributes["class"] == null)
                this.Cell.Attributes["class"] = " " + Class.Trim() + " ";
            else
            {
                if (!this.Cell.Attributes["class"].Contains(" " + Class.Trim() + " "))
                    this.Cell.Attributes["class"] += " " + Class.Trim() + " ";
                this.Cell.Attributes["class"] = this.Cell.Attributes["class"].Replace("  ", " ");
            }
            return this;
        }

    }

    public class ColumnAccessor
    {
        internal ColumnAccessor (CellAddressableTable parent)
        {
            parentTable = parent;
        }

        CellAddressableTable _parentTable;
        public CellAddressableTable parentTable
        {
            get { return _parentTable; }
            private set { _parentTable = value; }
        }

        public int Count
        {
            get
            {
                return parentTable._Cells.ColCount;
            }
        }

        public Column this[int colNo]
        {
            get
            {
                return new Column(parentTable, colNo);
            }

        }
    }

    public class Column
    {
        CellAddressableTable _parent;
        private CellAddressableTable parent
        {
            get { return _parent; }
             set { _parent = value; }
        }
        CellAdressableCell _firstCell;
        private CellAdressableCell firstCell
        {
            get { return _firstCell; }
            set { _firstCell = value; }
        }

        int _column;
        private int column
        {
            get { return _column; }
            set { _column = value; }
        }

        public CellAdressableCell this[int cellNo]
        {
            get
            {
                if (this.firstCell != this.parent[0, column])
                {
                    CellCoordinate myCoord = parent.FindCell(this.firstCell);
                    this.column = myCoord.column;
                }
                return this.parent[cellNo, this.column];
            }
        }

        internal Column (CellAddressableTable parentTable, int colNo)
        {

            parent = parentTable;
            column = colNo;
            firstCell = this.parent[0, this.column];
        }

        public IEnumerable<CellAdressableCell> Cells
        {
            get
            {
                CellAdressableCell currentCell = null;
                for (int i = 0; i < parent._Cells.RowCount; ++i)
                {
                    currentCell = this[i];
                    yield return currentCell;
                    if (currentCell != this[i])
                    {
                        CellCoordinate myCoord = parent.FindCell(currentCell);
                        i = myCoord.row;
                    }
                }
            }
        }
    }


    public class RowAccessor
    {
        internal RowAccessor (CellAddressableTable parent)
        {
            parentTable = parent;
        }

        CellAddressableTable _parentTable;
        public CellAddressableTable parentTable
        {
            get { return _parentTable; }
            set { _parentTable = value; }
        }

        public int Count
        {
            get
            {
                return parentTable._Cells.RowCount;
            }
        }
        public Row this[int rowNo]
        {
            get
            {
                return new Row(parentTable, rowNo);
            }

        }
    }

    public class Row
    {
        CellAddressableTable _parent;
        private CellAddressableTable parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        int _row;
        private int row
        {
            get { return _row; }
            set { _row = value; }
        }

        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        CellAdressableCell _firstCell;
        private CellAdressableCell firstCell
        {
            get { return _firstCell; }
            set { _firstCell = value; }

        }

        public CellAdressableCell this[int cellNo]
        {
            get
            {
                if (this.firstCell != this.parent[row, 0])
                {
                    CellCoordinate myCoord = parent.FindCell(this.firstCell);
                    this.row = myCoord.row;
                }

                return this.parent[this.row, cellNo];
            }
        }

        internal Row (CellAddressableTable parentTable, int rowNo)
        {
            parent = parentTable;
            row = rowNo;
            firstCell = this.parent[this.row, 0];
        }

        public IEnumerable<CellAdressableCell> Cells
        {
            get
            {
                CellAdressableCell currentCell = null;
                for (int i = 0; i < parent._Cells.ColCount; ++i)
                {
                    currentCell = this[i];
                    yield return currentCell;
                    if (currentCell != this[i])
                    {
                        CellCoordinate myCoord = parent.FindCell(currentCell);
                        i = myCoord.column;
                    }
                }
            }
        }
    }

    public enum CellJoinDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

}