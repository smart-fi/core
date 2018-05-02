using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;

using HttpUtils;

namespace ExcelToOrionTransfer
{
    class Program
    {

        public static DataTable dtexcel;
        static void Main(string[] args)
        {

            string filePath = string.Empty;
            string fileExt = string.Empty;

            filePath = @"C:\POI.xlsx"; ;//get the path of the file
            fileExt = Path.GetExtension(filePath);//get the file extension

            try
            {
                DataTable dtExcel = new DataTable();
                dtExcel = ReadExcel(filePath, fileExt);
                                                       
                var client = new RestClient();
                client.EndPoint = @"http:\\OrionIP:1026\v2\op\update"; ;
                client.Method = HttpVerb.POST;
                client.ContentType = "application/json";

                foreach (DataRow dr in dtExcel.Rows)
                {
                    try
                    {
                        client.PostData = "{'actionType': 'APPEND', 'entities': [".Replace("'", "\"") + dr[0].ToString() + "]}";
                        var json = client.MakeRequest();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }
        public static DataTable ReadExcel(string fileName, string fileExt)
        {
            string conn = string.Empty;
            dtexcel = new DataTable();

            if (fileExt.CompareTo(".xls") == 0)//compare the extension of the file

                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for excel 2007 and older versions
            else

                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1';"; //newer excel versions
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [sheet1$]", con); //here we read data from sheet1
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            return dtexcel;
        }

    }
}
