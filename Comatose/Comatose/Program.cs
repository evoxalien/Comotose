using System;

namespace Comatose {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ComatoseGame game = new ComatoseGame())
            {
                game.Run();
            }
        }
    }
#endif
}

