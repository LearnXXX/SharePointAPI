using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class OpenXmlTest
    {
        public static void Test()
        {
            using (var auditLogReport = File.OpenRead(@"C:\Users\xluo\Desktop\Run_a_custom_report_2019-11-19T174301.xlsx"))
            using (SpreadsheetDocument xlPackage = SpreadsheetDocument.Open(auditLogReport, false))
            {
                WorkbookPart workbook = xlPackage.WorkbookPart;

                SharedStringTable stringTable = xlPackage.WorkbookPart.SharedStringTablePart.SharedStringTable;

                IEnumerable<Sheet> sheets = workbook.Workbook.Descendants<Sheet>().ToList();
                foreach (var sheet in sheets)
                {
                    WorksheetPart worksheetPart = workbook.GetPartById(sheet.Id) as WorksheetPart;
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();
                    int rowCount = 0;
                    foreach (Row row in rows)
                    {
                        if (rowCount >= 2)
                        {
                            try
                            {
                               GetAuditData(stringTable, row);
                            }
                            catch (Exception e)
                            {
                                //logger.Error("Error occurred when get audit data from audit report. Exception:{0}", e.ToString());
                            }
                        }
                        rowCount++;
                    }
                }
            }
        }

        private static string GetReportCellValue(Cell cell, SharedStringTable stringTable)
        {
            //由于Excel的数据存储在SharedStringTable中，需要获取数据在SharedStringTable 中的索引
            string value = string.Empty;
            try
            {
                if (cell.ChildElements.Count == 0)
                    return value;

                value = double.Parse(cell.CellValue.InnerText).ToString();

                if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                {
                    value = stringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
            }
            catch (Exception e)
            {
                value = "N/A";
            }
            return value;

        }
        private static void GetAuditData(SharedStringTable stringTable, Row row)
        {

            int i = 0;
            foreach (Cell cell in row)
            {
                string value = GetReportCellValue(cell, stringTable);

                #region get audit data

                //Site Id
                //Item Id
                //Item Type
                //User Id
                //Document Location
                //Occurred (GMT)
                //Event
                //Custom Event Name
                //Event Source
                //Source Name
                //Event Data
                //App Id
                switch (i)
                {
                    case 0:
                        //auditData.SiteId = value.TrimStart('{').TrimEnd('}');
                        break;
                    case 1:
                        //auditData.ItemId = value.TrimStart('{').TrimEnd('}');
                        break;
                    case 2:
                        //auditData.ItemType = value;
                        break;
                    case 3:
                        //auditData.UserId = value;
                        break;
                    case 4:
                        //auditData.Url = value;
                        break;
                    case 5:
                        //TODO: 使用admin@M365x157144.onmicrosoft.com 的 https://m365x157144.sharepoint.com/sites/BreakinheritanceRule 站点测试 
                        //发现读出来的Datetime 是OADate类型 而不是正常的DateTime.ToString, 在另外一个环境读出来的数据就是普通的DateTime.ToString，因此添加该特殊处理
                        double tempDate;
                        if (!string.IsNullOrEmpty(value) && Double.TryParse(value, out tempDate))
                        {
                            //auditData.OccurredGMT = DateTime.FromOADate(tempDate).ToString();
                        }
                        else
                        {
                            //auditData.OccurredGMT = value;
                        }
                        break;
                    case 6:
                        //auditData.Event = value;
                        break;
                    case 7:
                        //auditData.CustomEventName = value;
                        break;
                    case 8:
                        //auditData.EventSource = value;
                        break;
                    case 9:
                        //auditData.SourceName = value;
                        break;
                    case 10:
                        //auditData.EventData = value;
                        break;
                    case 11:
                        //auditData.AppId = value;
                        break;
                    default:
                        break;
                }

                #endregion get audit data

                i++;
            }
            //ConvertSpecialData(auditData);

            //return auditData;
        }

    }
}
