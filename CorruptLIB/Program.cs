using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZecosMAX.Corrupt;

namespace CorruptLIB
{
    /// <summary>
    /// This Class is only for a test properties
    /// it needs to compile only one file - Corruptor.cs
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //Parser parser = new Parser("file.png", ImageType.PNG);
            //PNGCorruptor corruptor = parser.CorruptorInstance as PNGCorruptor;
            //while (true)
            //{
            //    corruptor.Corrupt(PNGCorruptType.Swap, 0, 0, 3, 0);
            //    parser.BuildPNG(corruptor.Chunks);
            //    Corruptor.Resave("builded.png");
            //    Console.Write("Again? "); var r = Console.ReadLine();
            //    if (r == "n")
            //        break;
            //}

            Parser parser = new Parser("file.jpg", ImageType.JPG);
        }
    }
}
