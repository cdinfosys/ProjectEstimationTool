using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ProjectEstimationTool.ExcelExport
{
    class ExcelExporter : IDisposable
    {
        #region Private fields
        /// <summary>
        /// Spreadsheet document object.
        /// </summary>
        private SpreadsheetDocument mSpreadsheetDocument;

        /// <summary>
        /// Workbook.
        /// </summary>
        private WorkbookPart mWorkbookPart;
        #endregion // Private fields

        /// <summary>
        /// Worksheet
        /// </summary>
        private WorksheetPart mWorksheetPart;

        /// <summary>
        /// Workbook sheets.
        /// </summary>
        private Sheets mSheets;

        /// <summary>
        /// Sheet object.
        /// </summary>
        private Sheet mSheet;

        #region Construction
        /// <summary>
        /// Construct an object to write spreadsheet documents.
        /// </summary>
        /// <param name="path">
        /// Path to the output file.
        /// </param>
        public ExcelExporter(String path)
        {
            mSpreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
            mWorkbookPart = mSpreadsheetDocument.AddWorkbookPart();
            mWorkbookPart.Workbook = new Workbook();
            mWorksheetPart = mWorkbookPart.AddNewPart<WorksheetPart>();
            mWorksheetPart.Worksheet = new Worksheet(new SheetData());
            mSheets = mWorkbookPart.Workbook.AppendChild(new Sheets());
            mSheet = new Sheet()
            {
                Id = mWorkbookPart.GetIdOfPart(mWorksheetPart),
                SheetId = 1,
                Name = "Project Plan",
                State = SheetStateValues.Visible
            };
            mSheets.Append(mSheet);
        }
        #endregion Construction

        #region IDisposable interface
        public void Dispose()
        {
            if (mSpreadsheetDocument != null)
            {
                mSpreadsheetDocument.Save();
                mSpreadsheetDocument.Dispose();
                mSpreadsheetDocument = null;
            }
        }
        #endregion // IDisposable interface
    } // class ExcelExporter
} // namespace ProjectEstimationTool.ExcelExport
