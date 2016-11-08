using Artech.Architecture.Common;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Properties;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Parts.SDT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GUG.Packages.KBCodeReview
{
	static class KBCodeReviewHelper
    {
		private static string OutputId = "General";
        
        //--------------------------------GIT-----------------------------------------------------------------------------
        public static void GitInit()
        {
            string path = KBCodeReviewHelper.GetKBCodeReviewDirectory() + "/.git";
            if (!Directory.Exists(path))
            {

                string result = ExecuteCommand.Execute("git init");
                GxConsoleHandler.GitConsoleWriter(result, "KBCodeReviewer - Execute Git Init");

            }
            else
            {
                GxConsoleHandler.GitConsoleWriter("Git was already initialized", "KBCodeReviewer - Execute Git Init");

            }

        }

        public static void GitCommit()
        {
            string result = ExecuteCommand.Execute("git add .");
            GxConsoleHandler.GitConsoleWriter(result, "KBCodeReviewer - Execute Git add .");
            string gitcommit = "git commit -m " + '"' + "default message" + '"';
            result = ExecuteCommand.Execute(gitcommit);
            GxConsoleHandler.GitConsoleWriter(result, "KBCodeReviewer - Execute Git commit");
        }

        public static void ArcDiff()
        {
            ExecuteCommand.ExecuteArc("arc diff");
            GxConsoleHandler.GitConsoleWriter("Executing Arc Diff", "KBCodeReviewer - Execute Arc diff");
        }

        public static void ArcLand()
        {
            ExecuteCommand.ExecuteArc("arc land");
            GxConsoleHandler.GitConsoleWriter("Executing Arc Land", "KBCodeReviewer - Execute Arc land");
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
					obj.Type == ObjClass.Table		||
					obj.Type == ObjClass.Attribute	||
					obj.Type == ObjClass.Index		||
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
				file.WriteLine("======OBJECT = " + name + " === " + obj.Description + "=====");
				WriteObjectContent(obj, file);
			}
		}

		private static string GetObjectFolderPath(KBObject obj, string rootFolderPath)
		{
			string parentPath = (obj.ParentKey != null) ? GetObjectFolderPath(obj.Parent, rootFolderPath) : rootFolderPath;

			string objectPath = parentPath;
			if (obj is Folder || obj is Module)
				objectPath = Path.Combine(parentPath, obj.Name);

			return objectPath;
		}

		private static void WriteObjectContent(KBObject obj, StreamWriter file)
        {
            ListRulePart(obj, file);
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


        }

        private static void ListRulePart(KBObject obj, StreamWriter file)
        {
            RulesPart rp = obj.Parts.Get<RulesPart>();
            if (rp != null)
            {
                file.WriteLine(Environment.NewLine + "=== RULES ===");
                file.WriteLine(rp.Source);
            }
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
                file.WriteLine(Environment.NewLine + "=== PROCEDURE SOURCE ===");
                file.WriteLine(pp.Source);
            }
        }

        private static void ListTransactionStructure(KBObject obj, StreamWriter file)
        {
            StructurePart sp = obj.Parts.Get<StructurePart>();
            if (sp != null)
            {
                file.WriteLine(Environment.NewLine + "=== STRUCTURE ===");
                file.WriteLine(sp.ToString());
            }
        }

        private static void ListEvents(KBObject obj, StreamWriter file)
        {
            EventsPart ep = obj.Parts.Get<EventsPart>();
            if (ep != null)
            {
                file.WriteLine(Environment.NewLine + "=== EVENTS SOURCE ===");
                file.WriteLine(ep.Source);
            }
        }

        private static void ListProperties(KBObject obj, StreamWriter file)
        {
            file.WriteLine(Environment.NewLine + "====== PROPERTIES =======");
            foreach (Property prop in obj.Properties)
            {
                if (!prop.IsDefault)
                {
                    file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
                }
                else
                {
                    if ((prop.Name == "CommitOnExit") || (prop.Name == "TRNCMT") || (prop.Name == "GenerateObject"))
                    {
                        file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
                    }
                }
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
                file.WriteLine(Environment.NewLine + "====== CATEGORIES =======");
                foreach (string name in categories)
                {
                    file.WriteLine(name);
                }
            }
        }

        private static void ListSDTStructure( SDT sdtToList, StreamWriter file)
        {
            if (sdtToList != null)
            {
                file.WriteLine(Environment.NewLine + "=== STRUCTURE ===");
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

        public static void ListProperties(KBObject obj)
        {

                
        }
    }
}
