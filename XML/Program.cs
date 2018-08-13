using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WTF.Core.Xml;

namespace XmlXslTransformer
{
    class Program
    {
        static XmlUtility xutil;
        static XsltTransformer transformer;

        static void Main(string[] args)
        {
            string xml = string.Empty;
            string xslt = string.Empty;

            string xmlSourceFile = string.Empty;
            string xsltTransformFile = string.Empty;
            string outputFile = string.Empty;

            StringBuilder errorLine = new StringBuilder();
            StringBuilder argumentsPassedIn = new StringBuilder();
            
            if (args.Length != 3)
            {
                errorLine.AppendLine("Faulty command line format. Not enough arguments. Check your command line format." + args.Length.ToString());
                Console.WriteLine(errorLine.ToString() + "\n\nPress any key to quit program, and fix command line");
                Console.ReadKey();
                return;
            }

            xmlSourceFile = args.Where(c=>c.Contains("/xml=")).FirstOrDefault();
            xsltTransformFile = args.Where(c => c.Contains("/xsl=")).FirstOrDefault();
            outputFile = args.Where(c => c.Contains("/out=")).FirstOrDefault();

            argumentsPassedIn.AppendLine("\n\nHere are your arguments:");
            argumentsPassedIn.AppendLine(xmlSourceFile);
            argumentsPassedIn.AppendLine(xsltTransformFile);            
            argumentsPassedIn.AppendLine(outputFile);

            if(string.IsNullOrEmpty(xmlSourceFile) || string.IsNullOrEmpty(xsltTransformFile) || string.IsNullOrEmpty(outputFile))
            {
                errorLine.AppendLine("Faulty command line format. Check your command line format. Make sure all file paths are surrounded by double quotes.  " +
                                    "The switches required are xml=\"[your xml file name]\", xsl=\"[xslt stylesheet]\", and out=\"[your output file]\"" + argumentsPassedIn.ToString());
            }

            xmlSourceFile = xmlSourceFile.Replace("/xml=", "");
            xsltTransformFile = xsltTransformFile.Replace("/xsl=", "");
            outputFile = outputFile.Replace("/out=", "");            

            xutil = new XmlUtility();
            transformer = new XsltTransformer();

            try
            {
                xml = xutil.ReadXmlFile(xmlSourceFile);
                xslt = transformer.ReadStyleSheet(xsltTransformFile);
                transformer.Transform(xml, xslt, outputFile);
                Console.WriteLine("Transformation succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in transformation: " + ex.Message + "\n\n" + errorLine.ToString() + argumentsPassedIn.ToString());
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            
        }
    }
}
