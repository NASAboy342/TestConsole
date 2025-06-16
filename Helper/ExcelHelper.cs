using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Helper
{
    public class ExcelHelper
    {
        public static DataTable ReadExcelToDataTable(string filePath, string sheetName)
        {
            using (var workBook = new XLWorkbook(filePath))
            {
                var workSheet = workBook.Worksheet(sheetName);

                DataTable dt = new DataTable();

                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    if (firstRow)
                    {
                        // Use the first row to add columns to the DataTable
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Add rows to the DataTable
                        dt.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }

                return dt;
            }
        }

        public static void WriteDataTableToExcel(DataTable dataTable, string filePath, string sheetName = "sheet1")
        {
            using (var workBook = new XLWorkbook())
            {
                var workSheet = workBook.Worksheets.Add(sheetName);
                // Add column names
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    workSheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
                }
                // Add rows
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        var value = dataTable.Rows[i][j];
                        if (value is int)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (int)value;
                        }
                        else if (value is long)
                        {
                            workSheet.Cell(j + 2, j + 1).Value = Convert.ToString(value);
                        }
                        else if (value is float)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (float)value;
                        }
                        else if (value is double)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (double)value;
                        }
                        else if (value is decimal)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (decimal)value;
                        }
                        else if (value is DateTime)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (DateTime)value;
                        }
                        else if (value is bool)
                        {
                            workSheet.Cell(i + 2, j + 1).Value = (bool)value;
                        }
                        else
                        {
                            workSheet.Cell(i + 2, j + 1).Value = value.ToString();
                        }
                    }
                }
                // Optionally, auto-adjust column widths
                workSheet.Columns().AdjustToContents();
                // Optionally, set the first row as header
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;
                workSheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                workSheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                workSheet.Row(1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                workSheet.Row(1).Style.Border.BottomBorderColor = XLColor.Black;
                workSheet.Row(1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                workSheet.Row(1).Style.Border.TopBorderColor = XLColor.Black;


                // Save the workbook
                workBook.SaveAs(filePath);
            }
        }
    }
}
