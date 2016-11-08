using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Artech.Architecture.Common.Packages;


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KBCodeReview")]
[assembly: AssemblyDescription("Review and control quality of GeneXus Code")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("GUGMontevideo")]
[assembly: AssemblyProduct("KBCodeReview")]
[assembly: AssemblyCopyright("Copyright 2007-2016")]
[assembly: AssemblyTrademark("GUGMontevideo")]
[assembly: AssemblyCulture("")]

// The following attributes are declarations related to this assembly
// as a GeneXus Package
  [assembly: PackageAttribute(typeof(GUG.Packages.KBCodeReview.Package), IsCore = false, IsUIPackage = true )] 

//[assembly: PackageAttribute(typeof(Artech.Packages.TeamDevClient.Package), IsCore = false, IsUIPackage = false)]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0290bda2-2969-47b4-948a-5a0bb880b85f")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("0.0.2.*")]
[assembly: AssemblyFileVersion("0.0.2")]

//[assembly: PackageAttribute(typeof(Artech.Packages.TeamDevClient.Package), IsCore = false, IsUIPackage = false)]

