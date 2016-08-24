using System;
using System.Collections.Generic;
using System.Text;
using Artech.Common.Framework.Commands;

namespace GUG.Packages.KBCodeRevisor
{
    static class CommandKeys
    {
        private static CommandKey objectInTextFormat = new CommandKey(Package.guid, "ObjectInTextFormat");
        public static CommandKey ObjectInTextFormat { get { return objectInTextFormat; } }

        private static CommandKey openFolderKBCodeRevisor = new CommandKey(Package.guid, "OpenFolderKBCodeRevisor");
        public static CommandKey OpenFolderKBCodeRevisor { get { return openFolderKBCodeRevisor; } }

        private static CommandKey pushChanges = new CommandKey(Package.guid, "PushChanges");
        public static CommandKey PushChanges { get { return pushChanges; } }

        private static CommandKey sendDiff = new CommandKey(Package.guid, "SendDiff");
        public static CommandKey SendDiff { get { return sendDiff; } }

        // Acerca de
        private static CommandKey aboutKBCodeRevisor = new CommandKey(Package.guid, "AboutKBCodeRevisor");
        public static CommandKey AboutKBCodeRevisor { get { return aboutKBCodeRevisor; } }

        private static CommandKey helpKBCodeRevisor = new CommandKey(Package.guid, "HelpKBCodeRevisor");
        public static CommandKey HelpKBCodeRevisor { get { return helpKBCodeRevisor; } }
    }
}
