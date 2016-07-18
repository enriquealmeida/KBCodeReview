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

namespace GUG.Packages.KBCodeRevisor
{
	static class KBCodeRevisorHelper
    {
		private static string OutputId = "General";

		public static bool IsCodeRevisorExportable(IKBObject obj)
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
			selectObjectOption.Filters.Add(obj => IsCodeRevisorExportable(obj));
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

			string title = "KBCodeRevisor - Generate objects in text format";
			output.StartSection(title);

            bool success = true;
			try
			{
				string outputPath = GetKBCodeRevisorDirectory();
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

			string name = Functions.ReplaceInvalidCharacterInFileName(obj.Name) + "." + obj.TypeDescriptor.Name;
				
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
			RulesPart rp = obj.Parts.Get<RulesPart>();
			if (rp != null)
			{
				file.WriteLine(Environment.NewLine + "=== RULES ===");
				file.WriteLine(rp.Source);
			}

            EventsPart ep = obj.Parts.Get<EventsPart>();
            if (ep != null)
            {
                file.WriteLine(Environment.NewLine + "=== EVENTS SOURCE ===");
                file.WriteLine(ep.Source);
            }

            switch (obj.TypeDescriptor.Name)
			{
				case "Attribute":

					Artech.Genexus.Common.Objects.Attribute att = (Artech.Genexus.Common.Objects.Attribute)obj;

					file.WriteLine(Functions.ReturnPicture(att));
					if (att.Formula == null)
						file.WriteLine("");
					else
						file.WriteLine(att.Formula.ToString());
					break;

				case "Procedure":
					ProcedurePart pp = obj.Parts.Get<ProcedurePart>();
					if (pp != null)
					{
						file.WriteLine(Environment.NewLine + "=== PROCEDURE SOURCE ===");
						file.WriteLine(pp.Source);
					}
					break;
				case "Transaction":
					StructurePart sp = obj.Parts.Get<StructurePart>();
					if (sp != null)
					{
						file.WriteLine(Environment.NewLine + "=== STRUCTURE ===");
						file.WriteLine(sp.ToString());
					}
					break;

				case "WorkPanel":
					break;
				case "WebPanel":
					break;
				case "WebComponent":
					break;

				case "Table":
					Table tbl = (Table)obj;

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
					break;


				case "SDT":
					SDT sdtToList = (SDT)obj;
					if (sdtToList != null)
					{
						file.WriteLine(Environment.NewLine + "=== STRUCTURE ===");
						ListStructure(sdtToList.SDTStructure.Root, 0, file);
					}
					break;

				default:

					//Unknown object. Use export format.
					file.Write(SerializeObject(obj).ToString());
					break;
			}
            
            file.WriteLine(Environment.NewLine + "====== PROPERTIES =======");
            foreach (Property prop in obj.Properties)
            {
                if (!prop.IsDefault)
                    file.WriteLine(prop.Name + " -> " + prop.Value.ToString());
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

        public static void OpenFolderKBCodeRevisor()
        {
            Process.Start(GetKBCodeRevisorDirectory());
        }

        public static string GetSpcDirectory(IKBService kbserv)
        {
            GxModel gxModel = kbserv.CurrentKB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            return kbserv.CurrentKB.Location + string.Format(@"\GXSPC{0:D3}\", gxModel.Model.Id);
        }

		public static string GetKBCodeRevisorDirectory()
		{
			return GetKBCodeRevisorDirectory(UIServices.KB);
		}

        public static string GetKBCodeRevisorDirectory(IKBService kbserv)
        {
            string dir = Path.Combine(GetSpcDirectory(kbserv), "KBCodeRevisor");
			if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public static void ListProperties(KBObject obj)
        {

                
        }
    }
}
