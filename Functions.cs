using System;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Parts;
using System.IO;
using System.Linq;
using Artech.Architecture.UI.Framework.Services;
using System.Text.RegularExpressions;

namespace GUG.Packages.KBCodeReview
{
    class Functions
    {

        public static string StripHTML(string HTMLText)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(HTMLText, " ");
        }

        public static string linkFile(string file)
        {
            return "<a href=\"file:///" + file + "\"" + ">" + file + "</a" + ">";
        }

        public static string ReturnPicture(Artech.Genexus.Common.Objects.Attribute a)
        {
            string Picture = "";
            Picture = a.Type.ToString() + "(" + a.Length.ToString() + (a.Decimals > 0 ? "." + a.Decimals.ToString() : "") + ")" + (a.Signed ? "-" : "");
            return Picture;
        }

        public static string ReturnVariableDefinitionAlert(Variable v)
        {
            string chrAlert = "";
            if (v.IsAutoDefined)
            {
                chrAlert = "!";
            }
            else
            {
                if (v.AttributeBasedOn == null & v.DomainBasedOn == null & v.Type.ToString() != "GX_USRDEFTYP")
                {
                    chrAlert = "*";
                }
            }
               
            return chrAlert;
        }

        public static bool CheckObjUsesVariable(Variable v, KBObject obj)
        {
            bool varused = true;
            if (!v.IsStandard)
            {
                varused = false;
                ProcedurePart pp = obj.Parts.Get<ProcedurePart>();
                if (pp != null)
                {
                    varused = _VarUsedInText(pp.Source, v.Name);
                }
                if (!varused)
                {
                    RulesPart rp = obj.Parts.Get<RulesPart>();
                    if (rp != null)
                    {
                        varused = _VarUsedInText(rp.Source, v.Name);
                    }
                }
                if (!varused)
                {
                    ConditionsPart cp = obj.Parts.Get<ConditionsPart>();
                    if (cp != null)
                    {
                        varused = _VarUsedInText(cp.Source, v.Name);
                    }
                }
                if (!varused)
                {
                    EventsPart ep = obj.Parts.Get<EventsPart>();
                    if (ep != null)
                    {
                        varused = _VarUsedInText(ep.Source, v.Name);
                    }
                }
                if (!varused)
                {
                    WebFormPart fp = obj.Parts.Get<WebFormPart>();
                    if (fp != null)
                    {
                        varused = _VarUsedInWebForm(fp, v.Id); ;
                    }
                }
                
            }
            return varused;
        }

        public static void ObjStats_Comments(KBObject obj, out int linesSource, out int linesComment, out float PercentComment)
        {
            linesSource = 0;
            linesComment = 0;
            PercentComment = 0;

            if (obj is Transaction || obj is WebPanel || obj is Procedure || obj is WorkPanel)
            {

                if (ObjectsHelper.isGenerated(obj) && !ObjectsHelper.isGeneratedbyPattern(obj))
                {
                    string sourceWOComments = ObjectsHelper.GetSourceCodeWithoutComments(obj);
                    string source = ObjectsHelper.GetSourceCode(obj);

                    CountCommentsLines(source, sourceWOComments, out linesSource, out linesComment, out PercentComment);

                }
            }
        }

        public static int LineCount(string s)
        {
            int n = 0;
            foreach (var c in s)
            {
                if (c == '\n') n++;
            }
            return n;
        }

        public static string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", "", RegexOptions.Multiline);
        }

        public static string ExtractComments(string source)
        {

            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(source, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
             me =>
             {
                 if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                     return me.Value.StartsWith("//") ? Environment.NewLine : "";
                 // Keep the literal strings
                 return me.Value;
             },
                    RegexOptions.Singleline);

            noComments = noComments.Replace("(", " (");
            noComments = noComments.Replace(")", ") ");
            noComments = noComments.Replace("\"", "\'");
            noComments = noComments.Replace("\t", " ");

            //saco blancos
            string aux = noComments.Replace("  ", " ");
            do
            {
                noComments = aux;
                aux = noComments.Replace("  ", " ");
            } while (noComments != aux);

            //noComments = noComments.ToUpper();
            return noComments;
        }

        public static string ReturnPictureVariable(Variable v)
        {
            string Length = "";

            if (v.Type.ToString() == "GX_SDT" || v.Type.ToString() == "Boolean" || v.Type.ToString() == "GX_USRDEFTYP")
                Length = "";
            else
                Length = "(" + v.Length.ToString() + (v.Decimals > 0 ? "." + v.Decimals.ToString() : "") + ")" + (v.Signed ? "-" : "");

            string Picture = "";

            if (v.Type.ToString() == "GX_USRDEFTYP")
            {
                Picture = v.GetPropertyValue<string>(Properties.ATT.DataTypeString);
            }
            else
            {
                Picture = v.Type.ToString() + Length;
            }

            return Picture;

        }

        public static void SaveObject(IOutputService output, KBObject obj)
        {

            try
            {
                obj.Save();
            }
            catch (Exception e)
            {
                output.AddErrorLine(e.Message + " - " + e.InnerException);
            }
        }

        public static string ReturnPictureDomain(Domain d)
        {
                  
            string Picture = "";
            Picture = d.Type.ToString() + "(" + d.Length.ToString() + (d.Decimals > 0 ? "." + d.Decimals.ToString() : "") + ")" + (d.Signed ? "-" : "");
            return Picture;
        }

        public static string ReplaceInvalidCharacterInFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            string invalidCharsRemoved = new string(name
            .Where(x => !invalidChars.Contains(x))
            .ToArray());
            name = name.Replace("'", "");
            name = name.Replace(":", "_");
            name = name.Replace(" ", "");
            name = name.Replace(@"\", "_");
            name = name.Replace("/", "_");
            return name;
        }

        public static string SpcDirectory(IKBService kbserv)
        {
            GxModel gxModel = kbserv.CurrentKB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            return kbserv.CurrentKB.Location + string.Format(@"\GXSPC{0:D3}\", gxModel.Model.Id);
        }

        public static string CreateOutputFile(IKBService kbserv, string title)
        {
            string outputFile = kbserv.CurrentKB.UserDirectory + @"\kbdoctor." + Functions.CleanFileName(title) + ".html";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            return outputFile;
        }

        public static string CleanFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static string ExtractRuleParm(KBObject obj)
        {
            RulesPart rulesPart = obj.Parts.Get<RulesPart>();
            string aux = "";

            if (rulesPart != null)
            {
                Regex myReg = new Regex("//.*", RegexOptions.None);
                Regex paramReg = new Regex(@"parm\(.*\)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                string reglas = rulesPart.Source;
                reglas = myReg.Replace(reglas, "");
                Match match = paramReg.Match(reglas);
                if (match != null)
                    aux = match.ToString();
                else
                    aux = "";
            }
            return aux;
        }

        public static string linkObject(KBObject obj)
        {
            return "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;OpenObject&name=" + obj.Guid.ToString() + "\">" + obj.Name + "</a>";
        }

        private static bool _VarUsedInText(string sourceCode, string varName)
        {
            bool usedvar = false;
            if (sourceCode != null)
            {
                Regex myReg = new Regex("//.*", RegexOptions.None);
                Regex myReg2 = new Regex(@"/\*.*\*/", RegexOptions.Singleline);
                Regex paramReg = new Regex(varName + @"\W+", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                sourceCode = myReg.Replace(sourceCode, "");
                sourceCode = myReg2.Replace(sourceCode, "");
                Match match = paramReg.Match(sourceCode);
                if (match.Success)
                {
                    usedvar = true;
                }
                return usedvar;
            }
            else
            {
                return false;
            }
        }

        private static bool _VarUsedInWebForm(WebFormPart wF, int varId)
        {
            return (wF.GetVariable(varId) != null);
        }

        private static void CountCommentsLines(string source, string sourceWOComments, out int linesSource, out int linesComment, out float PercentComment)
        {
            linesSource = Functions.LineCount(source);
            int linesWOComment = Functions.LineCount(sourceWOComments);

            linesComment = linesSource - linesWOComment;
            PercentComment = (linesSource == 0) ? 0 : (linesComment * 100) / linesSource;
        }

        internal static void AddLineSummary(string fileName, string texto)
        {
            IKBService kbserv = UIServices.KB;

            string outputFile = kbserv.CurrentKB.UserDirectory + @"\" + fileName;

            using (FileStream fs = new FileStream(outputFile, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(DateTime.Now.ToString() + "," + texto);
            }
        }

        internal static KBCategory MainCategory(KBModel model)
        {
            return KBCategory.Get(model, "Main Programs");
        }
    }
}


