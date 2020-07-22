using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Windows.Forms;
using DataMigration.DAL;

namespace DataMigration.BLL
{
    public class ExcelParse
    {
        public static Dictionary<int,string> sheetDBDic;
        /// <summary>
        /// 表格解析
        /// </summary>
        /// <param name="rtb_ReturnMsg"></param>
        /// <param name="excelPath"></param>
        /// <param name="dgv_ExeData"></param>
        public static string Parse(string excelPath,DataGridView dataGridView)
        {
            string result = string.Empty;
            DataTable sheetDBData = new DataTable();
            sheetDBData = NewDBTable();
            sheetDBDic = new Dictionary<int, string>();
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            try
            {
                fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                if (excelPath.IndexOf(".xlsx") > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (excelPath.IndexOf(".xls") > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                if (workbook != null)
                {
                    try
                    {
                        sheet = workbook.GetSheetAt(0);
                        if (sheet != null)
                        {
                            Dictionary<string, int> columsNames = new Dictionary<string, int>();
                            IRow firstRow = sheet.GetRow(0);
                            foreach (ICell cell in firstRow.Cells)
                            {
                                columsNames.Add(cell.StringCellValue, cell.ColumnIndex);
                            }
                            int rowNum = 1;
                            IRow row = null;
                            DataRow dataRow = null;
                            while (rowNum <= sheet.LastRowNum)
                            {
                                row = sheet.GetRow(rowNum++);
                                if (CheckNullData(row))
                                {
                                    dataRow = sheetDBData.NewRow();
                                    dataRow["区服"] = row.GetCell(columsNames["区服"]).ToString();
                                    string connConfig = row.GetCell(columsNames["目标库连接串"]).ToString();
                                    string mysqlConn = connConfig.EndsWith(";") ? connConfig : connConfig + ";";
                                    string str = mysqlConn.Contains("Variables") ? mysqlConn : mysqlConn + "Allow User Variables=True;";
                                    dataRow["目标库连接串"] = mysqlConn.Contains("Variables") ? mysqlConn : mysqlConn + "Allow User Variables=True;";
                                    sheetDBData.Rows.Add(dataRow);
                                    sheetDBDic.Add(int.Parse(dataRow["区服"].ToString()), dataRow["目标库连接串"].ToString());
                                }
                            }
                            result = "表格解析完成" ;
                        }
                        dataGridView.ReadOnly = true;
                        dataGridView.DataSource = sheetDBData;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("读取文档配置失败：{0}", e.Message));
                        WirterLogs.WriteFile(e);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                result = string.Format("表格解析失败:{0}", e.Message);
                return result;
            }
        }

        /// <summary>
        /// 检测是否空行
        /// </summary>
        private static bool CheckNullData(IRow row)
        {
            bool isNull = true;
            if (row == null || row.GetCell(0) == null)
            {
                isNull = false;
            }
            else
            {
                if (string.IsNullOrEmpty(row.GetCell(0).ToString().Trim())) isNull = false;
            }
            return isNull;
        }
        /// <summary>
        /// 构建空表
        /// </summary>
        /// <returns></returns>
        public static DataTable NewDBTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("区服");
            dataTable.Columns.Add("目标库连接串");
            return dataTable;
        }
    }
}
