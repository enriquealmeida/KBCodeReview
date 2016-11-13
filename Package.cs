using System;
using System.Runtime.InteropServices;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Packages;
using Artech.Common.Properties;

namespace GUG.Packages.KBCodeReview
{
   

   

    [Guid("0290bda2-2969-47b4-948a-5a0bb880b85f")]
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
            PropertiesDefinition myTrnProperties = new PropertiesDefinition(propDefinitionKey);
            PropDefinition myBool = new PropDefinition("Code Review Tool", typeof(Tool), null, "Tool for code review");
            myTrnProperties.Add(myBool);
            base.AddPropertiesDefinition(propDefinitionKey, myTrnProperties);

        }
      
    }
}
