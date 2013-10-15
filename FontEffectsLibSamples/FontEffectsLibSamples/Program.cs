using System;

namespace FontEffectsLibSamples
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FontEffectsLibSamples game = new FontEffectsLibSamples())
            {
                game.Run();
            }
        }
    }
#endif
}

