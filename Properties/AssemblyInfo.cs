using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ContactManger")]
[assembly: AssemblyDescription("Used to hold Contacts")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Chris Hudson LLC")]
[assembly: AssemblyProduct("ContactManager.Properties")]
[assembly: AssemblyCopyright("2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("660cc8ae-fe0e-4537-b30f-a6c7fbecefa7")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]
// Log4net config file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Configs\\logging.config", Watch = true)]
