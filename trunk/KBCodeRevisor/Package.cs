using System;
using System.Drawing;
using System.Runtime.InteropServices;

using Artech.Architecture.Common.Packages;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Controls;
using Artech.Architecture.UI.Framework.Packages;

namespace GUG.Packages.KBCodeRevisor
{
   [Guid("fa2c542d-cd46-4df2-9317-bd5899a536eb")]
   public class Package : AbstractPackageUI {
        public static Guid guid = typeof(Package).GUID;

      public override string Name
      {
         get { return "KBCodeRevisor"; }
      }

      public override void Initialize(IGxServiceProvider services)
      {
         base.Initialize(services);
            AddCommandTarget(new CommandManager());
      }

   }
}
