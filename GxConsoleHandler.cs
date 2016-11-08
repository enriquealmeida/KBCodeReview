using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Artech.Architecture.Common.Services;

namespace GUG.Packages.KBCodeReview
{
    static class GxConsoleHandler
    {
        private static string OutputId = "General";

        // THIS MUST BE SEPARATED FROM HERE
        public static void GitConsoleWriter(string result,string title)
        {
            IOutputService output = InitializeGXOutput();
            GXWrtStartConsole(output, title);
            GXWrtLineConsole(output, result);
            GXEndOutputSection(output, title, true);
        }


        public static void GitAlreadyInitiated(string result) {
            IOutputService output = InitializeGXOutput();
            GXWrtLineConsole(output, result);
        }

        //------------------------------------------------------------------------------

        private static IOutputService InitializeGXOutput()
        {
            IOutputService output = CommonServices.Output;
            output.SelectOutput(OutputId);
            return output;
        }

        private static void GXWrtStartConsole(IOutputService output,string text)
        {
            output.StartSection(text);
        }

        private static void GXWrtLineConsole(IOutputService output, string text)
        {
            output.AddLine(text);
        }

        private static void GXEndOutputSection(IOutputService output,string title,bool success)
        {
            output.EndSection(title, success);
            output.UnselectOutput(OutputId);
        }


        

    }
}
