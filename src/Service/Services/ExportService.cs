using System;
using Serilog;
using OfficeOpenXml;
using Common.DTOs.Export;
using Common.DTOs.Report;
using System.Collections.Generic;
using Service.Services.Abstractions;

namespace Service.Services
{
    public class ExportService : BaseService, IExportService
    {
        public string ExportExcel(ExcelSheet data)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add(data.SheetName);
                WriteDataToWorksheet(ref workSheet, data);
                return Convert.ToBase64String(excel.GetAsByteArray());
            }
        }

        public string ExportExcel(List<ExcelSheet> data)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                for (int i = 0; i < data.Count; i++)
                {
                    ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add(data[i].SheetName);
                    WriteDataToWorksheet(ref workSheet, data[i]);
                }
                return Convert.ToBase64String(excel.GetAsByteArray());
            }
        }

        private void WriteDataToWorksheet(ref ExcelWorksheet workSheet, ExcelSheet data)
        {
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            // Create group header
            int rowIndex = 1;
            if (data.HasGroup)
            {
                int fromCol = 1, toRow = 1, toCol = 0;
                for (int i = 0; i < data.Columns.Length; i++)
                {
                    var group = data.Columns[i];
                    toCol += group.Header.Length;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Merge = true;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Value = group.Group;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, fromCol, toRow, toCol].AutoFitColumns();
                    fromCol = toCol + 1;
                }
                rowIndex = rowIndex + 1;
            }
            // Create header
            var colIndex = 0;
            var properties = new List<PropertyIndex>();
            for (int i = 0; i < data.Columns.Length; i++)
            {
                var columns = data.Columns[i].Header;
                var props = data.Columns[i].Property;
                for (int j = 0; j < columns.Length; j++)
                {
                    colIndex = colIndex + 1;
                    WriteCell(ref workSheet, rowIndex, colIndex, columns[j], true);
                    properties.Add(new PropertyIndex { Property = props[j], Index = colIndex });
                }
            }
            //Create Row
            if (data.ListData != null)
            {
                if (data.ListData is SearchingCriteria)
                {
                    ExportSearchingCriteria(ref workSheet, properties, data);
                }
                else
                {
                    foreach (dynamic dataObject in data.ListData)
                    {
                        rowIndex = rowIndex + 1;                        
                        foreach (var prop in properties)
                        {
                            dynamic valueItem = string.Empty;
                            #region for Exception Export
                            if (dataObject is ReportExeptionOutput)
                            {
                                if (prop.Property == "Export")
                                {
                                    valueItem = dataObject.Export + "";
                                }
                                else
                                {
                                    foreach (var exp in dataObject.Exception)
                                    {
                                        if (exp.ExceptionProp == prop.Property)
                                        {
                                            valueItem = exp.Quantity + "";
                                        }
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                               
                                try
                                {
                                    valueItem = dataObject.GetType().GetProperty(prop.Property).GetValue(dataObject);
                                    if (valueItem is DateTime)
                                    {
                                        valueItem = valueItem != null ? valueItem.ToString("ddd, dd/MM/yyyy") : string.Empty;
                                    }
                                    if (prop.Property == "Week")
                                    {
                                        valueItem = valueItem != null ? WeekString(valueItem) : string.Empty;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error("Get Value To Excel Error {e}", e);
                                }
                            }
                            WriteCell(ref workSheet, rowIndex, prop.Index, valueItem);
                        }
                    }
                }
            }
        }

        private void ExportSearchingCriteria(ref ExcelWorksheet workSheet, List<PropertyIndex> properties, ExcelSheet data)
        {
            var obj = (SearchingCriteria)data.ListData;
            foreach (var prop in properties)
            {
                var valueItem = obj.GetType().GetProperty(prop.Property).GetValue(obj);
                if ("From".Equals(prop.Property) || "To".Equals(prop.Property))
                {
                    var date = valueItem != null ? ((DateTime)valueItem).ToString("ddd, dd/MM/yyyy") : string.Empty;
                    WriteCell(ref workSheet, 2, prop.Index, date);
                    for (int i = 1; i < obj.RowNumber; i++)
                    {
                        WriteCell(ref workSheet, 2 + i, prop.Index, string.Empty);
                    }
                }
                else if (valueItem is List<string>)
                {
                    var arr = (List<string>)valueItem;
                    for (int i = 0; i < arr.Count; i++)
                    {
                        WriteCell(ref workSheet, 2 + i, prop.Index, arr[i]);
                    }
                    for (int i = arr.Count; i < obj.RowNumber; i++)
                    {
                        WriteCell(ref workSheet, 2 + i, prop.Index, string.Empty);
                    }
                }
            }
        }

        private void WriteCell(ref ExcelWorksheet workSheet, int row, int col, object value, bool bold = false)
        {
            workSheet.Cells[row, col].Value = value != null ? value : "";
            workSheet.Cells[row, col].AutoFitColumns();
            workSheet.Cells[row, col].Style.Font.Bold = bold;
            workSheet.Cells[row, col].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            workSheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            workSheet.Cells[row, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            workSheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        private string WeekString(string week)
        {
            var res = "";
            if (!string.IsNullOrEmpty(week))
            {
                var weekSplit = week.Split(",");
                var w = Int32.Parse(weekSplit[0]);
                var y = Int32.Parse(weekSplit[1]);
                DateTime startOfYear = new DateTime(y, 1, 1);
                var d = (w - 1) * 7 - 1;
                var start = startOfYear.AddDays(d - 1);
                var end = startOfYear.AddDays(d + 5);
                res = "Week " + w + ", " + start.ToString("dd/MM/yyyy") + "-" + end.ToString("dd/MM/yyyy");
            }
            return res;
        }
    }
}