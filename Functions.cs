using System;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Genexus.Common;
using System.IO;
using System.Linq;

namespace GUG.Packages.KBCodeRevisor
{
    class Functions
    {
        public static string ReturnPicture(Artech.Genexus.Common.Objects.Attribute a)
        {
            string Picture = "";
            Picture = a.Type.ToString() + "(" + a.Length.ToString() + (a.Decimals > 0 ? "." + a.Decimals.ToString() : "") + ")" + (a.Signed ? "-" : "");
            return Picture;
        }

        public static string ReturnPictureVariable(Variable v)
        {
            string Picture = "";
            Picture = v.Type.ToString() + "(" + v.Length.ToString() + (v.Decimals > 0 ? "." + v.Decimals.ToString() : "") + ")" + (v.Signed ? "-" : "");
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


    }
}

