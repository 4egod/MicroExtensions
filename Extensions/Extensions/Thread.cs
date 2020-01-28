#if !MF && !TINYCLR
//namespace Extensions
//{
    using System.Threading;

    public class Thread
    {
        public static void Sleep(int millisecondsTimeout)
        {
            using (AutoResetEvent e = new AutoResetEvent(false))
            {
                e.WaitOne(millisecondsTimeout);
            }
        }
    }
//}
#endif
