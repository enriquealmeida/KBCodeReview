using Artech.Architecture.Common;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Properties;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Services;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Parts.SDT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Artech.Architecture.Common.Collections;
using System.Xml;
using System.Xml.Xsl;

namespace GUG.Packages.KBCodeReview
{
    static class KBCodeReviewHelper
    {
        private static string OutputId = "General";

        //--------------------------------GIT-----------------------------------------------------------------------------
        public static void GitInit()
        {
            string path = KBCodeReviewHelper.GetKBCodeReviewDirectory() + "/.git";
            ArrayList lines = new ArrayList();
            string result;
            bool success;
            if (!Directory.Exists(path))
            {

                ExecuteCommand.Execute("git init",out result,out success);
                lines.Add(result);
                GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReviewer - Execute Git Init", success);

            }
            else
            {
                lines.Add("Git was already initialized");
                GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReview - Execute Git Init",true);

            }

        }

        public static void GitClone()
        {
            string GitUser = PropertyAccessor.GetValueString(UIServices.KB.CurrentModel, "Git UserName");
            string GitPwd = PropertyAccessor.GetValueString(UIServices.KB.CurrentModel, "Git Password");
            string GitServer = PropertyAccessor.GetValueString(UIServices.KB.CurrentModel, "Git remote server");

            string user2 = PropertyAccessor.GetValueString(UIServices.KB.CurrentModel, "Git UserName");
            string url = "https://" + GitUser.ToString() + ":" + GitPwd.ToString() + "@" + GitServer.ToString();
            string result2;
            ArrayList debug = new ArrayList();
            result2 = "User: " + GitUser.ToString();
            debug.Add(result2);
            result2 = "Password: " + GitPwd.ToString();
            debug.Add(result2);
            result2 = "GitServer: " + GitServer.ToString();
            debug.Add(result2);
            result2 = "GitUrl: " + url.ToString();
            debug.Add(result2);
            result2 = "User2: " + user2.ToString();
            debug.Add(result2);

            GxConsoleHandler.GitConsoleWriter(debug, "Debugging", true);
            string result;
            ArrayList lines = new ArrayList();
            bool success;
            if (GitServer != "")
            {
                if ((GitUser != "") && (GitPwd != ""))
                {
                    ExecuteCommand.Execute("git clone " + url + " .", out result, out success);
                }
                else
                {
                    result = "UserName or Password can not be null";
                    success = false;
                }
            }
            else
            {
                result = "Git remote server can not be null";
                success = false;
            }

            lines.Add(result);
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReview - Execute Git Clone", success);
        }

        public static void GitCommit()
        {
            string result;
            bool success;
            ExecuteCommand.Execute("git add .", out result, out success);
            ArrayList lines = new ArrayList();
            lines.Add(result);
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReview - Execute Git add .", true);
            string gitcommit = "git commit -m " + '"' + "default message" + '"';
            ExecuteCommand.Execute(gitcommit, out result, out success);
            lines.Clear();
            lines.Add(result);
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReviewer - Execute Git commit", true);
        }

        public static bool GitPull()
        {
            string result;
            bool success;
            ArrayList lines = new ArrayList();
            ExecuteCommand.Execute("git pull", out result, out success);
            lines.Add(result);
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReviewer - Execute Git Pull", true);
            return success;
        }


        public static bool GitStatus()
        {
            string result;
            bool success;
            ExecuteCommand.Execute("git status", out result, out success);
            string aheadTxt = "ahead";
            bool ahead = result.Contains(aheadTxt);
            string pendingDiffs;
            string formatedMsg = null;
            char[] separator = new[] { '\n' };

            ArrayList lines = new ArrayList();
            if (ahead)
            {
                result = "Your branch is ahead of 'origin/master' by 1 commit. Please Push your changes before sending another diff \n";
                lines.Add(result);   
                pendingDiffs = GetPendingDiffs();     
                List<string> LineList= pendingDiffs.Split(separator).ToList();
                formatedMsg = "Diff Message: " + LineList[4].Trim() + '\n';
                foreach (string msg in LineList)
                {
                    if (msg.Contains("Reviewers"))
                    {
                        formatedMsg += msg.Trim() + '\n';
                    }
                    if (msg.Contains("Differential Revision"))
                    {
                        formatedMsg += msg.Trim() + '\n';
                    }
                }                
            }
            lines.Add(formatedMsg);

            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReviewer - Execute Git status",!ahead);
            return ahead;
           
        }

        public static string GetPendingDiffs()
        {

            string result;
            bool success;
            ExecuteCommand.Execute("git log origin/master..HEAD", out result, out success);
            return result;

        }


        public static void ArcDiff()
        {
            ExecuteCommand.ExecuteArc("arc diff");
            ArrayList lines = new ArrayList();
            lines.Add("Executing Arc Diff");
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReview - Execute Arc diff",true);
        }

        public static void ArcLand()
        {
            ExecuteCommand.ExecuteArc("arc land");
            ArrayList lines = new ArrayList();
            lines.Add("Executing Arc Land");
            GxConsoleHandler.GitConsoleWriter(lines, "KBCodeReview - Execute Arc land",true);
        }
        //----------------------------------------------------------------------------------------------------------------

        public static bool IsCodeReviewExportable(IKBObject obj)
        {
            string name = obj.TypeDescriptor.Name;
            ObjectTypeFlags flags = obj.TypeDescriptor.Flags;

            if ((flags & ObjectTypeFlags.Internal) != ObjectTypeFlags.Internal)
            {
                // export all non internal types
                return true;
            }

            // exclude all internal types, except for a few
            if (
                    obj.Type == ObjClass.Table ||
                    obj.Type == ObjClass.Attribute ||
                    obj.Type == ObjClass.Index ||
                    false
                )
            {
                return true;
            }

            return false;
        }

        public static IList<KBObject> SelectObjects()
        {
            SelectObjectOptions selectObjectOption = new SelectObjectOptions();
            selectObjectOption.Filters.Add(obj => IsCodeReviewExportable(obj));
            selectObjectOption.MultipleSelection = true;
            IList<KBObject> objects = UIServices.SelectObjectDialog.SelectObjects(selectObjectOption);
            return objects;
        }

        public static void ExportObjectInTextFormat()
        {
            IList<KBObject> objects = SelectObjects();
            if (objects.Count < 1)
                return;

            ExportObjectInTextFormat(objects);
        }

        public static void ExportObjectInTextFormat(IList<KBObject> objects)
        {
            IOutputService output = CommonServices.Output;
            output.SelectOutput(OutputId);

            string title = "KBCodeReviewer - Generate objects in text format";
            output.StartSection(title);

            bool success = true;
            try
            {
                string outputPath = GetKBCodeReviewDirectory();
                Functions.WriteXSLTtoDir();
                foreach (KBObject obj in objects)
                {
                    output.AddLine(obj.GetFullName());
                    WriteObjectToTextFile(obj, outputPath);
                }
            }
            catch (Exception)
            {
                success = false;
                throw;
            }
            finally
            {
                output.EndSection(title, success);
                output.UnselectOutput(OutputId);
            }
        }

        public static void WriteObjectToTextFile(KBObject obj, string rootFolderPath)
        {
            string objectFolderPath = GetObjectFolderPath(obj, rootFolderPath);
            if (!Directory.Exists(objectFolderPath))
                Directory.CreateDirectory(objectFolderPath);

            string name = Functions.ReplaceInvalidCharacterInFileName(obj.Name) + ".txt";

            string filePath = Path.Combine(objectFolderPath, name);
            using (StreamWriter file = new StreamWriter(filePath))
            {
                PrintHeaderLine("OBJECT:" + obj.Name + " -" + obj.Description + "- TYPE:" + obj.TypeDescriptor.Name, file);
                WriteObjectContent(obj, file);
            }
        }

        private static string GetObjectFolderPath(KBObject obj, string rootFolderPath)
        {
            string parentPath = (obj.ParentKey != null) ? GetObjectFolderPath(obj.Parent, rootFolderPath) : rootFolderPath;

            string objectPath = parentPath;
//            if (obj is Folder || obj is Module)
                objectPath = Path.Combine(parentPath, obj.Name);

            return objectPath;
        }

        private static void WriteObjectContent(KBObject obj, StreamWriter file)
        {

            ListRulePart(obj, file);
            ListVariables(obj, file);
            ListStats(obj, file);

            ListEvents(obj, file);

            switch (obj.TypeDescriptor.Name)
            {
                case "Attribute":
                    ListAttribute(obj, file);
                    break;
                case "Procedure":
                    ListProcedureSource(obj, file);
                    break;
                case "Transaction":
                    ListTransactionStructure(obj, file);
                    break;
                case "WorkPanel":
                    break;
                case "WebPanel":
                    break;
                case "WebComponent":
                    break;
                case "Table":
                    Table tbl = (Table)obj;
                    ListTableStructure(tbl, file);
                    break;
                case "SDT":
                    SDT sdtToList = (SDT)obj;
                    ListSDTStructure(sdtToList, file);
                    break;
                default:
                    //Unknown object. Use export format.
                    file.Write(SerializeObject(obj).ToString());
                    break;
            }


            ListProperties(obj, file);
            ListCategories(obj, file);
            //ListNavigation(obj, file);
        }

        private static void PrintHeaderLine(string Title, StreamWriter file)
        {
            file.WriteLine("************   " + Title.ToUpper() + "   ************************************");
        }

        private static void PrintSectionHeader(string Title, StreamWriter file)
        {
            file.WriteLine(Environment.NewLine + "============   " + Title.ToUpper() + "   ==============================================");
        }

        private static void ListStats(KBObject obj, StreamWriter file)
        {
            PrintSectionHeader("QUALITIY INDICATORS", file);

            int sourceLines = ObjectsHelper.CountSourceCodeLines(obj);
            file.WriteLine("LINES of Code :   {0,-10}", sourceLines.ToString());

            float commentPct = ObjectsHelper.SourceCodeCommentPct(obj);
            file.WriteLine("COMMENTS %    :   {0,-10}", commentPct.ToString());

            int maxBlock = ObjectsHelper.MaxBlockOfCode(obj);
            file.WriteLine("MAX BLOCK     :   {0,-10}", maxBlock.ToString());

            int complexityLevel = ObjectsHelper.ComplexityLevel(obj);
            file.WriteLine("COMPLEXITY    :   {0,-10}", complexityLevel.ToString());

            int maxNest = ObjectsHelper.MaxNestLevel(obj);
            file.WriteLine("MAX NEST LEVEL:   {0,-10}", maxNest.ToString());

        }

        private static void ListRulePart(KBObject obj, StreamWriter file)
        {
            RulesPart rp = obj.Parts.Get<RulesPart>();
            if (rp != null)
            {
                PrintSectionHeader("RULES", file);

                file.WriteLine(rp.Source);
                if (!ValidateParms(obj))
                {
                    file.WriteLine("--> ATTENTION REVIEWER: Parameters without IN/OUT");
                }
            }
        }

        private static bool ValidateParms(KBObject obj)
        {
            bool result = true;
            // Object with parm() rule without in: out: or inout:
            IKBService kbserv = UIServices.KB;
            ICallableObject callableObject = obj as ICallableObject;

            if (callableObject != null)
            {
                foreach (Signature signature in callableObject.GetSignatures())
                {
                    Boolean someInOut = false;
                    foreach (Parameter parm in signature.Parameters)
                    {
                        if (parm.Accessor.ToString() == "PARM_INOUT")
                        {
                            someInOut = true;
                            break;
                        }
                    }
                    if (someInOut)
                    {
                        string ruleParm = Functions.ExtractRuleParm(obj);
                        if (ruleParm != "")
                        {
                            int countparms = ruleParm.Split(new char[] { ',' }).Length;
                            int countsemicolon = ruleParm.Split(new char[] { ':' }).Length - 1;
                            if (countparms != countsemicolon)
                            {
                                string objNameLink = Functions.LinkObject(obj);

                                KBObjectCollection objColl = new KBObjectCollection();

                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static void ListAttribute(KBObject obj, StreamWriter file)
        {
            Artech.Genexus.Common.Objects.Attribute att = (Artech.Genexus.Common.Objects.Attribute)obj;

            file.WriteLine(Functions.ReturnPicture(att));
            if (att.Formula == null)
                file.WriteLine("");
            else
                file.WriteLine(att.Formula.ToString());
        }

        private static void ListProcedureSource(KBObject obj, StreamWriter file)
        {
            ProcedurePart pp = obj.Parts.Get<ProcedurePart>();
            if (pp != null)
            {
                PrintSectionHeader("PROCEDURE SOURCE", file);
                file.WriteLine(pp.Source);
            }
        }

        private static void ListTransactionStructure(KBObject obj, StreamWriter file)
        {
            StructurePart sp = obj.Parts.Get<StructurePart>();
            if (sp != null)
            {
                PrintSectionHeader("STRUCTURE", file);
                file.WriteLine(sp.ToString());
            }
        }

        private static void ListEvents(KBObject obj, StreamWriter file)
        {
            EventsPart ep = obj.Parts.Get<EventsPart>();
            if (ep != null)
            {
                PrintSectionHeader("EVENTS SOURCE", file);
                file.WriteLine(ep.Source);
            }
        }

        private static void ListProperties(KBObject obj, StreamWriter file)
        {
            PrintSectionHeader("PROPERTIES", file);
            //foreach (Property prop in obj.Properties)
            //{
            //if (!prop.IsDefault)
            //{
            //    file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
            //}
            //else
            //{
            //if ((prop.Name == "CommitOnExit") || (prop.Name == "TRNCMT") || (prop.Name == "GenerateObject"))
            //{
            //    file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
            //}
            //}
            //}

            foreach (Property prop in obj.Properties)
            {
                if (prop.Value != null)
                {
                    if ((prop.Name == "Name") || (prop.Name == "Description") || (prop.Name == "IsDefault") ||
                        (prop.Name == "CommitOnExit") || (prop.Name == "TRNCMT") || (prop.Name == "GenerateObject"))
                    {

                        file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
                    }
                }
            }


        }

        private static void ListVariables(KBObject obj, StreamWriter file)
        {
            bool hasUnusedVars = false;

            PrintSectionHeader("VARIABLE DEFINITION", file);
            file.WriteLine(String.Format("{0,-3} {1,-30} {2,-30} {3,-30} {4,-30}", "", "Name", "Type", "Based on", "Desc"));

            VariablesPart vars = obj.Parts.Get<VariablesPart>();
            int alertCount = 0;

            foreach (Variable var in vars.Variables)
            {
                if (!var.IsStandard)
                {

                    string chrAlert = Functions.ReturnVariableDefinitionAlert(var);
                    string dataType = Functions.ReturnPictureVariable(var);
                    string basedOn = var.AttributeBasedOn == null & var.DomainBasedOn == null ? "" : var.GetPropertyValue<string>(Properties.ATT.DataTypeString);
                    bool inUse = Functions.CheckObjUsesVariable(var, obj);

                    if (chrAlert != "")
                    { alertCount += 1; }

                    if (!inUse)
                    {
                        chrAlert += "@";
                        hasUnusedVars = true;
                    }

                    file.WriteLine(String.Format("{0,-3} {1,-30} {2,-30} {3,-30} {4,-30}",
                        chrAlert,
                        var.Name,
                        dataType,
                        basedOn,
                        var.Description
                        ));
                }
            }

            if (alertCount > 0 || hasUnusedVars)
            {
                file.WriteLine(Environment.NewLine + "--> ATTENTION REVIEWER:");
            }

            if (alertCount > 0)
            {
                file.WriteLine("-Sign ! indicates Autodefined variables");
                file.WriteLine("-Sign * indicates variables poorly defined");
            }

            if (hasUnusedVars)
            {
                file.WriteLine("-Sign @ indicates variables not used in the code");
            }
        }

        private static void ListCategories(KBObject obj, StreamWriter file)
        {
            //CATEGORIES
            IEnumerable<Artech.Udm.Framework.References.EntityReference> refe = obj.GetReferences();

            string GUIDCatString = "00000000-0000-0000-0000-000000000006";
            List<string> categories = new List<string>();

            foreach (Artech.Udm.Framework.References.EntityReference reference in refe)
            {
                Guid GUIDRefTo = reference.To.Type;
                string GUIDRefToString = GUIDRefTo.ToString();

                if (GUIDRefToString == GUIDCatString)
                {
                    KBCategory cat = KBCategory.Get(UIServices.KB.CurrentModel, reference.To.Id);
                    categories.Add(cat.Name);
                }
            }

            if (categories.Count > 0)
            {
                PrintSectionHeader("CATEGORIES", file);
                foreach (string name in categories)
                {
                    file.WriteLine(name);
                }
            }
        }

        private static void ListSDTStructure(SDT sdtToList, StreamWriter file)
        {
            if (sdtToList != null)
            {
                PrintSectionHeader("SDT STRUCTURE", file);
                ListStructure(sdtToList.SDTStructure.Root, 0, file);
            }
        }

        private static void ListTableStructure(Table tbl, StreamWriter file)
        {
            foreach (TableAttribute attr in tbl.TableStructure.Attributes)
            {
                String line = "";
                if (attr.IsKey)
                {
                    line = "*";
                }
                else
                {
                    line = " ";
                }

                line += attr.Name + "  " + attr.GetPropertiesObject().GetPropertyValueString("DataTypeString") + "-" + attr.GetPropertiesObject().GetPropertyValueString("Formula");

                if (attr.IsExternalRedundant)
                    line += " External_Redundant";

                line += " Null=" + attr.IsNullable;
                if (attr.IsRedundant)
                    line += " Redundant";

                file.WriteLine(line);
            }
        }

        private static string SerializeObject(KBObject obj)
        {
            StringBuilder buffer = new StringBuilder();
            using (TextWriter writer = new StringWriter(buffer))
                obj.Serialize(writer);
            return buffer.ToString();
        }

        private static void ListStructure(SDTLevel level, int tabs, System.IO.StreamWriter file)
        {
            WriteTabs(tabs, file);
            file.Write(level.Name);
            if (level.IsCollection)
                file.Write(", collection: {0}", level.CollectionItemName);
            file.WriteLine();

            foreach (var childItem in level.GetItems<SDTItem>())
                ListItem(childItem, tabs + 1, file);
            foreach (var childLevel in level.GetItems<SDTLevel>())
                ListStructure(childLevel, tabs + 1, file);
        }


        private static void ListItem(SDTItem item, int tabs, System.IO.StreamWriter file)
        {
            WriteTabs(tabs, file);
            string dataType = item.Type.ToString().Substring(0, 1) + "(" + item.Length.ToString() + (item.Decimals > 0 ? "." + item.Decimals.ToString() : "") + ")" + (item.Signed ? "-" : "");
            file.WriteLine("{0}, {1}, {2} {3}", item.Name, dataType, item.Description, (item.IsCollection ? ", collection " + item.CollectionItemName : ""));
        }

        private static void WriteTabs(int tabs, System.IO.StreamWriter file)
        {
            while (tabs-- > 0)
                file.Write('\t');
        }

        public static void OpenFolderKBCodeReview()
        {
            Process.Start(GetKBCodeReviewDirectory());
        }

        public static string GetSpcDirectory(IKBService kbserv)
        {
            GxModel gxModel = kbserv.CurrentKB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            return kbserv.CurrentKB.Location + string.Format(@"\GXSPC{0:D3}\", gxModel.Model.Id);
        }

		public static string GetKBCodeReviewDirectory()
		{
			return GetKBCodeReviewDirectory(UIServices.KB);
		}

        public static string GetKBCodeReviewDirectory(IKBService kbserv)
        {
            string dir = Path.Combine(GetSpcDirectory(kbserv), "KBCodeReview");
			if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        private static void ListNavigation(KBObject obj, StreamWriter file)
        {

            PrintSectionHeader("NAVIGATION", file);

            string navigation = NavigationHelper.GetNavigation(obj);

            file.Write(Environment.NewLine + navigation);

        }


    }
}
