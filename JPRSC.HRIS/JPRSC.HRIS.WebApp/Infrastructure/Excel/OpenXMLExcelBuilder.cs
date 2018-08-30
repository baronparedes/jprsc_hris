using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JPRSC.HRIS.WebApp.Infrastructure.Excel
{
    public interface IExcelBuilder
    {
        void BuildExcelFile(string exportPath, ExcelObject excelObject);
        byte[] BuildExcelFile(ExcelObject excelObject);
        byte[] BuildExcelFile(IEnumerable<IEnumerable<string>> lines);
    }

    public class OpenXMLExcelBuilder : IExcelBuilder
    {
        public byte[] BuildExcelFile(ExcelObject excelObject)
        {
            using (var ms = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    BuildDocument(excelObject.Header, excelObject.Rows, document);
                }

                return ms.ToArray();
            }
        }

        public void BuildExcelFile(string exportPath, ExcelObject excelObject)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(exportPath, SpreadsheetDocumentType.Workbook))
            {
                BuildDocument(excelObject.Header, excelObject.Rows, document);
            }
        }

        public byte[] BuildExcelFile(IEnumerable<IEnumerable<string>> lines)
        {
            using (var ms = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    BuildDocument(lines, document);
                }

                return ms.ToArray();
            }
        }

        private static void BuildDocument(IList<string> header, IList<IList<string>> rows, SpreadsheetDocument document)
        {
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet 1" };

            sheets.Append(sheet);

            workbookPart.Workbook.Save();

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            var row = new Row();

            row.Append(header.Select(h => new Cell
            {
                CellValue = new CellValue(h),
                DataType = new EnumValue<CellValues>(CellValues.String)
            }));

            sheetData.AppendChild(row);

            foreach (var contentRow in rows)
            {
                row = new Row();

                row.Append(contentRow.Select(cr => new Cell
                {
                    CellValue = new CellValue(cr),
                    DataType = int.TryParse(cr, out int val) ? new EnumValue<CellValues>(CellValues.Number) : new EnumValue<CellValues>(CellValues.String)
                }));

                sheetData.AppendChild(row);
            }

            workbookPart.Workbook.Save();
        }

        private static void BuildDocument(IEnumerable<IEnumerable<string>> lines, SpreadsheetDocument document)
        {
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet 1" };

            sheets.Append(sheet);

            workbookPart.Workbook.Save();

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            var row = new Row();

            foreach (var line in lines)
            {
                row = new Row();

                row.Append(line.Select(cr => new Cell
                {
                    CellValue = new CellValue(cr),
                    DataType =
                        //int.TryParse(cr?.Replace(",", ""), out int intVal) || double.TryParse(cr?.Replace(",", ""), out double doubleVal) || decimal.TryParse(cr?.Replace(",", ""), out decimal decimalVal) ?
                        int.TryParse(cr?.Replace(",", ""), out int intVal) ?
                        new EnumValue<CellValues>(CellValues.Number) :
                        new EnumValue<CellValues>(CellValues.String)
                }));

                sheetData.AppendChild(row);
            }

            workbookPart.Workbook.Save();
        }
    }
}