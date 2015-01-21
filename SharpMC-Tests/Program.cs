using System;
using System.Drawing;

namespace SharpMCTests
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            DrawCircle ();
            Console.ReadKey ();
        }

        private static void DrawCircle()
        {
            Bitmap bmp = new Bitmap(200, 200);

            int r = 13; //Radius. (13*4 = 52) and we need 49 Chunks for a player to spawn. So this should be fine :)
            int ox = 100, oy = 100; //Middle point

            for (int x = -r; x < r ; x++)
            {
                int height = (int)Math.Sqrt(r * r - x * x);

                for (int y = -height; y < height; y++)
                {
                    bmp.SetPixel (x + ox, y + oy, Color.Red);
                }
            }
            bmp.Save("test.bmp");
        }
           

    }
}
