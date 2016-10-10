using System;
using System.IO;
using E = Microsoft.Office.Interop.Excel;

namespace Cnaws.Office.Excel
{
    public sealed class ExcelDocument : IDisposable
    {
        private E.Application _application;
        private E.Workbook _book;
        private bool disposed = false;

        public ExcelDocument(string filename)
        {
            _application = new E.ApplicationClass();
            if (File.Exists(filename))
            {
                _book = _application.Workbooks.Open(filename, 0, false, 5, "", "", false, E.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            }
            else
            {
                _book = _application.Workbooks.Add(Type.Missing);
                SaveAs(filename);
            }
        }

        public ExcelSheetCollection Sheets
        {
            get { return new ExcelSheetCollection(_book); }
        }

        public void Save()
        {
            _book.Save();
        }
        public void SaveAs(string filename)
        {
            _book.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, E.XlSaveAsAccessMode.xlShared, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_application != null)
                    {
                        _application.Quit();
                        _application = null;
                    }
                }
                disposed = true;
            }
        }
        ~ExcelDocument()
        {
            Dispose(false);
        }
    }
}
