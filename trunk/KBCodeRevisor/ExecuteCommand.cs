using System;

namespace GUG.Packages.KBCodeRevisor
{
    static class ExecuteCommand
    {

        //That must be separated and eliminate duplicate code

        public static String Execute(string _Command) {

            //Init Cmd.exe
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + _Command);
            //Redirect output into a stream
            string KBCodeReviwerDirectory = KBCodeRevisorHelper.GetKBCodeRevisorDirectory();
            procStartInfo.WorkingDirectory = KBCodeReviwerDirectory;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //Background execution - no black window
            procStartInfo.CreateNoWindow = true;
            //Inicializa el proceso
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            //Consigue la salida de la Consola(Stream) y devuelve una cadena de texto
            string result = proc.StandardOutput.ReadToEnd();
            //Muestra en pantalla la salida del Comando 
            Console.WriteLine(result);
            return result;
        }

        public static void ExecuteArc(string _Command)
        {

            //Init Cmd.exe
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/k " + _Command);
            //Redirect output into a stream
            string KBCodeReviwerDirectory = KBCodeRevisorHelper.GetKBCodeRevisorDirectory();
            procStartInfo.WorkingDirectory = KBCodeReviwerDirectory;
            procStartInfo.RedirectStandardInput = false;
            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.UseShellExecute = true;
            //Background execution - no black window
            procStartInfo.CreateNoWindow = false;
            //Inicializa el proceso
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            
            proc.Start();

        }

    }
}
