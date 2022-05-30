using System.Reflection;

namespace Xbehave.Test.Infrastructure
{
    internal static class AssemblyExtensions
    {
#if NETCOREAPP
        public static string GetFileName(this Assembly assembly) =>
            assembly.Location;
#else
        public static string GetFileName(this Assembly assembly) =>
            assembly.CodeBase.Substring(8).Replace('/', '\\');
#endif
    }
}
