using System;

namespace TestGame.Desktop
{
    public static class Program
    {
        static void Main()
        {
            using (var game = new TestGameProcess())
            {
                game.Run();
            }
        }
    }
}
