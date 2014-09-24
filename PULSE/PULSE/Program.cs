using System;

namespace PulseGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PulseGame game = new PulseGame())
            {
                game.Run();
            }
        }
    }
#endif
}

