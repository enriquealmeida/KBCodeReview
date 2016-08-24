using Artech.Architecture.UI.Framework.Helper;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace GUG.Packages.KBCodeRevisor
{
    class CommandManager : CommandDelegator
	{
		public CommandManager()
		{
			AddCommand(CommandKeys.ObjectInTextFormat, new ExecHandler(ExecObjectInTextFormat), new QueryHandler(EnableWhenKbOpened));
			AddCommand(CommandKeys.OpenFolderKBCodeRevisor, new ExecHandler(ExecOpenFolderKBCodeRevisor), new QueryHandler(EnableWhenKbOpened));
            AddCommand(CommandKeys.SendDiff, new ExecHandler(ExecSendDiff), new QueryHandler(EnableWhenKbOpened));
            AddCommand(CommandKeys.PushChanges, new ExecHandler(ExecPushChanges), new QueryHandler(EnableWhenKbOpened));            
            AddCommand(CommandKeys.AboutKBCodeRevisor, new ExecHandler(ExecAboutKBCodeRevisor), new QueryHandler(EnableAlways));
			AddCommand(CommandKeys.HelpKBCodeRevisor, new ExecHandler(ExecHelpKBCodeRevisor), new QueryHandler(EnableAlways));
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

        public bool ExecSendDiff(CommandData cmdData)
        {
            KBCodeRevisorHelper.GitInit();
            KBCodeRevisorHelper.GitCommit();
            KBCodeRevisorHelper.ArcDiff();
            
            return true;
        }

        public bool ExecPushChanges(CommandData cmdData) {

            KBCodeRevisorHelper.ArcLand();
            return true;
        }

		public bool ExecOpenFolderKBCodeRevisor(CommandData cmdData)
		{
			KBCodeRevisorHelper.OpenFolderKBCodeRevisor();
			return true;
		}

		private bool EnableWhenKbOpened(CommandData cmdData, ref CommandStatus status)
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

		private bool EnableAlways(CommandData cmdData, ref CommandStatus status)
		{
			status.State = CommandState.Enabled;
			return true;
		}
	}
}
