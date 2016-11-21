using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUG.Packages.KBCodeReview
{
    class ObjectsHelper
    {

        public static bool isGenerated(KBObject obj)
        {
            object aux = obj.GetPropertyValue(Properties.TRN.GenerateObject);
            return ((aux != null) && (aux.ToString() == "True"));

        }

        public static bool isGeneratedbyPattern(KBObject obj)
        {
            if (!(obj == null))
            { return obj.GetPropertyValue<bool>(KBObjectProperties.IsGeneratedObject); }
            else
            { return true; }

        }

        private static string ObjectSourceUpper(KBObject obj)
        {
            string source = "";
            try
            {
                if (obj is Procedure) source = obj.Parts.Get<ProcedurePart>().Source;

                if (obj is Transaction) source = obj.Parts.Get<EventsPart>().Source;

                if (obj is WorkPanel) source = obj.Parts.Get<EventsPart>().Source;

                if (obj is WebPanel) source = obj.Parts.Get<EventsPart>().Source;
            }
            catch (Exception e) {
                string errMsg = e.Message;
                return "";
            }

            return source.ToUpper();
        }

        public static string GetSourceCodeWithoutComments(KBObject obj)
        {
            string sourceWOComments = "";

            if (obj is Transaction || obj is WebPanel || obj is Procedure || obj is WorkPanel)
            {

                if (ObjectsHelper.isGenerated(obj) && !ObjectsHelper.isGeneratedbyPattern(obj))
                {
                    string source = ObjectsHelper.ObjectSourceUpper(obj);
                    source = Functions.RemoveEmptyLines(source);

                    sourceWOComments = Functions.ExtractComments(source);
                }
            }
            return sourceWOComments;
        }

        public static string GetSourceCode(KBObject obj)
        {
            string source = "";

            if (obj is Transaction || obj is WebPanel || obj is Procedure || obj is WorkPanel)
            {

                if (ObjectsHelper.isGenerated(obj) && !ObjectsHelper.isGeneratedbyPattern(obj))
                {
                    source = ObjectsHelper.ObjectSourceUpper(obj);
                    source = Functions.RemoveEmptyLines(source);

                }
            }
            return source;
        }

        public static int MaxBlockOfCode(KBObject obj)
        {
            string source = GetSourceCodeWithoutComments(obj);
            return SourceHelper.MaxCodeBlock(source);
        }

        public static int ComplexityLevel(KBObject obj)
        {
            string source = GetSourceCodeWithoutComments(obj);
            return SourceHelper.ComplexityLevel(source);
        }

        public static int MaxNestLevel(KBObject obj)
        {
            string source = GetSourceCodeWithoutComments(obj);
            return SourceHelper.MaxNestLevel(source);
        }

        public static int CountSourceCodeLines(KBObject obj)
        {
            int sourceLines = 0;
            int commentLines = 0;
            float Percentage = 0;
            Functions.ObjStats_Comments(obj, out sourceLines, out commentLines, out Percentage);

            return sourceLines;
        }

        public static int CountSourceCodeCommentLines(KBObject obj)
        {
            int sourceLines = 0;
            int commentLines = 0;
            float Percentage = 0;
            Functions.ObjStats_Comments(obj, out sourceLines, out commentLines, out Percentage);

            return commentLines;
        }

        public static float SourceCodeCommentPct(KBObject obj)
        {
            int sourceLines = 0;
            int commentLines = 0;
            float Percentage;
            Functions.ObjStats_Comments(obj, out sourceLines, out commentLines, out Percentage);

            return Percentage;
        }

        public static bool IsCallalable(KBObject obj)
        {
            return ((obj is Transaction) || (obj is Procedure) || (obj is WebPanel) || (obj is WorkPanel) || (obj is DataProvider) || (obj is Menubar) || (obj is DataSelector));
        }
    }
}
