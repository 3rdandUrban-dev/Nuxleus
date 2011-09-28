using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyTitle("Mvp.Xml")]
[assembly: AssemblyDescription("MVP XML Library")]
[assembly: AssemblyVersion("1.2.*")]

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("../../../mvp-xml.snk")]
//[assembly: AssemblyKeyName("")]

#region Security Permissions

[assembly: AllowPartiallyTrustedCallers]

//[assembly: SecurityPermission(SecurityAction.RequestRefuse, UnmanagedCode=true)]

#endregion Security Permissions
