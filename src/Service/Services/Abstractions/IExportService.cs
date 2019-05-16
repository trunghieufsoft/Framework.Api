using System.Collections.Generic;
using Common.DTOs.Export;

namespace Service.Services.Abstractions
{
    public interface IExportService
    {
        string ExportExcel(ExcelSheet data);
        string ExportExcel(List<ExcelSheet> datas);
    }
}