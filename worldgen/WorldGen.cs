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

        public static void testGenerationWorld(string[] args)
        {
            #region Parameters for Landform Generation
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            #endregion
            bool testScale = false;

            if (testScale)
            {
                int[,] deleteme = GeographyGenerator.LandGenerator.generateLandMassSmall(args, 4);
                int[,] newMap = deleteme;

                // 
                newMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, deleteme, 2);



                //  Mars Size
                newMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, deleteme, 1.78);
                //  Venus Sized
                newMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, deleteme, 2.61);
                //  Earth Sized
                newMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, deleteme, 2.69148);
                // 1048x2048
                newMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, deleteme, 3);
            }

            int[,] contsAndIslesMap =  GeographyGenerator.LandGenerator.generateContinentsAndIslands(args);
            string filepath = Utility.Files.GetValidPath("\\debug\\mapScaling\\mapBeforeScaled_ID" + INITSEED + ".png");
            Bitmap ogmap = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(contsAndIslesMap);
            Utility.Images.ImageFile.saveImage(ogmap, filepath);


            double sizeMultiplier = 2.69148;
            contsAndIslesMap = GeographyGenerator.LandGenerator.scaleLandmapToNewSizeWithoutInterp(args, contsAndIslesMap, sizeMultiplier);
            contsAndIslesMap = GeographyGenerator.LandGenerator.naturalizeLandmap(args, contsAndIslesMap);
            filepath = Utility.Files.GetValidPath("\\debug\\mapScaling\\mapNaturalizedID" + INITSEED + "_" + contsAndIslesMap.GetLength(0) + "x" + contsAndIslesMap.GetLength(1) + "_" + (sizeMultiplier + 1) + ".png");
            Bitmap newmap = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(contsAndIslesMap);
            Utility.Images.ImageFile.saveImage(newmap, filepath);

        }
    }
}
