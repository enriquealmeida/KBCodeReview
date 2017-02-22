using System;
using System.Collections.Generic;
using System.Text;
using Artech.Common.Framework.Commands;

namespace GUG.Packages.KBCodeReview
{
    static class CommandKeys
    {
        private static CommandKey objectInTextFormat = new CommandKey(Package.guid, "ObjectInTextFormat");
        public static CommandKey ObjectInTextFormat { get { return objectInTextFormat; } }

        private static CommandKey openFolderKBCodeReview = new CommandKey(Package.guid, "OpenFolderKBCodeReview");
        public static CommandKey OpenFolderKBCodeReview { get { return openFolderKBCodeReview; } }

        private static CommandKey clone = new CommandKey(Package.guid, "Clone");
        public static CommandKey Clone { get { return clone; } }

        private static CommandKey pushChanges = new CommandKey(Package.guid, "PushChanges");
        public static CommandKey PushChanges { get { return pushChanges; } }

        private static CommandKey sendDiff = new CommandKey(Package.guid, "SendDiff");
        public static CommandKey SendDiff { get { return sendDiff; } }

        // Acerca de
        private static CommandKey aboutKBCodeReview = new CommandKey(Package.guid, "AboutKBCodeReview");
        public static CommandKey AboutKBCodeReview { get { return aboutKBCodeReview; } }

        private static CommandKey helpKBCodeReview = new CommandKey(Package.guid, "HelpKBCodeReview");
        public static CommandKey HelpKBCodeReview { get { return helpKBCodeReview; } }
    }
}
