#if MF 
namespace System.Diagnostics
{
    using MFDebug = Microsoft.SPOT.Debug;

    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void WriteLine(string text)
        {
            MFDebug.Print(text);
        }
    }
}
#endif
