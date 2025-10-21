using Utility;

namespace testProgram
{

    public partial class WorldGen
    {
        #region Temperature Codes
        public static readonly int temperatureCode_polarLimit = 15;
        public static readonly int temperatureCode_subpolarLimit = 32;
        public static readonly int temperatureCode_borealLimit = 48;
        public static readonly int temperatureCode_coolTemperateLimit = 64;
        public static readonly int temperatureCode_warmTemperateLimit = 80;
        public static readonly int temperatureCode_tropicalLimit = 96;
        #endregion

        #region Hydration Codes
        public static readonly int hydCode_OceanWater = 0;
        public static readonly int hydCode_Freshwater = 1;
        public static readonly int hydCode_LandNoHydration = 2;

        public static readonly int hydCode_SuperArid = 3;
        public static readonly int hydCode_Arid = 4;
        public static readonly int hydCode_SemiArid = 5;
        public static readonly int hydCode_SemiHumid = 7;
        public static readonly int hydCode_Humid = 8;
        public static readonly int hydCode_SuperHumid = 9;

        #endregion


        //  This section is responsible for generating biomes, climates, etc
        public class BiomeGenerator
        {
            public class Temperature_Generation
            {
                public static void printTemperatureMap(string[] args, int[,] temperatureMap, int[,] worldMap, bool displayLandOnly)
                {
                    List<PrintableCell> landCodeCellsKey = new List<PrintableCell>();

                    for (int i = 0; i < temperatureMap.GetLength(0); i++)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(">|");
                        for (int j = 0; j < temperatureMap.GetLength(1); j++)
                        {
                            if (worldMap[i, j] >= landCode_coastalLand)
                            {
                                //  Polar
                                if (temperatureMap[i, j] < temperatureCode_subpolarLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#0419BA");
                                }
                                //  Boreal
                                else if (temperatureMap[i, j] >= temperatureCode_subpolarLimit && temperatureMap[i, j] < temperatureCode_borealLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#0cd0ff");
                                }
                                //  Cool Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_borealLimit && temperatureMap[i, j] < temperatureCode_coolTemperateLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#267808");
                                }
                                //  Warm Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_coolTemperateLimit && temperatureMap[i, j] < temperatureCode_warmTemperateLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#98e601");
                                }
                                //  Sub Tropical
                                else if (temperatureMap[i, j] >= temperatureCode_warmTemperateLimit && temperatureMap[i, j] < temperatureCode_tropicalLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#ffaa01");
                                }
                                //  Scorching
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalLimit)
                                {
                                    Utility.Print.Write(" ", "#000000", "#ff5302");
                                }

                            }
                            else
                            {
                                if (displayLandOnly)
                                {
                                    Console.Write(" ");
                                }
                                else
                                {
                                    //  Polar
                                    if (temperatureMap[i, j] < temperatureCode_subpolarLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#0419BA");
                                    }
                                    //  Boreal
                                    else if (temperatureMap[i, j] >= temperatureCode_subpolarLimit && temperatureMap[i, j] < temperatureCode_borealLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#0cd0ff");
                                    }
                                    //  Cool Temperate
                                    else if (temperatureMap[i, j] >= temperatureCode_borealLimit && temperatureMap[i, j] < temperatureCode_coolTemperateLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#267808");
                                    }
                                    //  Warm Temperate
                                    else if (temperatureMap[i, j] >= temperatureCode_coolTemperateLimit && temperatureMap[i, j] < temperatureCode_warmTemperateLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#98e601");
                                    }
                                    //  Sub Tropical
                                    else if (temperatureMap[i, j] >= temperatureCode_warmTemperateLimit && temperatureMap[i, j] < temperatureCode_tropicalLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#ffaa01");
                                    }
                                    //  Scorching
                                    else if (temperatureMap[i, j] >= temperatureCode_tropicalLimit)
                                    {
                                        Utility.Print.Write(" ", "#000000", "#ff5302");
                                    }
                                }

                            }



                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("|<");
                        Console.WriteLine("");

                    }




                }

                public static int[,] generateTemperatureMap(string[] args, int[,] worldMapID)
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

                    //  Generate a semiRandom temperature gradient between 0 and 100
                    int randomSeed = random.Next(-9999, 9999);
                    int minValTemperature = -10;
                    int maxValTemperature = 95;
                    double waveval = 5.5;

                    int[,] temperatureMap = Utility.Noise.Gradients.GenerateGradientNoise(worldMapID.GetLength(0), worldMapID.GetLength(1), minValTemperature, maxValTemperature, false, random.Next(-999789, 779999), waveval);


                    //  Create influencable array complement, and influence the array
                    double[,] landDistortion = new double[worldMapID.GetLength(0), worldMapID.GetLength(1)];
                    for (int i = 0; i < landDistortion.GetLength(0); i++)
                    {
                        for (int j = 0; j < landDistortion.GetLength(1); j++)
                        {
                            landDistortion[i, j] = 1.0;

                            //  Checks if it is on or near a mountain tile; mountains tend to be colder
                            if (worldMapID[i, j] == landCode_mountain)
                            {
                                List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 128);
                                foreach (Coords coord in circCoords)
                                {
                                    landDistortion[coord.x, coord.y] -= 3;
                                }
                            }

                            //  Checks if it is on or near a hill tile; hill tend to be colder
                            if (worldMapID[i, j] == landCode_hillLand)
                            {
                                List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 128);
                                foreach (Coords coord in circCoords)
                                {
                                    landDistortion[coord.x, coord.y] -= 0.75;
                                }
                            }
                            
                            //  Checks if it is near a water tile: water tiles tend to moderate heat
                            //  TODO: Fix this so that it counts amount of water tiles, then applies it as a buff/debuff
                            if (worldMapID[i, j] == landCode_deepWater || worldMapID[i, j] == landCode_offcoastWater || worldMapID[i, j] == landCode_coastalWater)
                            {
                                List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 64);
                                foreach (Coords coord in circCoords)
                                {
                                    //  If hotter, cool it down
                                    if (temperatureMap[coord.x, coord.y] > 60)
                                    {
                                        landDistortion[coord.x, coord.y] -= 0.50;

                                        //  TODO: issue with there being wierd variances in temperature. Remember, water makes temperatures more towards the norm
                                        //  If the offset would go below 0, set it to 0
                                        if (landDistortion[coord.x, coord.y] < 0)
                                        {
                                            landDistortion[coord.x, coord.y] = 0;
                                        }
                                        
                                    }
                                    //  If colder, warm it up
                                    else if (temperatureMap[coord.x, coord.y] < 50)
                                    {
                                        
                                        landDistortion[coord.x, coord.y] += 0.50;
                                        //  If the offset would go above 0, set it to 0
                                        if (landDistortion[coord.x, coord.y] > 0)
                                        {
                                            landDistortion[coord.x, coord.y] = 0;
                                        }
                                    }
                                    
                                }
                            }

                        }

                    }
                    temperatureMap = Utility.Matrices.Misc.InfluenceArray(landDistortion, temperatureMap, mapsizeModifier / 128);


                    //  Reinforce temperature
                    landDistortion = new double[worldMapID.GetLength(0), worldMapID.GetLength(1)];
                    List<List<Coords>> listOfHots = Utility.Matrices.Selection.SelectionSectionEdges(temperatureMap, 90, 999999, mapsizeModifier / 64);
                    foreach (List<Coords> centercoordList in listOfHots)
                    {
                        foreach (Coords centercoord in centercoordList)
                        {
                            List<Coords> CIRcoordsList = Utility.Matrices.Selection.SelectCircleRegion(temperatureMap, centercoord, mapsizeModifier / 128);
                            foreach (Coords circentercoord in CIRcoordsList)
                            {
                                landDistortion[circentercoord.x, circentercoord.y] += 0.25;
                            }
                            landDistortion[centercoord.x, centercoord.y] += 3;
                        }

                    }
                    temperatureMap = Utility.Matrices.Misc.InfluenceArray(landDistortion, temperatureMap, mapsizeModifier / 128);


                    return temperatureMap;
                }
            }

            public class Hydration_Generation
            {
                public static void printHydrationMap(string[] args, int[,] worldMap, int[,] hydrationMap)
                {

                    for (int i = 0; i < hydrationMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < hydrationMap.GetLength(1); j++)
                        {
                            if (hydrationMap[i, j] == hydCode_LandNoHydration)
                            {
                                Utility.Print.Write("-", "BFBFBF", "BFBFBF");
                            }
                            else if (hydrationMap[i, j] == hydCode_OceanWater)
                            {
                                Utility.Print.Write(" ", "5C5C7D", "5C5C7D");
                            }
                            else if (hydrationMap[i, j] == hydCode_Freshwater)
                            {
                                Utility.Print.Write(" ", "0068E3", "0068E3");
                            }



                            else if (hydrationMap[i, j] == hydCode_SuperArid)
                            {
                                Utility.Print.Write(" ", "#f80104", "#f80104");
                            }

                            else if (hydrationMap[i, j] == hydCode_Arid)
                            {
                                Utility.Print.Write(" ", "#e2b22c", "#e2b22c");
                            }

                            else if (hydrationMap[i, j] == hydCode_Humid)
                            {
                                Utility.Print.Write(" ", "#5e8300", "#5e8300");
                            }

                            else if (hydrationMap[i, j] == hydCode_SuperHumid)
                            {
                                Utility.Print.Write(" ", "#013f00", "#013f00");
                            }

                            else
                            {
                                Utility.Print.Write("X", "FF00FF", "000000");
                            }
                        }
                        Console.WriteLine();
                    }

                }

                public static int[,] generateHydrationMap(string[] args, int[,] geoMap, int[,] temperatureMap)
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

                    int[,] hydrationMap = new int[mapRows, mapCols];

                    //  Default set everything as ocean water
                    for (int i = 0; i < hydrationMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < hydrationMap.GetLength(1); j++)
                        {
                            hydrationMap[i, j] = hydCode_OceanWater;
                        }
                    }

                    //  Next, set all freshwater tiles as freshwater hydration
                    List<List<Coords>> freshwaterLists = Utility.Matrices.Selection.SelectSections(geoMap, -9999, landCode_coastalWater, true, true);
                    Utility.Lists.RemoveListAboveSize(freshwaterLists, mapsizeModifier / 4);
                    foreach (List<Coords> coordsList in freshwaterLists)
                    {
                        foreach (Coords coord in coordsList)
                        {
                            hydrationMap[coord.x, coord.y] = hydCode_Freshwater;
                        }
                    }


                    // Set all land to hydCode_SemiArid
                    List<List<Coords>> landCoords = Utility.Matrices.Selection.SelectSections(geoMap, landCode_coastalLand, 9999, false, false);
                    foreach (List<Coords> list in landCoords)
                    {
                        foreach (Coords coord in list)
                        {
                            hydrationMap[coord.x, coord.y] = hydCode_SemiArid;
                        }
                    }



                    //  Set all tiles next to oceans as hydrated
                    List<List<Coords>> nextToOcean = Utility.Matrices.Selection.SelectionSectionEdges(hydrationMap, hydCode_OceanWater, hydCode_OceanWater, mapsizeModifier / 64);
                    foreach (List<Coords> coordinates in nextToOcean)
                    {
                        foreach (Coords coords in coordinates)
                        {
                            if (geoMap[coords.x, coords.y] >= landCode_coastalLand)
                            {

                                hydrationMap[coords.x, coords.y] = hydCode_SemiHumid;
                            }


                        }
                    }

                    

                    //  Set all tiles next to lakes as hydrated
                    List<List<Coords>> nextToFresh = Utility.Matrices.Selection.SelectionSectionEdges(hydrationMap, hydCode_Freshwater, hydCode_Freshwater, mapsizeModifier / 64);
                    foreach (List<Coords> coordinates in nextToFresh)
                    {
                        foreach (Coords coords in coordinates)
                        {
                            if (geoMap[coords.x, coords.y] >= landCode_coastalLand)
                            {
                                hydrationMap[coords.x, coords.y] = hydCode_Humid;

                            }

                        }
                    }



                    //  Deal with extremis
                    for (int i = 0; i < hydrationMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < hydrationMap.GetLength(1); j++)
                        {
                            if (hydrationMap[i, j] >= hydCode_SuperHumid)
                            {
                                hydrationMap[i, j] = hydCode_SuperHumid;
                            }

                            
                        }
                    }

                    return hydrationMap;
                }


            }



        }



    }
}
