using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static testProgram.WorldGen.GeographyGenerator;

namespace testProgram
{

    //  Currently this program is accustomed to 64x256
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

        
        //  This function generates a representation of the world using WorldTile Objects
        public static void GenerateWorld(string[] args)
        {
            #region Parameters 1
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            //  Loads the requested dimensions
            string MAPSIZE = Convert.ToString(args[index_mapSize]);
            int mapRows;
            int mapCols;
            switch (MAPSIZE)
            {
                case "VERY_SMALL":
                    mapRows = 128;
                    mapCols = 256;
                    break;
                case "SMALL":
                    mapRows = 256;
                    mapCols = 512;
                    break;
                case "MEDIUM":
                    mapRows = 512;
                    mapCols = 1024;
                    break;
                case "LARGE":
                    mapRows = 1024;
                    mapCols = 2048;
                    break;
                case "VERY_LARGE":
                    mapRows = 2048;
                    mapCols = 4096;
                    break;
                default:
                    //  Defaults to Small
                    mapRows = 128;
                    mapCols = 256;
                    break;
            }
            int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

            //  Loads the parameters for land generation
            string MAPTYPE = args[index_mapType];
            #endregion

            bool printMapsAsGenerated = false;
            bool saveGeneratedMaps = true;

            //  Generate the Geography Map 
            int[,] GeographyMap = GeographyGenerator.GenerateGeography(args);
            if (printMapsAsGenerated)
            {
                Console.WriteLine("GENWORLD:    Geography Map");
                GeographyGenerator.PrintGeoMap(args, GeographyMap);
            }

            //  TODO: Generate the Rivers Map
            


            //  Generate the Temperature Map
            int[,] TemperatureMap = WorldGen.BiomeGenerator.Temperature_Generation.generateTemperatureMap(args, GeographyMap);
            if (printMapsAsGenerated)
            {
                Console.WriteLine("GENWORLD:    Temperature Map");
                WorldGen.BiomeGenerator.Temperature_Generation.printTemperatureMap(args, TemperatureMap, GeographyMap, true);
            }


            //  Generate the Hydration Map
            int[,] HydrationMap = WorldGen.BiomeGenerator.Hydration_Generation.generateHydrationMap(args, GeographyMap, TemperatureMap);
            if (printMapsAsGenerated)
            {
                Console.WriteLine("GENWORLD:    Hydration Map");
                WorldGen.BiomeGenerator.Hydration_Generation.printHydrationMap(args, GeographyMap, HydrationMap);
            }




            if (saveGeneratedMaps) {
                
                //  Save image map of the worldmap
                string[,] imageMapLandmass = new string[GeographyMap.GetLength(0), GeographyMap.GetLength(1)];
                for (int i = 0; i < GeographyMap.GetLength(0); i++) {
                    for (int j = 0; j < GeographyMap.GetLength(1); j++) {
                        
                        if (GeographyMap[i, j] == landCode_coastalLand)
                        {
                            imageMapLandmass[i, j] = "E3E38F";
                        }
                        else if (GeographyMap[i, j] == landCode_Land)
                        {
                            imageMapLandmass[i, j] = "00A614";
                        }
                        else if (GeographyMap[i, j] == landCode_hillLand)
                        {
                            imageMapLandmass[i, j] = "97560C";
                        }
                        else if (GeographyMap[i, j] == landCode_mountain)
                        {
                            imageMapLandmass[i, j] = "A6A6A6";
                        } 
                        else if (GeographyMap[i, j] == landCode_coastalWater)
                        {
                            imageMapLandmass[i, j] = "87AFFF";
                        }
                        else if (GeographyMap[i, j] == landCode_offcoastWater)
                        {
                            imageMapLandmass[i, j] = "0034A3";
                        }
                        else if (GeographyMap[i, j] == landCode_deepWater)
                        {
                            imageMapLandmass[i, j] = "002166";
                        }
                        else if (GeographyMap[i, j] == landCode_outOfBounds)
                        {
                            imageMapLandmass[i, j] = "FF0000";
                        }
                        else
                        {
                            imageMapLandmass[i, j] = "FF00C8";
                        }

                    }
                }
                Bitmap GeographyMapBitmap = Utility.Images.ImageFile.ArrayToBitmap(imageMapLandmass);
                Utility.Images.ImageFile.saveImage(GeographyMapBitmap, "\\displayableMaps\\testworld_landtype.png");



                //  Save image map of the temperature map
                string[,] imageMapTemperature = new string[TemperatureMap.GetLength(0), TemperatureMap.GetLength(1)];
                for (int i = 0; i < TemperatureMap.GetLength(0); i++)
                {
                    for (int j = 0; j < TemperatureMap.GetLength(1); j++)
                    {
                        if (GeographyMap[i, j] >= landCode_coastalLand)
                        {
                            //  Polar
                            if (TemperatureMap[i, j] < temperatureCode_polarLimit)
                            {
                                imageMapTemperature[i, j] = "210082";
                            }
                            //  Subpolar
                            else if (TemperatureMap[i, j] < temperatureCode_subpolarLimit)
                            {
                                imageMapTemperature[i, j] = "0419BA";  
                            }
                            //  Boreal
                            else if (TemperatureMap[i, j] >= temperatureCode_subpolarLimit && TemperatureMap[i, j] < temperatureCode_borealLimit)
                            {
                                imageMapTemperature[i, j] = "0cd0ff";
                            }
                            //  Cool Temperate
                            else if (TemperatureMap[i, j] >= temperatureCode_borealLimit && TemperatureMap[i, j] < temperatureCode_coolTemperateLimit)
                            {
                                imageMapTemperature[i, j] = "267808";
                            }
                            //  Warm Temperate
                            else if (TemperatureMap[i, j] >= temperatureCode_coolTemperateLimit && TemperatureMap[i, j] < temperatureCode_warmTemperateLimit)
                            {
                                imageMapTemperature[i, j] = "98e601";
                            }
                            //  Sub Tropical
                            else if (TemperatureMap[i, j] >= temperatureCode_warmTemperateLimit && TemperatureMap[i, j] < temperatureCode_tropicalLimit)
                            {
                                imageMapTemperature[i, j] = "ffaa01";
                            }
                            //  Scorching
                            else if (TemperatureMap[i, j] >= temperatureCode_tropicalLimit)
                            {
                                imageMapTemperature[i, j] = "ff5302";
                            }
                        }
                        else
                        {
                            //  Polar
                            if (TemperatureMap[i, j] < temperatureCode_polarLimit)
                            {
                                imageMapTemperature[i, j] = "262626";
                            }
                            //  SubPolar
                            if (TemperatureMap[i, j] < temperatureCode_subpolarLimit)
                            {
                                imageMapTemperature[i, j] = "424242";
                            }
                            //  Boreal
                            else if (TemperatureMap[i, j] >= temperatureCode_subpolarLimit && TemperatureMap[i, j] < temperatureCode_borealLimit)
                            {
                                imageMapTemperature[i, j] = "525252";
                            }
                            //  Cool Temperate
                            else if (TemperatureMap[i, j] >= temperatureCode_borealLimit && TemperatureMap[i, j] < temperatureCode_coolTemperateLimit)
                            {
                                imageMapTemperature[i, j] = "696969";
                            }
                            //  Warm Temperate
                            else if (TemperatureMap[i, j] >= temperatureCode_coolTemperateLimit && TemperatureMap[i, j] < temperatureCode_warmTemperateLimit)
                            {
                                imageMapTemperature[i, j] = "#808080";
                            }
                            //  Sub Tropical
                            else if (TemperatureMap[i, j] >= temperatureCode_warmTemperateLimit && TemperatureMap[i, j] < temperatureCode_tropicalLimit)
                            {
                                imageMapTemperature[i, j] = "969696";
                            }
                            //  Scorching
                            else if (TemperatureMap[i, j] >= temperatureCode_tropicalLimit)
                            {
                                imageMapTemperature[i, j] = "#ADADAD";
                            }
                        }
                    }
                }
                Bitmap TemperatureMapBitmap = Utility.Images.ImageFile.ArrayToBitmap(imageMapTemperature);
                Utility.Images.ImageFile.saveImage(TemperatureMapBitmap, "\\displayableMaps\\testworld_temperature.png");



                //  Save image of the hydration map
                string[,] imageMapHydration = new string[HydrationMap.GetLength(0), HydrationMap.GetLength(1)];
                for (int i = 0; i < HydrationMap.GetLength(0); i++)
                {
                    for (int j = 0; j < HydrationMap.GetLength(1); j++)
                    {

                        if (HydrationMap[i, j] == hydCode_LandNoHydration)
                        {
                            imageMapHydration[i, j] = "BABABA";
                        }
                        else if (HydrationMap[i, j] == hydCode_OceanWater)
                        {
                            imageMapHydration[i, j] = "6164FF";
                        }
                        else if (HydrationMap[i, j] == hydCode_Freshwater)
                        {
                            imageMapHydration[i, j] = "00D2E3";
                        }


                        else if (HydrationMap[i, j] == hydCode_SuperArid)
                        {
                            imageMapHydration[i, j] = "F58884";
                        }
                        else if (HydrationMap[i, j] == hydCode_Arid)
                        {
                            imageMapHydration[i, j] = "F5C084";
                        }
                        else if (HydrationMap[i, j] == hydCode_SemiArid)
                        {
                            imageMapHydration[i, j] = "F1F584";
                        }


                        else if (HydrationMap[i, j] == hydCode_SemiHumid)
                        {
                            imageMapHydration[i, j] = "#D1C546";
                        }
                        else if (HydrationMap[i, j] == hydCode_Humid)
                        {
                            imageMapHydration[i, j] = "97D146";
                        }
                        else if (HydrationMap[i, j] == hydCode_SuperHumid)
                        {
                            imageMapHydration[i, j] = "52D146";
                        }

                        else
                        {
                            imageMapHydration[i, j] = "FF05F0";
                        }
                    }
                }
                Bitmap HydrationMapBitmap = Utility.Images.ImageFile.ArrayToBitmap(imageMapHydration);
                Utility.Images.ImageFile.saveImage(HydrationMapBitmap, "\\displayableMaps\\testworld_hydration.png");



            }
        }

        public static void testGenerationWorld(string[] args)
        {
            #region Parameters 1
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            //  Loads the requested dimensions
            string MAPSIZE = Convert.ToString(args[index_mapSize]);
            int mapRows;
            int mapCols;
            switch (MAPSIZE)
            {
                case "VERY_SMALL":
                    mapRows = 128;
                    mapCols = 256;
                    break;
                case "SMALL":
                    mapRows = 256;
                    mapCols = 512;
                    break;
                case "MEDIUM":
                    mapRows = 512;
                    mapCols = 1024;
                    break;
                case "LARGE":
                    mapRows = 1024;
                    mapCols = 2048;
                    break;
                case "VERY_LARGE":
                    mapRows = 2048;
                    mapCols = 4096;
                    break;
                default:
                    //  Defaults to Small
                    mapRows = 128;
                    mapCols = 256;
                    break;
            }
            int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

            //  Loads the parameters for land generation
            string MAPTYPE = args[index_mapType];
            #endregion


            #region Display Info
            Console.WriteLine("");
            Console.WriteLine();
            //  Generate a world according to the init args
            Console.WriteLine("         Parameters          ");
            Console.WriteLine("SEED:                " + INITSEED);
            Console.WriteLine("");
            Console.WriteLine("MAPSIZE:             " + MAPSIZE);
            Console.WriteLine("Rows:                " + mapRows);
            Console.WriteLine("Cols:                " + mapCols);
            Console.WriteLine("MapSize Modifier:    " + mapsizeModifier);
            Console.WriteLine("                                 ");

            Console.WriteLine("MAPTYPE:             " + MAPTYPE);
            Console.WriteLine("");

            Console.WriteLine("                                 ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-");
            Console.ForegroundColor = ConsoleColor.White;
            #endregion

            GenerateWorld(args);

        }



    }
} 
