using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("3DashModMenu")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("art0007i")]
[assembly: AssemblyProduct("3DashModMenu")]
[assembly: AssemblyCopyright("Copyright © art0007i 2023")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("7f13c686-46b8-43c8-93dc-04d7b60e2c84")]
[assembly: AssemblyFileVersion("1.2.0")]
[assembly: AssemblyVersion("1.2.0")]
#if MELON
[assembly: MelonLoader.MelonInfo(typeof(_3DashUtils._3DashUtils),
    _3DashUtils._3DashUtils.MODNAME,
    _3DashUtils._3DashUtils.VERSION,
    _3DashUtils._3DashUtils.AUTHOR,
    "https://github.com/art0007i/3DashUtils"
)]
[assembly: MelonLoader.MelonGame("DelugeDrop", "3Dash")]
#endif
