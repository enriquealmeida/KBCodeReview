using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using Artech.Architecture.UI.Framework.Helper;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Artech.Architecture.Common.Objects;
using Artech.Common.Framework.Selection;
using System.Diagnostics;

namespace GUG.Packages.KBCodeRevisor
{
    class CommandManager : CommandDelegator
    {
        public CommandManager()
        {
            AddCommand(CommandKeys.ObjectInTextFormat, new ExecHandler(ExecObjectInTextFormat), new QueryHandler(QueryKBCodeRevisorNoKB));
            AddCommand(CommandKeys.OpenFolderKBCodeRevisor, new ExecHandler(ExecOpenFolderKBCodeRevisor), new QueryHandler(QueryKBCodeRevisorNoKB));
            AddCommand(CommandKeys.AboutKBCodeRevisor, new ExecHandler(ExecAboutKBCodeRevisor), new QueryHandler(QueryKBCodeRevisorNoKB));
            AddCommand(CommandKeys.HelpKBCodeRevisor, new ExecHandler(ExecHelpKBCodeRevisor), new QueryHandler(QueryKBCodeRevisorNoKB));
        }

      public bool ExecAboutKBCodeRevisor(CommandData cmdData)
        {
            Assembly assem = this.GetType().Assembly;
            object[] atributos = assem.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            using (Form aboutBox = new AboutKBCodeRevisor())
            {
                aboutBox.ShowDialog();
            }
            return true;
        }

        public bool ExecHelpKBCodeRevisor(CommandData cmdData)
        {
            Process.Start("http://wiki.genexus.com/commwiki/servlet/hwikibypageid?31586");
            return true;
        }

        public bool ExecObjectInTextFormat(CommandData cmdData)
        {
            KBCodeRevisorHelper.ExportObjectInTextFormat();
            return true;
        }



        public bool ExecOpenFolderKBCodeRevisor(CommandData cmdData)
        {
            KBCodeRevisorHelper.OpenFolderKBCodeRevisor();
            return true;
        }


        private bool QueryKBCodeRevisor(CommandData cmdData, ref CommandStatus status)
        {
            // This is where you have a chance to modify the status of
            // menu / toolbar items.
            status.State = CommandState.Disabled;

            IKBService kbserv = UIServices.KB;
            if (kbserv != null && kbserv.CurrentKB != null)
            {
                status.State = CommandState.Enabled;
            }

            // return true to indicate you already resolved the command status;
            // otherwise the framework will try with its next registered
            // command target
            return true;
        }

        private bool QueryKBCodeRevisorNoKB(CommandData cmdData, ref CommandStatus status)
        {
            status.State = CommandState.Enabled;

            return true;
        }
    

    }
}
