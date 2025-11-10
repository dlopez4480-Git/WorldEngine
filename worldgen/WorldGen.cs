using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace testProgram
{
    public partial class WorldGen
    {
        #region Initial args indexes
        private static readonly int index_seed = 0;
        private static readonly int index_mapSize = 1;
        private static readonly int index_placeholder2 = 2;

        private static readonly int index_placeholder3 = 3;
        private static readonly int index_placeholder4 = 4;

        private static readonly int index_mapType = 5;
        private static readonly int index_placeholder6 = 6;
        private static readonly int index_placeholder7 = 7;


        private static readonly int index_placeholder8 = 8;
        private static readonly int index_placeholder9 = 9;
        private static readonly int index_placeholder10 = 10;
        private static readonly int index_placeholder11 = 11;
        private static readonly int index_placeholder12 = 12;
        private static readonly int index_trueRandom = 13;
        private static readonly int index_placeholder14 = 14;
        private static readonly int index_placeholder15 = 15;
        private static readonly int index_placeholder16 = 16;
        private static readonly int index_placeholder17 = 17;

        #endregion

        public static void testGenerationEarthlike(string[] args)
        {
            #region Parameters for Landform Generation
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            #endregion

            bool debug_printMap = true;


            //  Load custom map
            Bitmap LandMapBitMap = Utility.Images.ImageFile.getBitmap(Utility.Files.GetValidPath("\\debug\\geogeneration\\landgen\\customload\\customMap.png"));
            int[,] LandMap = GeographyGenerator.LandGenerator.createLandMapFromBitmap(LandMapBitMap);
            LandMap = GeographyGenerator.LandGenerator.naturalizeLandmap(args, LandMap);

            if (debug_printMap)
            {

                string filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\landgen\\mapseed.png");
                Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                Utility.Images.ImageFile.saveImage(image, filepath);
            }



            int[,] TemperatureMap = GeographyGenerator.TemperatureGenerator.GenerateTemperatureMap(args, LandMap);

        }

        public static void testGenerationWorld(string[] args)
        {
            #region Parameters for Landform Generation
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            #endregion
            

            
            int[,] LandMap = GeographyGenerator.LandGenerator.createLandMap(args);
            int[,] TemperatureMap = GeographyGenerator.TemperatureGenerator.GenerateTemperatureMap(args, LandMap);

        }
    }
}
