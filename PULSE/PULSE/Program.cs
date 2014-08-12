using System;

namespace SampleGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PULSEGame game = new PULSEGame())
            {
                game.Run();
            }
        }
    }
#endif
}

