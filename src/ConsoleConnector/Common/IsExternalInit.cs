// Polyfill: enables C# 9+ records and `init` accessors on .NET Framework 4.8.
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif
