using System;

namespace WorldEditor.Desktop
{
    public static class Program
    {
        static void Main()
        {
            using (var game = new WorldEditorProcess())
            {
                game.Run();
            }
        }
    }
}
