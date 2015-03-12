using System;

namespace TfsGitAdmin
{
    static class Helpers
    {
        public static void Dump(this Exception e, System.IO.TextWriter console)
        {
            console.Write("Error: ");
            while (e != null)
            {
                console.WriteLine(e.Message);
                e = e.InnerException;
            }//while
        }
    }
}
