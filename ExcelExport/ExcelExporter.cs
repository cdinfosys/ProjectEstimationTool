using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ProjectEstimationTool.ExcelExport
{
    class ExcelExporter : IDisposable
    {
        #region Type fields
        /// <summary>
        /// Object used by <see cref="Dispose(bool)"/> to synchronise thread access 
        /// </summary>
        private static Object sDisposeSynchLock = new Object();
        #endregion // Type fields

        #region Private fields
        /// <summary>
        /// Spreadsheet document object.
        /// </summary>
        private SpreadsheetDocument mSpreadsheetDocument;

        /// <summary>
        /// Workbook.
        /// </summary>
        private WorkbookPart mWorkbookPart;

        /// <summary>
        /// Shared strings
        /// </summary>
        private SharedStringTablePart mSharedStringTablePart;

        /// <summary>
        /// Worksheet
        /// </summary>
        private WorksheetPart mWorksheetPart;

        /// <summary>
        /// Workbook styles
        /// </summary>
        private WorkbookStylesPart mWorkbookStylesPart;

        /// <summary>
        /// Workbook sheets.
        /// </summary>
        private Sheets mSheets;

        /// <summary>
        /// Sheet object.
        /// </summary>
        private Sheet mSheet;

        /// <summary>
        /// Flag for the <see cref="Dispose(bool)"/> method to indicate that it must discard the spreadsheet.
        /// </summary>
        private Boolean mAbandon = false;
        #endregion // Private fields

        #region Construction
        /// <summary>
        /// Construct an object to write spreadsheet documents.
        /// </summary>
        /// <param name="path">
        /// Path to the output file.
        /// </param>
        public ExcelExporter(String path)
        {
            try
            {
                mSpreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);

                mWorkbookPart = mSpreadsheetDocument.AddWorkbookPart();
                mWorkbookPart.Workbook = new Workbook();

                mSharedStringTablePart = mWorkbookPart.AddNewPart<SharedStringTablePart>();
                mSharedStringTablePart.SharedStringTable = new SharedStringTable();

                mWorkbookStylesPart = mWorkbookPart.AddNewPart<WorkbookStylesPart>();
                mWorkbookStylesPart.Stylesheet = new Stylesheet();

                mWorksheetPart = mWorkbookPart.AddNewPart<WorksheetPart>();
                mWorksheetPart.Worksheet = new Worksheet(new SheetData());
                mSheets = mWorkbookPart.Workbook.AppendChild(new Sheets());
                mSheet = new Sheet()
                {
                    Id = mWorkbookPart.GetIdOfPart(mWorksheetPart),
                    SheetId = 1,
                    Name = ProjectEstimationTool.Properties.Resources.ExcelSheetName,
                    State = SheetStateValues.Visible
                };
                mSheets.Append(mSheet);
            }
            catch
            {
                this.mAbandon = true;
                throw;
            }
        }
        #endregion Construction

        #region Finalizer
        ~ExcelExporter()
        {
            Dispose(false);
        }
        #endregion //Finalizer

        #region IDisposable interface
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Helper for the <see cref="Dispose"/> method
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> if calling from the <see cref="Dispose"/> method. <c>false</c> if calling from the finalizer.
        /// </param>
        protected virtual void Dispose(Boolean disposing)
        {
            try
            {
                if (disposing)
                {
                    Monitor.Enter(sDisposeSynchLock);
                }

                if (mSpreadsheetDocument != null)
                {
                    if (!this.mAbandon)
                    {
                        mSpreadsheetDocument.Save();
                    }
                    mSpreadsheetDocument.Dispose();
                    mSpreadsheetDocument = null;
                }
            }
            finally
            {
                if (disposing)
                {
                    Monitor.Exit(sDisposeSynchLock);
                }
            }
        }
        #endregion // IDisposable interface
    } // class ExcelExporter
} // namespace ProjectEstimationTool.ExcelExport
