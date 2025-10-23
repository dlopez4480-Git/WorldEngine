using System.Drawing;
using Utility;

namespace testProgram
{

    public partial class WorldGen
    {
        #region Values
        private static readonly int landgen_lowerbound = 0;
        private static readonly int landgen_upperbound = 63;
        private static readonly int landgen_canvasbound = 4;
        #endregion

        #region Land Codes

        //Land Codes Below Sealine
        private static int landCode_deepWater = 25;
        private static int landCode_offcoastWater = 29;
        private static int landCode_coastalWater = 30;

        //Land Codes Above Sealine
        private static readonly int landCode_outOfBounds = 15;
        private static readonly int landCode_coastalLand = 40;
        private static readonly int landCode_Land = 41;
        private static readonly int landCode_hillLand = 42;
        private static readonly int landCode_mountain = 50;

        #endregion


        //  This section is responsible for generating the raw geography of the map; land, rivers, mountains, oceans, etc.
        public class GeographyGenerator : WorldGen
        {

           



            //  Generate the correct outputs based on the relavent arguements
            public static int[,] GenerateGeography(string[] args)
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


                //  DEBUG Parameters
                bool verbose = true;
                bool printMap = false;
                bool debug = true;


                int contnumber = 1;


                double frequency = 0.15; //  This frequency works best for the size 64x128

                //  Generate the sea floor 

                //  Generate the GeoMap
                int[,] IDMap = new int[mapRows, mapCols];

                switch (MAPTYPE)
                {
                    case "CONTINENTS":
                        int continentSizeRank = 3;
                        IDMap = Landform_Generation.GenerateContinentsAndIslands(args);
                        break;
                    default:
                        break;

                }


                //  Print the final
                if (printMap)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[GENWORLD]   Generating ID Map post processing:");
                    Landform_Generation.PrintLandformMap(args, IDMap);
                    Console.WriteLine("");
                }
                if (verbose)
                {
                    Console.WriteLine("GENWORLD:    Generated Landmasses");
                }
                return IDMap;
            }

            //  Generates individual components for landmasses
            public class Landform_Generation : GeographyGenerator
            {
                //  Given an array in ID Map form, print an ASCII representation of the map
                public static void PrintLandformMap(string[] args, int[,] idMap)
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


                    for (int i = 0; i < idMap.GetLength(0); i++)
                    {
                        Utility.Print.Write(">|", "00A614", "002166");
                        for (int j = 0; j < idMap.GetLength(1); j++)
                        {
                            if (idMap[i, j] == landCode_coastalLand)
                            {
                                Utility.Print.Write("~", "E3E38F", "00A614");
                            }
                            else if (idMap[i, j] == landCode_Land)
                            {
                                Utility.Print.Write("~", "00A614", "00A614");
                            }
                            else if (idMap[i, j] == landCode_hillLand)
                            {
                                Utility.Print.Write("m", "ADADAD", "00A614");
                            }
                            else if (idMap[i, j] == landCode_mountain)
                            {
                                Utility.Print.Write("M", "FFFFFF", "00A614");
                            } // 0052FF
                            else if (idMap[i, j] == landCode_coastalWater)
                            {
                                Utility.Print.Write("`", "87AFFF", "0052FF");
                            }
                            else if (idMap[i, j] == landCode_offcoastWater)
                            {
                                Utility.Print.Write(" ", "0034A3", "0034A3");
                            }
                            else if (idMap[i, j] == landCode_deepWater)
                            {
                                Utility.Print.Write(" ", "002166", "002166");
                            }
                            else if (idMap[i, j] == landCode_outOfBounds)
                            {
                                Utility.Print.Write("X", "FF0000", "002166");
                            }
                            else
                            {
                                Utility.Print.Write("X", "FF00C8", "000000");
                            }

                        }
                        Utility.Print.Write("|<", "00A614", "002166");
                        Console.WriteLine();
                    }

                    Console.WriteLine();
                }

                public static Bitmap createBitmapLandform(string[] args, int[,] landMap)
                {
                    Bitmap[,] landFormMap = new Bitmap[landMap.GetLength(0), landMap.GetLength(1)];
                    string filepath = "";

                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < landMap.GetLength(1); j++)
                        {
                            if (landMap[i, j] == landCode_coastalLand)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\land\\coastal.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_Land)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\land\\land.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_hillLand)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\land\\hills.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_mountain)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\land\\mountain.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_coastalWater)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\water\\deepwater.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_offcoastWater)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\water\\deepwater.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_deepWater)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\water\\deepwater.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else if (landMap[i, j] == landCode_outOfBounds)
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\alt\\outOfBounds.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                            else
                            {
                                filepath = Utility.Files.GetDirectory("\\debug\\displayableMaps\\displayMapsAssets\\landForm\\tile\\alt\\errorUnknown.png");
                                landFormMap[i, j] = Utility.Images.ImageFile.getBitmap(filepath);
                            }
                        }
                    }


                    Bitmap landFormImage = Utility.Images.ImageManipulation.CombineBitmapArray(landFormMap);

                    return landFormImage;
                }



                #region Map Generator Components


                //  Given an int array in ID form, make the coastlines look more natural
                private static int[,] naturalizeCoastlines(string[] args, int[,] array, int chanceCrater, int cycles, bool cleanup)
                {
                    #region Parameters
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
                            mapCols = 512;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 2048;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 4096;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 8192;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion


                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> coastlineArray = new List<List<Coords>>();

                    for (int cyclesCount = 0; cyclesCount < cycles; cyclesCount++)
                    {
                        coastlineArray = Utility.Matrices.Selection.SelectionSectionEdges(array, landCode_coastalLand, 999, 1);

                        foreach (List<Coords> coastlineCoords in coastlineArray)
                        {
                            foreach (Coords coord in coastlineCoords)
                            {
                                //  Generate the probability outwards or inwards
                                int coastlinebulgeProb = random.Next(0, 101);

                                //  Raise Land
                                if (coastlinebulgeProb <= chanceCrater)
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)mapsizeModifier / 64));

                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (array[circoord.x, circoord.y] < landCode_coastalLand)
                                        {
                                            array[circoord.x, circoord.y] = landCode_Land;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)(mapsizeModifier / 64)));
                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (array[circoord.x, circoord.y] >= landCode_coastalLand)
                                        {
                                            array[circoord.x, circoord.y] = landCode_deepWater;
                                        }

                                    }
                                }
                            }
                        }
                    }

                    if (cleanup)
                    {
                        List<List<Coords>> validLands = Utility.Matrices.Selection.SelectSections(array, landCode_coastalLand, 9999999, false, false);
                        Utility.Lists.RemoveListAboveSize(validLands, mapsizeModifier / 32);

                        int[,] newarray = new int[mapRows, mapCols];
                        for (int i = 0; i < newarray.GetLength(0); i++)
                        {
                            for (int j = 0; j < newarray.GetLength(1); j++)
                            {
                                newarray[i, j] = landCode_deepWater;
                            }
                        }

                        foreach (List<Coords> coordslist in validLands)
                        {
                            foreach (Coords coords in coordslist)
                            {
                                newarray[coords.x, coords.y] = landCode_Land;
                            }
                        }
                    }

                    return array;
                }


                //  Given an int array in ID form, make the coastlines look more natural
                public static int[,] naturalizeMountains(string[] args, int[,] array, int chanceCrater, int cycles, bool cleanup)
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


                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> mountainArray = new List<List<Coords>>();

                    //int chanceCrater = 75;


                    for (int cyclesCount = 0; cyclesCount < cycles; cyclesCount++)
                    {
                        mountainArray = Utility.Matrices.Selection.SelectionSectionEdges(array, landCode_mountain, 999, mapsizeModifier / 128);

                        foreach (List<Coords> mountCoords in mountainArray)
                        {
                            foreach (Coords coord in mountCoords)
                            {
                                //  Generate the probability outwards or inwards
                                int coastlinebulgeProb = random.Next(0, 101);

                                //  Raise Land
                                if (coastlinebulgeProb <= chanceCrater)
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)mapsizeModifier / 64));

                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (array[circoord.x, circoord.y] >= landCode_coastalLand && array[circoord.x, circoord.y] < landCode_mountain)
                                        {
                                            array[circoord.x, circoord.y] = landCode_mountain;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)(mapsizeModifier / 64)));
                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (array[circoord.x, circoord.y] == landCode_mountain)
                                        {
                                            array[circoord.x, circoord.y] = landCode_Land;
                                        }

                                    }
                                }
                            }
                        }
                    }


                    if (cleanup)
                    {
                        List<List<Coords>> validLands = Utility.Matrices.Selection.SelectSections(array, landCode_mountain, 9999999, false, false);
                        Utility.Lists.RemoveListAboveSize(validLands, mapsizeModifier / 32);

                        int[,] newarray = new int[mapRows, mapCols];
                        for (int i = 0; i < newarray.GetLength(0); i++)
                        {
                            for (int j = 0; j < newarray.GetLength(1); j++)
                            {
                                newarray[i, j] = landCode_Land;
                            }
                        }

                        foreach (List<Coords> coordslist in validLands)
                        {
                            foreach (Coords coords in coordslist)
                            {
                                newarray[coords.x, coords.y] = landCode_Land;
                            }
                        }
                    }

                    return array;
                }


                //  Generate a simple array consisting of all ocean tiles
                public static int[,] getOceanicArray(string[] args)
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

                    int[,] water2DArray = new int[mapRows, mapCols];
                    for (int i = 0; i < water2DArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < water2DArray.GetLength(1); j++)
                        {
                            water2DArray[i, j] = landCode_deepWater;
                        }
                    }

                    return water2DArray;
                }


                //  Generates a single landmass, of various sizes, without terrain
                public static int[,] GenerateLandmassNoise(string[] args, int continentSize)
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

                    #region DebugParameters
                    bool verbose = true;

                    bool DEBUG_mapRegenerationAlert = false;     // Prints an Alert when the primemap or contmap resets
                    bool errorCountAlert = true;

                    bool DEBUG_printMapsAsGenerated = false;
                    bool DEBUG_printMapWhenAdded = false;

                    #endregion

                    #region Generators
                    //  Generation Parameters

                    double frequency = 2;  //  TODO: Change frequency so that it generates noise better for the size

                    //  Generate Initial Array
                    int landOffset = 38;
                    int[,] PrimeArrayMap = getOceanicArray(args);


                    //  Assign continentSize range
                    int minSizeContinent = 0;
                    int maxSizeContinent = 0;
                    int maximumTileCount = mapRows * mapCols;
                    switch (continentSize)
                    {

                        case 1: //  Small Island (This should be the smallest available land size 
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier / 64));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 1));
                            frequency = 15;
                            break;
                        case 2: //  Medium Sized Island
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 1));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 2.5));
                            break;
                        case 3: //  Large Island
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 4));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 7));
                            break;
                        case 4: //  Smaller sized continent

                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 8));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 16));
                            break;
                        case 5: //  Medium Sized Continent
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                            break;

                        case 6: //This is the theoretical max size
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 28));
                            maxSizeContinent = Convert.ToInt32(mapRows * mapCols);
                            break;

                        case 7:
                            break;


                        default:
                            //  Get Continents of a small size, by default
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                    }


                    //  Dimensions Values
                    int bordermodifierEdgesRow = mapRows / 8;
                    int bordermodifierEdgesCol = mapCols / 8;


                    #endregion

                    //int continentCurrentNumber = 0;
                    int errorIncrementor = 0;
                    int errorLimit = 10;
                    int timesTried = 0;

                regenerateContinents:
                    if (errorIncrementor >= errorLimit)
                    {
                        //  Reset everything
                        errorIncrementor = 0;
                        PrimeArrayMap = getOceanicArray(args);
                        timesTried = 0;
                        goto regenerateContinents;
                    }


                    //  Generate the initial continent noise
                    bool continentGenerator = true;
                    //  TODO: Map looks better if you combine a bunch of continents of the same size onto itself
                    int amountLayers = random.Next(3, 7);

                    while (continentGenerator)
                    {
                        //  Generate the Initial Noise of the continent
                        #region Generate Continent Noise
                        //  Generate a map
                        int genSeed = random.Next(-99999999, 999999999);
                        int[,] continentsMap = Utility.Noise.Perlin.GeneratePerlinInt(PrimeArrayMap.GetLength(0), PrimeArrayMap.GetLength(1), frequency, landgen_lowerbound, landgen_upperbound, genSeed);
                        //  Convert to ID form
                        for (int i = 0; i < continentsMap.GetLength(0); i++)
                        {
                            for (int j = 0; j < continentsMap.GetLength(1); j++)
                            {
                                if (continentsMap[i, j] >= landOffset)
                                {
                                    continentsMap[i, j] = landCode_Land;
                                }
                                else
                                {
                                    continentsMap[i, j] = landCode_deepWater;
                                }
                            }
                        }


                        #region Naturalize the coastlines
                        //  Shave off the edges
                        for (int i = 0; i < continentsMap.GetLength(0); i++)
                        {
                            for (int j = 0; j <= bordermodifierEdgesCol; j++)
                            {
                                continentsMap[i, j] = landCode_deepWater;
                            }
                            for (int j = continentsMap.GetLength(1) - bordermodifierEdgesCol; j < continentsMap.GetLength(1); j++)
                            {
                                continentsMap[i, j] = landCode_deepWater;
                            }

                        }
                        for (int j = 0; j < continentsMap.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                continentsMap[i, j] = landCode_deepWater;
                            }
                            for (int i = continentsMap.GetLength(0) - bordermodifierEdgesRow; i < continentsMap.GetLength(0); i++)
                            {
                                continentsMap[i, j] = landCode_deepWater;
                            }
                        }
                        //  Naturalize coastlines


                        if (continentSize < 4)
                        {
                            int randomChanceCrater = random.Next(15, 45);
                            continentsMap = WorldGen.GeographyGenerator.Landform_Generation.naturalizeCoastlines(args, continentsMap, randomChanceCrater, 7, false);
                        }
                        else
                        {
                            int randomChanceCrater = random.Next(20, 25);
                            continentsMap = WorldGen.GeographyGenerator.Landform_Generation.naturalizeCoastlines(args, continentsMap, randomChanceCrater, 4, false);
                        }


                        //  Shave off the edges and make it seem more natural
                        for (int i = 0; i < continentsMap.GetLength(0); i++)
                        {
                            for (int j = 0; j <= bordermodifierEdgesCol; j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(continentsMap, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        continentsMap[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                continentsMap[i, j] = landCode_deepWater;
                            }
                            for (int j = continentsMap.GetLength(1) - bordermodifierEdgesCol; j < continentsMap.GetLength(1); j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == continentsMap.GetLength(1) - bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(continentsMap, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        continentsMap[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }

                                continentsMap[i, j] = landCode_deepWater;
                            }
                        }
                        for (int j = 0; j < continentsMap.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(continentsMap, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        continentsMap[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                continentsMap[i, j] = landCode_deepWater;
                            }
                            for (int i = continentsMap.GetLength(0) - bordermodifierEdgesRow; i < continentsMap.GetLength(0); i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == continentsMap.GetLength(0) - bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.Selection.SelectCircleRegion(continentsMap, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        continentsMap[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                continentsMap[i, j] = landCode_deepWater;
                            }
                        }

                        //  Fill in potholes
                        List<List<Coords>> potholes = Utility.Matrices.Selection.SelectSections(continentsMap, landCode_coastalLand, 999, false, false);
                        //  Remove all water tiles above a size
                        Utility.Lists.RemoveListAboveSize(potholes, mapsizeModifier / 8);
                        foreach (List<Coords> coordList in potholes)
                        {
                            foreach (Coords pothole in coordList)
                            {
                                continentsMap[pothole.x, pothole.y] = landCode_Land;
                            }
                        }


                        #endregion

                        #region Select out invalid map sizes

                        //  Create a list of all continents below the size limit, then overwrite them on the array
                        List<List<Coords>> TooSmallConts = Utility.Matrices.Selection.SelectSections(continentsMap, landCode_coastalLand, 999999, false, false);
                        Utility.Lists.RemoveListAboveSize(TooSmallConts, minSizeContinent);
                        foreach (List<Coords> listCoords in TooSmallConts)
                        {
                            foreach (Coords coord in listCoords)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }

                        //  Create a list Of all continents above the size limit, then delete them
                        List<List<Coords>> TooLargeConts = Utility.Matrices.Selection.SelectSections(continentsMap, landCode_coastalLand, 999999, false, false);
                        Utility.Lists.RemoveListBelowSize(TooLargeConts, maxSizeContinent);
                        foreach (List<Coords> listCoords in TooLargeConts)
                        {
                            foreach (Coords coord in listCoords)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }
                        #endregion

                        #region Select out map with invalid dimensions
                        //  TODO: This code works for a row-to-col ratio of 1:4. Please modify this so that it works across ratios
                        //  Remove continents wider than they are tall
                        List<List<Coords>> wideContinents = Utility.Matrices.Selection.SelectSections(continentsMap, landCode_coastalLand, 999999, false, false);
                        List<List<Coords>> badConts = new List<List<Coords>>();
                        int rowMinimum = Convert.ToInt32((double)(mapsizeModifier / 2));
                        int colMaximum = Convert.ToInt32((double)(mapsizeModifier / 2));

                        foreach (List<Coords> contLists in wideContinents)
                        {

                            int continentColCount = Utility.Matrices.Selection.MaxColDistance(contLists);
                            int continentRowCount = Utility.Matrices.Selection.MaxRowDistance(contLists);

                            //  Check that these two do not vary signficiantly
                            if (continentColCount > Convert.ToInt32((double)(continentRowCount * (1.10))) || continentColCount < Convert.ToInt32((double)(continentRowCount * (0.90))))
                            {
                                badConts.Add(contLists);
                            }
                            if (continentRowCount > Convert.ToInt32((double)(continentColCount * (1.20))) || continentRowCount < Convert.ToInt32((double)(continentColCount * (0.80))))
                            {
                                //badConts.Add(contLists);
                            }
                        }

                        foreach (List<Coords> badContsList in badConts)
                        {
                            foreach (Coords coord in badContsList)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }

                        #endregion

                        #region Verify there are some valid continents remaining
                        //  Check that ANY continents exist
                        List<List<Coords>> validContinents = Utility.Matrices.Selection.SelectSections(continentsMap, landCode_coastalLand, 999999, false, false);
                        if (validContinents.Count() < 1)
                        {
                            if (DEBUG_mapRegenerationAlert)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No Valid Continents: regenerating the whole map");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            goto regenerateContinents;
                        }

                        #endregion

                        //  if DEBUG_printMapsAsGenerated is true, print the current Map
                        if (DEBUG_printMapsAsGenerated)
                        {
                            //  Print maps with valid continents highlighted in bright yellow
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            for (int i = 0; i < PrimeArrayMap.GetLength(0); i++)
                            {

                                Console.Write(">|");
                                for (int j = 0; j < PrimeArrayMap.GetLength(1); j++)
                                {
                                    if (continentsMap[i, j] == landCode_deepWater)
                                    {
                                        Console.Write(" ");
                                    }
                                    else if (continentsMap[i, j] == landCode_Land)
                                    {
                                        Console.Write("+");
                                    }
                                    else if (continentsMap[i, j] == landCode_outOfBounds)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.Write("-");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write("?");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    }



                                }
                                Console.Write("|<");
                                Console.WriteLine();
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                        }

                        #region Assign the continent value to a random position
                        //  Select a random valid continent
                        List<Coords> randomContinent = new List<Coords>();
                        int randomNumContinent = random.Next(0, validContinents.Count());
                        randomContinent = validContinents[randomNumContinent];

                        //  Get the center of the map
                        int middleRow = Convert.ToInt32((double)PrimeArrayMap.GetLength(0) / 2);
                        int middleCol = Convert.ToInt32((double)PrimeArrayMap.GetLength(1) / 2);
                        Coords centralCoords = new Coords(middleRow, middleCol);

                        //  Place this at the center of the map
                        List<Coords> recentered = Utility.Matrices.Transformation.TranslateSection(randomContinent, mapRows, mapCols, centralCoords, true);

                        //  Possibly modify this to be rotated
                        List<Coords> rotated = recentered;
                        int rotatedChance = random.Next(0, 101);
                        if (rotatedChance > 25)
                        {
                            rotated = Utility.Matrices.Transformation.RotateSection(rotated, mapRows, mapCols, 1);
                        }

                        //  Possibly modify this to be flipped



                        foreach (Coords coordinates in rotated)
                        {
                            PrimeArrayMap[coordinates.x, coordinates.y] = landCode_Land;
                        }
                        timesTried++;
                        #endregion





                        //  Print the current canvasMap
                        if (timesTried > amountLayers)
                        {
                            continentGenerator = false;
                        }

                        #endregion
                    }

                    int lakesIterator = 4;
                    for (int lakeCount = 0; lakeCount < lakesIterator; lakeCount++)
                    {
                        //  Generate lakes
                        double freqWaters = 15;
                        int floor = 10;
                        int[,] generateLakeLandNoise = Utility.Noise.Perlin.GeneratePerlinInt(PrimeArrayMap.GetLength(0), PrimeArrayMap.GetLength(1), freqWaters, 0, 64, random.Next(-99999, 99999));
                        int[,] generateLakeLandNoiseInverse = generateLakeLandNoise;

                        //  Invert lakes to lands
                        for (int i = 0; i < generateLakeLandNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j < generateLakeLandNoise.GetLength(1); j++)
                            {
                                if (PrimeArrayMap[i, j] < floor)
                                {
                                    generateLakeLandNoiseInverse[i, j] = landCode_Land;
                                }
                                else
                                {
                                    generateLakeLandNoiseInverse[i, j] = landCode_deepWater;
                                }
                            }
                        }
                        // Naturalize borders
                        generateLakeLandNoiseInverse = naturalizeCoastlines(args, generateLakeLandNoiseInverse, 65, 5, false);
                        List<List<Coords>> invCoord = Utility.Matrices.Selection.SelectSections(generateLakeLandNoiseInverse, landCode_Land, landCode_Land);
                        List<Coords> invCoordList = Utility.Lists.CollapseLists(invCoord);
                        foreach (Coords coordinates in invCoordList)
                        {
                            generateLakeLandNoise[coordinates.x, coordinates.y] = 0;
                        }
                        //Utility.Print.MatrixPrint.DisplayPerlin(generateLakeLandNoise, floor);

                        List<List<Coords>> lakesCoords = Utility.Matrices.Selection.SelectSections(generateLakeLandNoise, 0, floor);

                        foreach (List<Coords> lakes in lakesCoords)
                        {
                            //  Verify there are no water tiles nearby
                            int[,] getEdgesArr = Utility.Matrices.Misc.ConvertCoordstoArray(lakes, PrimeArrayMap.GetLength(0), PrimeArrayMap.GetLength(1));
                            List<List<Coords>> edgesLoL = Utility.Matrices.Selection.SelectionSectionEdges(getEdgesArr, 1, 1, mapsizeModifier / 32);
                            List<Coords> edges = Utility.Lists.CollapseLists(edgesLoL);
                            bool isByWater = false;
                            foreach (Coords edge in edges)
                            {
                                if (PrimeArrayMap[edge.x, edge.y] < landCode_coastalLand)
                                {
                                    isByWater = true;
                                }
                            }

                            if (!isByWater)
                            {
                                foreach (Coords lakeCoord in lakes)
                                {
                                    PrimeArrayMap[lakeCoord.x, lakeCoord.y] = landCode_deepWater;
                                }
                            }




                        }
                    }

                    //  Perform more detailed continent modification
                    //  Base convert into a valid map
                    for (int i = 0; i < PrimeArrayMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < PrimeArrayMap.GetLength(1); j++)
                        {
                            if (PrimeArrayMap[i, j] < landCode_coastalLand)
                            {
                                PrimeArrayMap[i, j] = landCode_deepWater;
                            }
                            else
                            {
                                PrimeArrayMap[i, j] = landCode_Land;
                            }
                        }
                    }


                    return PrimeArrayMap;
                }







                //  Given a land mass, convert it to be more natural
                public static int[,] TurnLandMassIntoNatural(string[] args, int[,] worldMap)
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

                    bool debug_Printmap = true;

                    int[,] naturalMap = worldMap;
                    List<List<Coords>> landCoords = Utility.Matrices.Selection.SelectSections(worldMap, landCode_coastalLand, 999, false, false);


                    //  Remove lists below a certain size (i.e small Islands)
                    Utility.Lists.RemoveListBelowSize(landCoords, Convert.ToInt32(((double)mapsizeModifier / 8)));

                    //  Create the seeds
                    List<Coords> seedsForVoronoi = new List<Coords>();
                    foreach (List<Coords> landCoordList in landCoords)
                    {

                        int sizeOfIsland = landCoordList.Count();
                        int sizeOfStartingCoords = 3;

                        if (sizeOfIsland >= mapsizeModifier*10)
                        {
                            sizeOfStartingCoords = random.Next(10, 15);
                        }
                        else
                        {
                            sizeOfStartingCoords = random.Next(5, 10);
                        }
                        

                        for (int countCor = 0; countCor < sizeOfStartingCoords; countCor++)
                        {

                            int randomindex = random.Next(0, landCoordList.Count());
                            Coords testChoice = landCoordList[randomindex];

                            //TODO: Implement something to check that they aren't near each other
                            //List<Coords> immediateRange = Utility.Matrices.Selection.SelectCircleRegion(naturalMap, testChoice, mapsizeModifier / 32);


                            seedsForVoronoi.Add(testChoice);

                        }
                    }


                    //  Perform the voronoi
                    List<List<Coords>> voronoiList = Utility.Matrices.Complex.Voronoi.ExpandTerritories(naturalMap, seedsForVoronoi, landCode_coastalLand, 999, random.Next(-9997, 8886));
                    //  Convert VoronoiLists to map
                    int[,] voronoiArray = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiList, worldMap.GetLength(0), worldMap.GetLength(1));




                    //  Convert Voronoi to Onion Slices
                    List<List<Coords>> onionSlices = new List<List<Coords>>();
                    int sizeOfMountain = 2;
                    foreach (List<Coords> coordsList in voronoiList)
                    {
                        //  For each Voronoi Section, get the edges as a seperate list of Coords
                        //  First, convert section to map
                        int[,] vSectionAsArray = Utility.Matrices.Misc.ConvertCoordstoArray(coordsList, worldMap.GetLength(0), worldMap.GetLength(1));
                        //  TODO: Find a good thickness for mounains, prev mapsizemodifier/128
                        List<List<Coords>> ListOfEdges = Utility.Matrices.Selection.SelectionSectionEdges(vSectionAsArray, 0, 0, sizeOfMountain);
                        List<Coords> listOnions = Utility.Lists.CollapseLists(ListOfEdges);
                        vSectionAsArray = Utility.Matrices.Misc.ConvertCoordstoArray(listOnions, worldMap.GetLength(0), worldMap.GetLength(1));
                        onionSlices.Add(listOnions);
                    }

                    //  Create Mountains
                    foreach (List<Coords> shape in onionSlices)
                    {
                        int randomChance = random.Next(0, 101);
                        if (randomChance > 15)
                        {
                            foreach (Coords coord in shape)
                            {
                                naturalMap[coord.x, coord.y] = landCode_mountain;
                            }
                        }
                    }
                    worldMap = naturalizeMountains(args, worldMap, 25, 2, false);


                    //  Surrond with hills
                    List<List<Coords>> hillsAroundMountains = Utility.Matrices.Selection.SelectionSectionEdges(worldMap, -999999, landCode_hillLand, mapsizeModifier / 64);
                    foreach (List<Coords> coordsL in hillsAroundMountains)
                    {
                        foreach (Coords coordinates in coordsL)
                        {
                            int randomChance = random.Next(0, 101);
                            if (randomChance > 50)
                            {
                                worldMap[coordinates.x, coordinates.y] = landCode_hillLand;
                            }
                            else
                            {
                                worldMap[coordinates.x, coordinates.y] = landCode_mountain;
                            }

                        }
                    }

                    

                    //  Generate rolling hills
                    double freqHills = 20;
                    int floor = 8;
                    int[,] generateHillLandNoise = Utility.Noise.Perlin.GeneratePerlinInt(worldMap.GetLength(0), worldMap.GetLength(1), freqHills, 0, 32, random.Next(-99999, 99999));
                    List<List<Coords>> hillCoordsLoL = Utility.Matrices.Selection.SelectSections(generateHillLandNoise, 0, floor);
                    List<Coords> hillCoords = Utility.Lists.CollapseLists(hillCoordsLoL);
                    //Utility.Print.MatrixPrint.DisplayPerlin(generateHillLandNoise, floor);
                    foreach (Coords coord in hillCoords)
                    {
                        if (naturalMap[coord.x, coord.y] > landCode_coastalLand)
                        {
                            naturalMap[coord.x, coord.y] = landCode_hillLand;
                        }

                    }


                    //  Set Edges and Coastlines
                    //  TODO: Set values to scale with size
                    List<List<Coords>> coastLandEdges = Utility.Matrices.Selection.SelectionSectionEdges(worldMap, landCode_deepWater, landCode_coastalWater, mapsizeModifier / 128);
                    List<List<Coords>> LandEdges = Utility.Matrices.Selection.SelectionSectionEdges(worldMap, landCode_deepWater, landCode_coastalWater, mapsizeModifier / 32);
                    foreach (List<Coords> coordsList in LandEdges)
                    {
                        foreach (Coords coords in coordsList)
                        {
                            naturalMap[coords.x, coords.y] = landCode_Land;
                        }
                    }
                    foreach (List<Coords> coordsList in coastLandEdges)
                    {
                        foreach (Coords coords in coordsList)
                        {
                            naturalMap[coords.x, coords.y] = landCode_coastalLand;
                        }
                    }

                    //  Set water coastlines
                    List<List<Coords>> coastWaterEdges = Utility.Matrices.Selection.SelectionSectionEdges(worldMap, landCode_coastalLand, 9999, mapsizeModifier / 64);
                    List<List<Coords>> offcoastWaterEdges = Utility.Matrices.Selection.SelectionSectionEdges(worldMap, landCode_coastalLand, 9999, mapsizeModifier / 32);
                    foreach (List<Coords> coordsList in offcoastWaterEdges)
                    {
                        foreach (Coords coords in coordsList)
                        {
                            naturalMap[coords.x, coords.y] = landCode_offcoastWater;
                        }
                    }
                    foreach (List<Coords> coordsList in coastWaterEdges)
                    {
                        foreach (Coords coords in coordsList)
                        {
                            naturalMap[coords.x, coords.y] = landCode_coastalWater;
                        }
                    }

                    return naturalMap;
                }
                #endregion

                public static int[,] GenerateContinentsAndIslands(string[] args)
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

                    #region Debug Parameter
                    bool verbose = true;
                    bool printmaps = true;

                    bool printContinentCompo = true;
                    bool displayPlacements = true;


                    bool printIslands = true;
                    #endregion


                    //  Generate a world map 
                    int[,] worldMap = WorldGen.GeographyGenerator.GeographyGenerator.Landform_Generation.getOceanicArray(args);


                    //  This next section generates a list of continents. For CaI, we will first generate a couple big continents


                    #region Generate Continents

                    restartTheProcess:
                    if (verbose)
                    {
                        Utility.Print.WriteLine("Generating Landform Maps", "8DB953");
                    }

                    List<List<Coords>> continentsList = new List<List<Coords>>();
                    List<List<Coords>> continentsListExtended = new List<List<Coords>>();
                    int continentAmount = 3;
                    for (int count = 0; count < continentAmount; count++)
                    {
                        //  Generate a random seed for the args
                        string[] passableArgs = args;
                        passableArgs[index_seed] = Convert.ToString(random.Next(-999999999, 9999999));

                        //  Create a random landmass
                        int sizeOfContinent = random.Next(4, 5);

                        //  
                        int[,] generatedContinent = WorldGen.GeographyGenerator.Landform_Generation.GenerateLandmassNoise(args, sizeOfContinent);
                        


                        //  Select it and collapse it to a list of coords
                        List<List<Coords>> mapCoordsLoL = Utility.Matrices.Selection.SelectSections(generatedContinent, landCode_coastalLand, 9999, false, false);
                        List<Coords> mapCoords = Utility.Lists.CollapseLists(mapCoordsLoL);



                        //  Now, we will generate the edges of the continent
                        int edgeRange = mapsizeModifier / 32;
                        List<List<Coords>> edgesCoordsLoL = Utility.Matrices.Selection.SelectionSectionEdges(generatedContinent, landCode_coastalLand, 9999, edgeRange);
                        List<Coords> edgesCoords = Utility.Lists.CollapseLists(edgesCoordsLoL);

                        //  Include the landCoords in edgesCoords
                        edgesCoords.AddRange(mapCoords);



                        if (printmaps)
                        {
                            GeographyGenerator.Landform_Generation.PrintLandformMap(args, worldMap);
                        }

                        continentsList.Add(mapCoords);
                        continentsListExtended.Add(edgesCoords);
                    }


                    

                    //  Prev 25
                    int maximumReturns = 1;
                    //  To make the function operate quicker, we will sort the list from largest to smallest
                    continentsListExtended = Utility.Lists.SortBySubListSize(continentsListExtended, false);



                    if (verbose)
                    {
                        Utility.Print.WriteLine("Placing continents...", "8DB953");
                    }
                    List<Coords[]> placements = Utility.Matrices.Complex.Packing.FindValidPlacements(mapRows, mapCols, continentsListExtended, maximumReturns);
                    if (placements.Count < 1)
                    {
                        if (verbose)
                        {
                            Utility.Print.WriteLine("No valid placements found: regenerating with new maps", "FF8C8C");

                        }
                        goto restartTheProcess;
                    }

                    int randomlySelectedIndex = random.Next(0, placements.Count);
                    if (verbose)
                    {
                        Console.WriteLine("Index: " + randomlySelectedIndex);
                        Console.WriteLine("Placement size: " + placements.Count());
                        Console.WriteLine("Further Generation shall continue");
                    }


                    Coords[] selectedPlacement = placements[randomlySelectedIndex];


                    for (int count = 0; count < continentsList.Count; count++)
                    {
                        List<Coords> selectedContinent = continentsList[count];
                        Coords placement = selectedPlacement[count];

                        //  Get the translated list
                        List<Coords> translatedCoords = Utility.Matrices.Transformation.TranslateSection(selectedContinent, mapRows, mapCols, placement);

                        //  Apply the translated list
                        foreach (Coords coordinates in translatedCoords)
                        {
                            worldMap[coordinates.x, coordinates.y] = landCode_Land;
                        }

                    }


                   


                    #endregion

                    #region Generate Big Islands
                    //  TODO: Generate Islands
                    List<List<Coords>> islandsCoords = new List<List<Coords>>();
                    int amountIslands = random.Next(4, 8);
                    int islandSizeMax = 4;
                    for (int isCount = 0; isCount < amountIslands; isCount++)
                    {
                        int islandSize = random.Next(1, islandSizeMax);
                        if (islandSize >= 4)
                        {
                            int chanceOfBig = random.Next(0, 101);
                            if (chanceOfBig >= 75)
                            {
                                islandSize = 2;
                            }
                        }

                        string[] passableArgs = args;
                        args[index_seed] = Convert.ToString(random.Next(-9999999, 9999999));
                        int[,] islandMap = WorldGen.GeographyGenerator.GeographyGenerator.Landform_Generation.GenerateLandmassNoise(passableArgs, islandSize);

                        List<List<Coords>> islandsCoordsLists = Utility.Matrices.Selection.SelectSections(islandMap, landCode_coastalLand, 9999);
                        List<Coords> newIsland = Utility.Lists.CollapseLists(islandsCoordsLists);
                        islandsCoords.Add(newIsland);

                        if (printIslands)
                        {
                            WorldGen.GeographyGenerator.GeographyGenerator.Landform_Generation.PrintLandformMap(args, islandMap);
                        }
                    }

                    //  Add as many islands as there are or you can to the array
                    int[,] passableArray = worldMap;
                    int bordermodifierEdgesRowBigIslands = mapRows / 16;
                    int bordermodifierEdgesColBigIslands = mapCols / 16;
                    


                    foreach (List<Coords> islandShape in islandsCoords)
                    {
                        List<Coords> centralCoords = Utility.Matrices.Complex.Packing.FindValidPlacementsInArray(passableArray, islandShape, landCode_coastalLand, 9999, mapsizeModifier / 8);
                        if (centralCoords.Count < 1)
                        {
                            break;
                        }

                        int randomindex = random.Next(0, centralCoords.Count);
                        Coords newCenter = centralCoords[randomindex];
                        List<Coords> islandTranslated = Utility.Matrices.Transformation.TranslateSection(islandShape, worldMap.GetLength(0), worldMap.GetLength(1), newCenter);
                        if (islandTranslated.Count < islandShape.Count)
                        {
                            continue;
                        }


                        foreach (Coords islandCoord in islandTranslated)
                        {
                            worldMap[islandCoord.x, islandCoord.y] = landCode_Land;
                        }
                    }
                    #endregion



                    //
                    //  Dimensions Values
                    int bordermodifierEdgesRow = mapRows / 32;
                    int bordermodifierEdgesCol = mapCols / 32;
                    //  Shave off the edges
                    for (int i = 0; i < worldMap.GetLength(0); i++)
                    {
                        for (int j = 0; j <= bordermodifierEdgesCol; j++)
                        {
                            worldMap[i, j] = landCode_deepWater;
                        }
                        for (int j = worldMap.GetLength(1) - bordermodifierEdgesCol; j < worldMap.GetLength(1); j++)
                        {
                            worldMap[i, j] = landCode_deepWater;
                        }

                    }
                    for (int j = 0; j < worldMap.GetLength(1); j++)
                    {
                        for (int i = 0; i < bordermodifierEdgesRow; i++)
                        {
                            worldMap[i, j] = landCode_deepWater;
                        }
                        for (int i = worldMap.GetLength(0) - bordermodifierEdgesRow; i < worldMap.GetLength(0); i++)
                        {
                            worldMap[i, j] = landCode_deepWater;
                        }
                    }



                    //  TODO: Naturalize the Continents
                    if (verbose)
                    {

                        Utility.Print.WriteLine("Naturalizing the Continents", "8DB953");

                    }
                    worldMap = TurnLandMassIntoNatural(args, worldMap);


                    return worldMap;
                }

            }
            

        }



    }
}
