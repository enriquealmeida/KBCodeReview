using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Text.RegularExpressions;

using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;


namespace GUG.Packages.KBCodeReview
{
    class NavigationHelper
    {

        public static XmlDocument GetObjectNavigationXML(KBObject obj)
        {
            XmlDocument doc = new XmlDocument();
            IKBService kbserv = UIServices.KB;
            string fileWildcard = @"" + obj.Name + ".xml";
            var searchSubDirsArg = System.IO.SearchOption.AllDirectories;
            string specpath = Functions.SpcDirectory(kbserv);

            string[] xFiles = System.IO.Directory.GetFiles(specpath, fileWildcard, searchSubDirsArg);

            foreach (string x in xFiles)
            {

                if (Path.GetFileNameWithoutExtension(x).Equals(obj.Name))
                {
                    string xmlstring = NavigationHelper.AddXMLHeader(x);

                    if (!ObjectsHelper.isGeneratedbyPattern(obj))
                    {
                        doc.LoadXml(xmlstring);
                    }
                }

            }

            return (doc);
        }

        public static string GetObjectNavigationFile(KBObject obj)
        {
            string filename = "";
            IKBService kbserv = UIServices.KB;
            string fileWildcard = @"" + obj.Name + ".xml";
            var searchSubDirsArg = System.IO.SearchOption.AllDirectories;

            string specpath = Functions.SpcDirectory(kbserv);

            string[] xFiles = System.IO.Directory.GetFiles(specpath, fileWildcard, searchSubDirsArg);

            foreach (string x in xFiles)
            {

                if (Path.GetFileNameWithoutExtension(x).Equals(obj.Name))
                {
                    filename = x;
                }

            }

            return (filename);
        }

        public static string GetNavigation(KBObject obj)
        {

            string specFilePath = GetObjectNavigationFile(obj);

            //Copy the navigation file to an .xxx file
            string xmlstring = AddXMLHeader(specFilePath);
            string tempSpecFilePath = specFilePath.Replace(".xml", ".xxx");
            File.WriteAllText(tempSpecFilePath, xmlstring);

            //Output file
            IKBService kbserv = UIServices.KB;
            string navigationPath = Functions.SpcDirectory(kbserv);
            string navigationFilePath = navigationPath + Path.GetFileNameWithoutExtension(specFilePath) + ".nvg";

            XslTransform xsl = NavigationHelper.GetNavigationTemplate();
            xsl.Transform(tempSpecFilePath, navigationFilePath);
            string readText = File.ReadAllText(navigationFilePath);

            //now clean up all intermediate files
            File.Delete(tempSpecFilePath);
            File.Delete(navigationFilePath);

            return readText;
        }

        private static XslTransform GetNavigationTemplate()
        {
            IKBService kbserv = UIServices.KB;
            string outputFile = kbserv.CurrentKB.UserDirectory + @"\NavigationHtmlToText.xslt";
            XslTransform xsl = new XslTransform();
            xsl.Load(outputFile);
            return xsl;
        }

        static string ExtractSpcInfo(string file, string containsTextArg)
        {
            string sourcestring = AddXMLHeader(file);

            sourcestring = sourcestring.Replace(System.Environment.NewLine, "");

            Regex re = new Regex("");
            RegexOptions myRegexOptions = RegexOptions.Multiline;
            string mTxt = "";

            switch (containsTextArg)
            {
                case "Error":
                    re = new Regex(@"<Errors>(.)*<\/Errors>", myRegexOptions);
                    break;
                case "Warning":
                    re = new Regex(@"<Warnings>(.)*<\/Warnings>", myRegexOptions);
                    break;
                case "deprecated":
                    re = new Regex(@"<Warnings>(.)*<\/Warnings>", myRegexOptions);
                    break;
                case "client":

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sourcestring);

                    XmlNode node = xmlDocument.DocumentElement.SelectSingleNode(@"//Condition[Icon='client']");

                    mTxt = node.InnerXml.ToString();
                    Regex r = new Regex(@"<AttriId>(.)*<\/AttriId>", RegexOptions.Singleline);
                    mTxt = r.Replace(mTxt, string.Empty);

                    Regex r2 = new Regex(@"<Description>(.)*<\/Description>", RegexOptions.Singleline);
                    mTxt = r2.Replace(mTxt, string.Empty);

                    mTxt = mTxt.Replace("client", System.Environment.NewLine);

                    return Functions.StripHTML(mTxt);

                default:

                    return Functions.LinkFile(file);

            }
            MatchCollection mc = re.Matches(sourcestring);

            foreach (Match m in mc)
            {
                for (int gIdx = 0; gIdx < m.Groups.Count; gIdx++)
                    if (gIdx <= 4)
                        mTxt += m.Groups[gIdx].Value;
            }

            mTxt = Functions.StripHTML(mTxt);
            mTxt = mTxt.Replace(">", "");
            return mTxt;
        }

        public static string AddXMLHeader(string fileName)
        {
            string xmlstring = File.ReadAllText(fileName);
            xmlstring = "<?xml version='1.0' encoding='iso-8859-1'?>" + xmlstring;
            return xmlstring;
        }

    }
}
