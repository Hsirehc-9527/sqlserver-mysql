using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataMigration.DAL
{
    public class WirterLogs
    {
        private static object sign = new object();
        //追加写文件的数据  
        public static void WriteFile(Exception ex)
        {
            lock (sign)
            {
                string filePath = string.Format("{0}\\Log\\{1}.txt", System.IO.Directory.GetCurrentDirectory(), DateTime.Now.ToString("yyyyMMddHHmm"));
                if (!System.IO.Directory.Exists(string.Format("{0}\\Log", System.IO.Directory.GetCurrentDirectory())))
                {
                    System.IO.Directory.CreateDirectory(string.Format("{0}\\Log", System.IO.Directory.GetCurrentDirectory()));
                }
                if (!System.IO.File.Exists(filePath))
                {
                    FileStream stream = System.IO.File.Create(filePath);
                    stream.Close();
                    stream.Dispose();
                }
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    //把异常信息输出到文件
                    sw.WriteLine("当前时间：" + DateTime.Now.ToString());
                    sw.WriteLine("异常信息：" + ex.Message);
                    sw.WriteLine("异常对象：" + ex.Source);
                    sw.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                    sw.WriteLine("触发方法：" + ex.TargetSite);
                    sw.WriteLine();
                }
            }
        }
    }
}
