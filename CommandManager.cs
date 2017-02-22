using Artech.Architecture.UI.Framework.Helper;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Artech.Genexus.Common;
using Artech.Architecture.Common.Objects;

namespace GUG.Packages.KBCodeReview
{
    class CommandManager : CommandDelegator
	{
		public CommandManager()
		{
			AddCommand(CommandKeys.ObjectInTextFormat, new ExecHandler(ExecObjectInTextFormat), new QueryHandler(EnableWhenKbOpened));
			AddCommand(CommandKeys.OpenFolderKBCodeReview, new ExecHandler(ExecOpenFolderKBCodeReview), new QueryHandler(EnableWhenKbOpened));
            AddCommand(CommandKeys.Clone, new ExecHandler(ExecClone), new QueryHandler(EnableWhenKbOpened));
            AddCommand(CommandKeys.SendDiff, new ExecHandler(ExecSendDiff), new QueryHandler(EnableWhenKbOpened));
            AddCommand(CommandKeys.PushChanges, new ExecHandler(ExecPushChanges), new QueryHandler(EnableWhenKbOpened));            
            AddCommand(CommandKeys.AboutKBCodeReview, new ExecHandler(ExecAboutKBCodeReview), new QueryHandler(EnableAlways));
			AddCommand(CommandKeys.HelpKBCodeReview, new ExecHandler(ExecHelpKBCodeReview), new QueryHandler(EnableAlways));
		}

		public bool ExecAboutKBCodeReview(CommandData cmdData)
		{
			Assembly assem = this.GetType().Assembly;
			object[] atributos = assem.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
			using (Form aboutBox = new AboutKBCodeReview())
			{
				aboutBox.ShowDialog();
			}
			return true;
		}

		public bool ExecHelpKBCodeReview(CommandData cmdData)
		{
			Process.Start("http://wiki.genexus.com/commwiki/servlet/hwikibypageid?31586");
			return true;
		}

        public bool ExecObjectInTextFormat(CommandData cmdData)
        {
            
            KBCodeReviewHelper.ExportObjectInTextFormat();
            bool ok = KBCodeReviewHelper.GitPull();

            return true;
		}

        public bool ExecClone(CommandData cmdData)
        {
            KBCodeReviewHelper.GitClone();
            return true;
        }
        
        public bool ExecSendDiff(CommandData cmdData)
        {
            //KBCodeReviewHelper.GitInit();
            KBCodeReviewHelper.GitCommit();
            KBCodeReviewHelper.ArcDiff();
            
            return true;
        }

        public bool ExecPushChanges(CommandData cmdData) {

            KBCodeReviewHelper.ArcLand();
            return true;
        }

		public bool ExecOpenFolderKBCodeReview(CommandData cmdData)
		{
			KBCodeReviewHelper.OpenFolderKBCodeReview();
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
