using System;
using System.Drawing;
using System.Runtime.InteropServices;

using Artech.Architecture.Common.Packages;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Controls;
using Artech.Architecture.UI.Framework.Packages;
using Artech.Common.Properties;
using Artech.Genexus.Common.Objects;
using Artech.Architecture.UI.Framework.Services;

namespace GUG.Packages.KBCodeReview
{
   [Guid("0290bda2-2969-47b4-948a-5a0bb880b85f")]

    enum Tool { Phabricator, GitHub};

    public class Package : AbstractPackageUI {
        public static Guid guid = typeof(Package).GUID;

        public override string Name
        {
            get { return "KBCodeReview"; }
        }

      public override void Initialize(IGxServiceProvider services)
      {
         base.Initialize(services);
            AddCommandTarget(new CommandManager());
            AddMyProperties();

        }

        public string Phab
        {
            get { return "Phabricator"; }

        }

        enum Tool { Phabricator, GitHub };

        private void AddMyProperties()
        {
            string propDefinitionKey = DefinitionsHelper.GetPropertiesDefinitionKey<Artech.Architecture.Common.Objects.KBModel>();
            PropertiesDefinition ModelProperties = new PropertiesDefinition(propDefinitionKey);
            PropDefinition CRTool = new PropDefinition("Code Review Tool", typeof(Tool), null, "Tool for code review");
            ModelProperties.Add(CRTool);
            PropDefinition GitRepo = new PropDefinition("Git remote server", typeof(string), null, "path");
            ModelProperties.Add(GitRepo);
            PropDefinition GitUser = new PropDefinition("Git UserName", typeof(string), null, "path");
            ModelProperties.Add(GitUser);
            PropDefinition GitPwd = new PropDefinition("Git Password", typeof(string), null, "path");
            ModelProperties.Add(GitPwd);
            base.AddPropertiesDefinition(propDefinitionKey, ModelProperties);

        }

    }
}
