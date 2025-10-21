using System.Diagnostics;
using System.Diagnostics.Metrics;
using Utility;

namespace testProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Stopwatch Start
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Code Begun");
            Console.ForegroundColor = ConsoleColor.White;
            #endregion

            

            testWorldgen();
            //testWorldPacking();



            #region Stopwatch End
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("RunTime " + elapsedTime);
            #endregion
        }

        

        static void testWorldgen()
        {
            /** Seeds used
                * 41525353
                * 34567890
                * 69696969
                * 99999999
                * 56866774 (Good looking continents)
                * 
                * Mostly land: 95651631
                * 
                 
                */
            Random rand = new Random();
            String randseed = Convert.ToString(rand.Next(-999999, 9999999));

            //randseed = "5265958";


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();




            /** Available Map Sizes
             *  "VERY_SMALL"    128  x 512     
             *  "SMALL"         256  x 1024
             *  "MEDIUM"        512  x 2048
             *  "LARGE"         1024 x 4096
             *  "VERY_LARGE"    2048 x 8192
             * 
             * **/

            String[] args = {
                randseed,       //  00 Seed: used for randomization
                "VERY_SMALL",   //  01 Rows
                "NULL",         //  02 Cols
                "NULL",         //  03 Placeholder
                "NULL",         //  04 Placeholder
                "CONTINENTS",   //  05 MapType
                "NULL",         //  06 Placeholder
                "NULL",         //  07 Placeholder
                "NULL",         //  08 Placeholder
                "NULL",         //  09 Placeholder
                "NULL",         //  10 Placeholder
                "NULL",         //  11 Placeholder
                "NULL",         //  12 Placeholder
                "NULL",         //  13 Placeholder
                "NULL",         //  14 Placeholder
                "NULL",         //  15 Placeholder
                "NULL",         //  16 Placeholder
                "NULL",         //  17 Placeholder
                "NULL",         //  18 Placeholder

            
            };

            WorldGen.testGenerationWorld(args);




            //  restest
            int testnumber = 0;
            Boolean testable = false;
            if (testable)
            {
                for (int i = 0; i < testnumber; i++)
                {
                    randseed = Convert.ToString(rand.Next(0, 99999999));


                    args[0] = randseed;
                    Console.WriteLine();
                    Console.WriteLine();
                    WorldGen.testGenerationWorld(args);
                    Console.WriteLine();
                    Console.WriteLine();

                }
            }




            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }


    }
}
