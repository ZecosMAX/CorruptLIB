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
            Random random = new Random();
            Parser parser = new Parser("suckmyPAINIS.png", ImageType.PNG);
            PNGCorruptor corruptor = parser.CorruptorInstance as PNGCorruptor;
            while (true)
            {
                corruptor.Corrupt(PNGCorruptType.Slide, 1, 5, 41);
                //res[5].Slide(4);
                //res[5].recalcCrc();
                parser.BuildPNG();

                Console.Write("Again? "); var r = Console.ReadLine();
                if (r == "n")
                    break;
            }
            int max = 6;
            int min = 0;
            //for (int i = -10 * max; i < 10 * max; i++)
            //{
            //    //for neg (max - Math.Abs(i % max)) % max
            //    //for pos (i % max)
            //    if (i < 0)
            //        Console.WriteLine((max - Math.Abs(i % max)) % max  + " -- " + i);
            //    else
            //        Console.WriteLine(i % max + " -- " + i);
            //}
        }
    }
}
