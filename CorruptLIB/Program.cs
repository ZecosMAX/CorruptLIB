using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZecosMAX.Corrupt;

namespace CorruptLIB
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Parser corruptor = new Parser("suckmyPAINIS.png", ImageType.PNG);
            while (true)
            {
                Console.WriteLine(1 % 50);

                List<Chunk> res = (List<Chunk>)corruptor.ParsePNG();
                for (int i = 0; i < res.Count; i++)
                    if (i > 4 & new String(res[i].Type) == "IDAT")
                    {
                        res[i].Slide(2);
                        res[i].RecalcCrc();
                    }
                //res[5].Slide(4);
                //res[5].recalcCrc();
                corruptor.BuildPNG(res.ToArray());

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
