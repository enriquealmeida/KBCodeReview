using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUG.Packages.KBCodeReview
{
    static class ExecuteCommand
    {

        //That must be separated and eliminate duplicate code

        public static void Execute(string _Command, out string result, out bool success) {

            //Init Cmd.exe
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + _Command);
            //Redirect output into a stream
            string KBCodeReviwerDirectory = KBCodeReviewHelper.GetKBCodeReviewDirectory();
            procStartInfo.WorkingDirectory = KBCodeReviwerDirectory;
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //Background execution - no black window
            procStartInfo.CreateNoWindow = true;
            //Inicializa el proceso
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            //Consigue la salida de la Consola(Stream) y devuelve una cadena de texto
            proc.WaitForExit();         
            string ErrorResult = proc.StandardError.ReadToEnd();
            string resultOK = proc.StandardOutput.ReadToEnd();
            int exitCode = proc.ExitCode;
            //Muestra en pantalla la salida del Comando 
            if (exitCode != 0)
            {
                success = false; 
            }
            else
            {
                success = true;
            }
            //This is because sometimes git send the output to the standardError although there were no errors.
            if (ErrorResult != "")
            {
                result = ErrorResult;
            }
            else
            {
                result = resultOK;
            }
           
        }

        public static void ExecuteArc(string _Command)
        {

            //Init Cmd.exe
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/k " + _Command);
            //Redirect output into a stream
            string KBCodeReviwerDirectory = KBCodeReviewHelper.GetKBCodeReviewDirectory();
            procStartInfo.WorkingDirectory = KBCodeReviwerDirectory;
            procStartInfo.RedirectStandardInput = false;
            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.UseShellExecute = true;
            //Background execution - black window
            procStartInfo.CreateNoWindow = false;
            //Inicializa el proceso
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            
            proc.Start();

        }

    }
}
