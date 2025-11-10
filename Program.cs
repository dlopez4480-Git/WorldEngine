

namespace testProgram
{

    class Program
    {
        static void Main(string[] args)
        {
            //testWorldGeneration();
            testWorldGenerationEarthlike();
        }



        static void testWorldGeneration()
        {
            Random random = new Random();
            string randseed = Convert.ToString(random.Next(1, 999999999));

            String[] args = {
                randseed,               //  00 Seed: used for randomization
                "MEDIUM",               //  01 Scale: changes map
                "NULL",                 //  02 Placeholder
                "NULL",                 //  03 Placeholder
                "NULL",                 //  04 Placeholder
                "CONTINENTS&ISLANDS",   //  05 MapType
                "NULL",                 //  06 Placeholder
                "NULL",                 //  07 Placeholder
                "NULL",                 //  08 Placeholder
                "NULL",                 //  09 Placeholder
                "NULL",                 //  10 Placeholder
                "NULL",                 //  11 Placeholder
                "NULL",                 //  12 Placeholder
                "NULL",                 //  13 Placeholder
                "NULL",                 //  14 Placeholder
                "NULL",                 //  15 Placeholder
                "NULL",                 //  16 Placeholder
                "NULL",                 //  17 Placeholder
                "NULL",                 //  18 Placeholder

            
            };

            WorldGen.testGenerationWorld(args);



        }


        static void testWorldGenerationEarthlike()
        {
            Random random = new Random();
            string randseed = Convert.ToString(random.Next(1, 999999999));

            String[] args = {
                randseed,               //  00 Seed: used for randomization
                "MEDIUM",               //  01 Scale: changes map
                "NULL",                 //  02 Placeholder
                "NULL",                 //  03 Placeholder
                "NULL",                 //  04 Placeholder
                "CONTINENTS&ISLANDS",   //  05 MapType
                "NULL",                 //  06 Placeholder
                "NULL",                 //  07 Placeholder
                "NULL",                 //  08 Placeholder
                "NULL",                 //  09 Placeholder
                "NULL",                 //  10 Placeholder
                "NULL",                 //  11 Placeholder
                "NULL",                 //  12 Placeholder
                "NULL",                 //  13 Placeholder
                "NULL",                 //  14 Placeholder
                "NULL",                 //  15 Placeholder
                "NULL",                 //  16 Placeholder
                "NULL",                 //  17 Placeholder
                "NULL",                 //  18 Placeholder

            
            };

            WorldGen.testGenerationEarthlike(args);



        }
    }
}
