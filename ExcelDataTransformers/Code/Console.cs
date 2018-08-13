using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using WTF.Core;
using OExcel = WTF.Core.OleDbExcel.OleDbExcelUtility; //Ok this IST utility doesn't have everything I need. Going to have to go it alone.
using WTF.Core.Common;
using WTF.Core.Xml;
using WTF.Core.FileSystem;

namespace ExcelToXmlConverter
{
    class Console
    {
        static void Main(string[] args)
        {   
            // XLS - Excel 2003 and Older:
            //@"Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + FILE_PATH + ";"


            // XLSX - Excel 2007, 2010, 2012, 2013
            //@"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=Excel 12.0 XML;Data Source=" + FILE_PATH + ";"


            //const string SOURCE_PATH = @"c:\temp\";
            //const string FILE_1 = "Minhast_Nouns_TestReadByExcelDll.xls";  //DO NOT NAME YOUR SHEETS!  And use "Paste Special" when doing the transfer to subsidieary sheets!
            //const string FILE_2 = "Minhast_Verbs_TestReadByExcelDll.xls";
            //const string FILE_3 = "Minhast_Vocabulary_TestReadAllByExcelDll.xls";
            //const string FILE_NAME = FILE_3;
            //const string FILE_PATH = SOURCE_PATH + FILE_NAME;
            //const string OUTPUT_FILE = FILE_PATH + ".xml";

            const string PRESS_ANY_KEY_TO_EXIT = "\n\n\nPress any key to exit.";

            const string invalidCmdLineLengthOrFormat = "Invalid command-line arguments. \n\n" +
                                    "You must specify 4 parameters in the command line in the following order and format: " +
                                    "/inpath=[input path] /infile=[name of Excel input file] /outpath=[output path] /outfile=[name of xml output file]" +
                                    PRESS_ANY_KEY_TO_EXIT;

            const string pathDoesNotExist = "Invalid path name.  Make sure the path for both your input file and output exists." +
                                            PRESS_ANY_KEY_TO_EXIT;

            const string inputFileDoesNotExist = "Input file does not exist.  Make sure the path and name of your input file are correct." +
                                                 PRESS_ANY_KEY_TO_EXIT;

            if (args == null || args.Length != 4)
            {
                string yourParms = "Your args are:\n\n";
                for (int i = 0; i < args.Length; i++)
                {
                    yourParms = yourParms + args[i] + "\n";
                }

                System.Console.WriteLine(invalidCmdLineLengthOrFormat);
                System.Console.ReadKey();
                return;
            }

            string infilePath = args.Where(c => c.Contains("/inpath=")).FirstOrDefault();
            string infileName = args.Where(c => c.Contains("/infile=")).FirstOrDefault();
            string outfilePath = args.Where(c => c.Contains("/outpath=")).FirstOrDefault();
            string outFileName = args.Where(c => c.Contains("/outfile=")).FirstOrDefault();

            if(string.IsNullOrEmpty(infilePath) || string.IsNullOrEmpty(infileName) || string.IsNullOrEmpty(outfilePath) || string.IsNullOrEmpty(outFileName))
            {
                 System.Console.WriteLine(invalidCmdLineLengthOrFormat);
                 System.Console.ReadKey();
                 return;
            }
            if (!infilePath.Trim().EndsWith(@"\"))
            {
                infilePath = infilePath + "\\";
                infilePath = infilePath.Replace("/inpath=","");
            }
            if(!outfilePath.Trim().EndsWith(@"\"))
            {
                outfilePath = outfilePath + "\\";
                outfilePath = outfilePath.Replace("/outpath=", "");
            }

            infileName = infileName.Replace("/infile=", "");
            outFileName = outFileName.Replace("/outfile=", "");

            WTF.Core.FileSystem.FileSystemManager fileMgr = new FileSystemManager();
            if(!fileMgr.DirectoryExists(infilePath) || !fileMgr.DirectoryExists(outfilePath))
            {
                 System.Console.WriteLine(pathDoesNotExist);
                 System.Console.ReadKey();
                 return;
            }
            if (!fileMgr.FileExists(infilePath + infileName))
            {
                System.Console.WriteLine(inputFileDoesNotExist);
                System.Console.ReadKey();
                return;
            }

            string CONNECTION_STRING = @"Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + infilePath + infileName + ";";

            string xml = string.Empty;
            string tableName = string.Empty;
            string sheetName = string.Empty;

            DataSet ds = new DataSet();
            DataTable dtSheet = null;
            OleDbDataAdapter da = null;

            //OExcel o = new OExcel();
            //Demo of why I can't use this...this line is returning me an empty dataset, and the file_path is the connection string
            //var ttt = o.GetDataTableFromFirstExcelWorksheet(FILE_PATH); 
            
            using (IDbConnection conn = new OleDbConnection(CONNECTION_STRING))
            {
                conn.Open();
                IDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                // Get all Sheets in Excel File
                dtSheet = ((OleDbConnection)(conn)).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                tableName = dtSheet.TableName;

                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    var items = dr.ItemArray.ToList().Where(c=>c.ToString().Contains("$")).ToList();

                    if (items != null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            try
                            {
                                sheetName = items[i].ToString();
                                cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                                da = new OleDbDataAdapter((OleDbCommand)cmd);//I can't seem to use an interface version.

                                DataTable dt = new DataTable();
                                dt.TableName = sheetName;
                                da.Fill(dt);
                                ds.Tables.Add(dt);
                            }
                            catch (System.Exception ex)
                            {
                                string msg = ex.Message;
                            }

                        }
                    }
     
                }

                cmd = null;
                conn.Close();

                xml = ds.GetXml();

                if (string.IsNullOrEmpty(xml) == false)
                {
                    WriteXmlStringToFile(xml, outfilePath +  outFileName);
                }
            }            

            System.Console.WriteLine("Finished processing. Press any key to quit.");
            System.Console.ReadKey();

        }

        //I can't believe that stupid XmlUtility doesn't have "WriteXmlStringToFile" method!
        public static void WriteXmlStringToFile(string xmlString, string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            xdoc.Save(fileName);
        }
    }
}
