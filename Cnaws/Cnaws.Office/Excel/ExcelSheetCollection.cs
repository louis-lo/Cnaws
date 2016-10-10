using System;
using System.Collections;
using E = Microsoft.Office.Interop.Excel;

namespace Cnaws.Office.Excel
{
    public sealed class ExcelSheetCollection : ICollection
    {
        private E.Workbook _book;

        internal ExcelSheetCollection(E.Workbook book)
        {
            _book = book;
        }

        public ExcelSheet this[int index]
        {
            get { return new ExcelSheet((E.Worksheet)_book.Sheets.Item[index + 1]); }
        }
        public ExcelSheet this[string name]
        {
            get { return new ExcelSheet((E.Worksheet)_book.Sheets.Item[name]); }
        }

        public int Count
        {
            get { return _book.Sheets.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public void CopyTo(Array array, int index)
        {
            int count = Count;
            ExcelSheet[] sheets = new ExcelSheet[count];
            for (int i = 0; i < count; ++i)
                sheets[i] = new ExcelSheet((E.Worksheet)_book.Sheets[i]);
            Array.Copy(sheets, 0, array, index, count);
        }

        public IEnumerator GetEnumerator()
        {
            return new ExcelSheetCollectionEnumerator(this);
        }

        private sealed class ExcelSheetCollectionEnumerator : IEnumerator
        {
            private int _index;
            private ExcelSheetCollection _parent;

            internal ExcelSheetCollectionEnumerator(ExcelSheetCollection parent)
            {
                _index = -1;
                _parent = parent;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return _parent[_index];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public bool MoveNext()
            {
                return (++_index < _parent.Count);
            }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
