using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace testProgram
{
    public partial class WorldGen
    {


        #region Land Codes
        private static int landCode_deepWater = 25;
        private static int landCode_subcoastWater = 29;
        private static int landCode_coastalWater = 30;

        private static readonly int landCode_outOfBounds = 15;
        private static readonly int landCode_lowLand = 40;
        private static readonly int landCode_Land = 41;
        private static readonly int landCode_hillLand = 42;
        private static readonly int landCode_mountain = 50;
        #endregion

        #region Land Color
        //  Water
        private static readonly string landColor_deepWater = "#1B80CB";
        private static readonly string landColor_subcoastWater = "#138FE5";
        private static readonly string landColor_coastalWater = "#009FFF";
        //  Land
        private static readonly string landColor_lowLand = "#A8B349";
        private static readonly string landColor_Land = "#73B349"; 
        private static readonly string landColor_hillLand = "##49B354";
        private static readonly string landColor_mountain = "#858585";
        //  Debugging
        private static readonly string landColor_outOfBounds = "#FF0000";
        private static readonly string landColor_error = "#FF52FB";
        #endregion





        private static readonly double landGen_lakeSizeModifier = 0.5;
        public partial class GeographyGenerator
        {

            public class LandGenerator
            {
                public static readonly string debug_directory = Utility.Files.GetValidPath("\\debug\\geogeneration\\landgen");
                //  Create a bitmap of the land
                public static Bitmap createLandBitmap(int[,] landMap)
                {
                    string[,] landBitmapString = new string[landMap.GetLength(0), landMap.GetLength(1)];

                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < landMap.GetLength(1); j++)
                        {
                            if (landMap[i, j] == landCode_Land)
                            {
                                landBitmapString[i, j] = landColor_Land;
                            }
                            else if (landMap[i, j] == landCode_lowLand)
                            {
                                landBitmapString[i, j] = landColor_lowLand;
                            }
                            else if (landMap[i, j] == landCode_hillLand)
                            {
                                landBitmapString[i, j] = landColor_hillLand;
                            }
                            else if (landMap[i, j] == landCode_mountain)
                            {
                                landBitmapString[i, j] = landColor_mountain;
                            }


                            else if (landMap[i, j] == landCode_coastalWater)
                            {
                                landBitmapString[i, j] = landColor_coastalWater;
                            }
                            else if (landMap[i, j] == landCode_subcoastWater)
                            {
                                landBitmapString[i, j] = landColor_subcoastWater;
                            }
                            else if (landMap[i, j] == landCode_deepWater)
                            {
                                landBitmapString[i, j] = landColor_deepWater;
                            }


                            else if (landMap[i, j] == landCode_outOfBounds)
                            {
                                landBitmapString[i, j] = landColor_outOfBounds;
                            }
                            else
                            {
                                landBitmapString[i, j] = landColor_error;
                            }




                            //
                        }
                    }


                    Bitmap landBitmap = Utility.Images.ImageFile.StringArrayToBitmap(landBitmapString);
                    return landBitmap;


                }
                public static int[,] createLandMap(string[] args)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the dimensions of a map of this specified size
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows = 128;
                    int mapCols = 256;

                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion

                    //  Debug shit
                    bool debug_showProcess = true;
                    string filepath = null;

                    //  Generate a map of the correct type
                    int[,] LandMap = new int[mapRows, mapCols];
                    switch (MAPTYPE)
                    {
                        case "CONTINENTS&ISLANDS":
                            //LandMap = generateContinentsAndIslands(args);
                            LandMap = generateDebugLandMap(args);
                            break;
                        case "DEBUG":
                            LandMap = generateDebugLandMap(args);
                            break;
                        default:
                            //LandMap = generateContinentsAndIslands(args);
                            LandMap = generateDebugLandMap(args);
                            break;
                    }
                    if (debug_showProcess) {

                        filepath = debug_directory + "\\stages\\mapseed.png";
                        Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }

                    //  Scale the map
                    double sizeMultiplier = 2.69148;
                    switch (MAPSIZE)
                    {
                        case "VERYSMALL":
                            sizeMultiplier = 1.68999;
                            break;
                        case "SMALL":
                            sizeMultiplier = 1.68999;
                            break;
                        case "MEDIUM":  // Earth Sized 
                            sizeMultiplier = 1.68999;
                            break;
                        case "LARGE":
                            sizeMultiplier = 2.69148;
                            break;
                        case "VERYLARGE":
                            sizeMultiplier = 2.69148;
                            break;

                        default: // Earth Size
                            sizeMultiplier = 2.69148;
                            break;
                    }
                    //  TODO: Test new scaleLandmap scaleLandMapToSizeWise


                    
                    //LandMap = scaleLandMapToSizeWise(args, LandMap, sizeMultiplier);
                    LandMap = scaleLandmapToNewSizeStepByStep(args, LandMap, sizeMultiplier);



                    //  Naturalize Map
                    LandMap = GeographyGenerator.LandGenerator.naturalizeLandmap(args, LandMap);
                    if (debug_showProcess)
                    {
                        //filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\_ID" + INITSEED + "scaled_" + LandMap.GetLength(0) + "x" + LandMap.GetLength(1) + "_" + (sizeMultiplier) + ".png");
                        filepath = debug_directory + "\\stages\\finalMap.png";
                        Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }
                   
                    return LandMap;
                }



                

                public static int[,] naturalizeLandmap(string[] args, int[,] sourceMap)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the dimensions of a map of this specified size
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows = sourceMap.GetLength(0);
                    int mapCols = sourceMap.GetLength(1);
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion


                    bool debug_verbose = true;
                    bool debug_hills = false;
                    bool debug_mountains = true;
                    int fileNumber = INITSEED;


                    int[,] naturalMap = sourceMap;

                    //  Save the location of coastlines and oceans to make sure no issues crop up later
                    List<Coords> savedCoasttiles = Matrices.Selection.IslandSelector.SelectSectionBordersList(naturalMap, landCode_lowLand, 9999, 1);
                    List<Coords> savedWaterTiles = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap, landCode_deepWater, landCode_coastalWater);
                    List<Coords> savedOnLandTiles = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap, landCode_lowLand, landCode_mountain);
                    List<Coords> savedOnWaterCoords = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap,-99999, landCode_coastalWater);

                    #region Hills Generation
                    //  Create hills perlin
                    int[,] arrayforHills = Utility.Noise.Perlin2D.GeneratePerlinInt(naturalMap.GetLength(0), naturalMap.GetLength(1), 200, 0, 64, random.Next(0, 999999));
                    int threshHills = 40;
                    for (int i = 0; i < arrayforHills.GetLength(0); i++)
                    {
                        for (int j = 0; j < arrayforHills.GetLength(1); j++)
                        {
                            //  If on land, generate hills
                            if (naturalMap[i, j] >= landCode_lowLand)
                            {
                                if (arrayforHills[i, j] > threshHills)
                                {
                                    arrayforHills[i, j] = landCode_hillLand;
                                }
                                else
                                {
                                    arrayforHills[i, j] = landCode_Land;
                                }
                            }
                            //  Else, set to water
                            else
                            {
                                arrayforHills[i, j] = landCode_deepWater;
                            }
                        }
                    }
                    //  Modify hills border
                    //  Use the randomize border map
                    List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(arrayforHills, landCode_hillLand, landCode_hillLand, amplitude: 1, frequency: 1, smoothness: 50, random.Next(-99999, 99999));
                    foreach (Coords coordinates in coordsList_border[0])
                    {
                        arrayforHills[coordinates.x, coordinates.y] = landCode_hillLand;
                    }
                    foreach (Coords coordinates in coordsList_border[1])
                    {
                        arrayforHills[coordinates.x, coordinates.y] = landCode_hillLand;
                    }



                    //  Remove invalid hills
                    List<List<Coords>> hillsCoords = Matrices.Selection.IslandSelector.SelectSectionsLists(arrayforHills, landCode_hillLand, landCode_hillLand);
                    //  Overwrite groups below a size
                    List<List<Coords>> hillCoords_removeBitsLow = Utility.Lists.RemoveListAboveSize(hillsCoords, mapsizeModifier / 20);
                    foreach (List<Coords> coordslist in hillCoords_removeBitsLow)
                    {
                        foreach (Coords coord in coordslist)
                        {
                            arrayforHills[coord.x, coord.y] = landCode_Land;
                        }
                    }
                    //  Overwrite groups above a size
                    List<List<Coords>> hillCoords_removeBitsHig = Utility.Lists.RemoveListBelowSize(hillsCoords, mapsizeModifier / 32);
                    foreach (List<Coords> coordslist in hillCoords_removeBitsHig)
                    {
                        foreach (Coords coord in coordslist)
                        {
                            arrayforHills[coord.x, coord.y] = landCode_Land;
                        }
                    }

                    //  Remove a random amount of hill groups
                    hillsCoords = Matrices.Selection.IslandSelector.SelectSectionsLists(arrayforHills, landCode_hillLand, landCode_hillLand);
                    foreach (List<Coords> coordslist in hillsCoords)
                    {
                        int chanceRemoved = random.Next(0, 101);
                        if (chanceRemoved > 25)
                        {
                            foreach (Coords coord in coordslist)
                            {
                                arrayforHills[coord.x, coord.y] = landCode_Land;
                            }
                        }
                    }


                    if (debug_hills)
                    {
                        Bitmap hillBitMap = createLandBitmap(arrayforHills);
                        string filepath = debug_directory + "\\mapgeneration\\hillsNoiseMap.png";
                        Utility.Images.ImageFile.saveImage(hillBitMap, filepath);
                    }
                    //  Apply hills to the real map
                    List<Coords> hillsSelected = Matrices.Selection.IslandSelector.SelectSectionsList(arrayforHills, landCode_hillLand, landCode_hillLand);
                    foreach (Coords coord in hillsSelected)
                    {
                        naturalMap[coord.x, coord.y] = landCode_hillLand;
                    }
                    #endregion

                    #region Montane Generation
                    List<List<Coords>> landCoordsMountainGen = Matrices.Selection.IslandSelector.SelectSectionsBorderLists(naturalMap, landCode_coastalWater, 99999, mapsizeModifier / 64);
                    List<Coords> seedsForVoronoi = new List<Coords>();

                    foreach (List<Coords> landCoordList in landCoordsMountainGen)
                    {
                        int sizeOfIsland = landCoordList.Count();
                        int sizeOfStartingCoords = 1;

                        if (sizeOfIsland >= mapsizeModifier * 10)
                        {
                            sizeOfStartingCoords = random.Next(15, 30);
                        }
                        else
                        {
                            sizeOfStartingCoords = 0;
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
                    List<List<Coords>> voronoiLists = Utility.Matrices.Complex.Voronoi.CreateVoronoi(naturalMap, seedsForVoronoi, landCode_lowLand, 99999, random.Next(0, 999999));
                    int[,] voronoiArray = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiLists, mapRows, mapCols);
                    if (debug_mountains)
                    {
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(voronoiArray);

                        //  Set water to zero
                        List<Coords> waterCoords = Matrices.Selection.IslandSelector.SelectSectionsList(voronoiArray, 0, 0, true, false);
                        foreach (Coords coord in waterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "000000";
                        }


                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = debug_directory + "\\mapgeneration\\mountainVoronoiRepresenation.png";
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);
                    }


                    

                    

                    //  Get create a list of "onion slices" (i.e region loops)
                    List<List<Coords>> onionSlices = new List<List<Coords>>();
                    foreach (List<Coords> voronoiSection in voronoiLists)
                    {
                        int[,] voronoiSlice = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiSection, mapRows, mapCols);
                        List<Coords> listEdges = Matrices.Selection.IslandSelector.SelectSectionBordersList(voronoiSlice, 1, 99999, 0, true);
                        onionSlices.Add(listEdges);

                    }



                    //  Create mountain valid regions map
                    int[,] mountainMapNoise = new int[naturalMap.GetLength(0), naturalMap.GetLength(1)];

                    for (int i = 0; i < mountainMapNoise.GetLength(0); i++)
                    {
                        for (int j = 0; j < mountainMapNoise.GetLength(1); j++) {
                            mountainMapNoise[i, j] = 0;
                        }
                    }
                    int indexForOniony = 1;
                    foreach (List<Coords> mountainrange in onionSlices)
                    {
                        foreach (Coords mCoords in mountainrange)
                        {
                            mountainMapNoise[mCoords.x, mCoords.y] = indexForOniony;
                        }
                        indexForOniony++;
                    }
                    if (debug_mountains)
                    {
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(mountainMapNoise);

                        //  Set water to zero
                        foreach (Coords coord in savedOnWaterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#000000";
                        }
                        //  Set Land to Gray
                        List<Coords> voronoiRepresentationTilesLand = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap, 0, 0);
                        foreach (Coords coord in voronoiRepresentationTilesLand)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#D9D9D9";
                        }

                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = debug_directory + "\\mapgeneration\\mountainLoopsNoiseUnmodified.png";
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);
                    }



                    //  Remove mountains near water
                    List<Coords> edgesOfMountains_WaterRemoval = Matrices.Selection.IslandSelector.SelectSectionsList(mountainMapNoise, 1, 999999999);
                    foreach (Coords coordinates in edgesOfMountains_WaterRemoval)
                    {
                        bool nearWater = false;
                        List<Coords> rangeOfWater = Utility.Matrices.Selection.SelectCircleRegion(mountainMapNoise, coordinates, 7);
                        foreach (Coords waterCoords in rangeOfWater)
                        {
                            if (naturalMap[waterCoords.x, waterCoords.y] < landCode_lowLand)
                            {
                                nearWater = true;
                            }
                        }

                        if (nearWater)
                        {
                            mountainMapNoise[coordinates.x, coordinates.y] = 0;
                        }


                    }
                    //  Get the remaining mountains
                    List<List<Coords>> mountainRangesBreakdown = Utility.Matrices.Selection.IslandSelector.CreateAllSectionsList(mountainMapNoise, true, true);
                    //  Convert them into a new array
                    mountainMapNoise = Utility.Matrices.Misc.ConvertCoordstoArray(mountainRangesBreakdown, mapRows, mapCols);
                    if (debug_mountains)
                    {
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(mountainMapNoise);

                        //  Set water to zero
                        foreach (Coords coord in savedOnWaterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#000000";
                        }
                        //  Set Land to Gray
                        List<Coords> voronoiRepresentationTilesLand = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap, 0, 0);
                        foreach (Coords coord in voronoiRepresentationTilesLand)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#D9D9D9";
                        }

                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = debug_directory + "\\mapgeneration\\mountainLoopsNoiseSortedFromWater.png";
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);
                    }


                    //  Break up the mountain chains
                    int indexForMountainBreakdown = 1;
                    int[,] mountainNoiseForBreakapart = new int[naturalMap.GetLength(0), naturalMap.GetLength(1)];
                    for (int i = 0; i < mountainNoiseForBreakapart.GetLength(0); i++)
                    {
                        for (int j = 0; j < mountainNoiseForBreakapart.GetLength(1); j++)
                        {
                            mountainNoiseForBreakapart[i, j] = 0;
                        }
                    }


                    //  For each onion loop, create a certain amount of mountains
                    
                    foreach (List<Coords> mountainRange in mountainRangesBreakdown)
                    {
                        int indexStart = 0;
                        int indexEnd = mountainRange.Count() / 2;

                        for (int counter = indexStart; counter < indexEnd; counter++)
                        {
                            Coords testValue = mountainRange[counter];
                            mountainNoiseForBreakapart[testValue.x, testValue.y] = indexForMountainBreakdown;
                        }

                        indexForMountainBreakdown++;
                    }


                    mountainMapNoise = mountainNoiseForBreakapart;
                    if (debug_mountains)
                    {
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(mountainMapNoise);

                        //  Set water to zero
                        foreach (Coords coord in savedOnWaterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#000000";
                        }
                        //  Set Land to Gray
                        List<Coords> voronoiRepresentationTilesLand = Matrices.Selection.IslandSelector.SelectSectionsList(naturalMap, 0, 0);
                        foreach (Coords coord in voronoiRepresentationTilesLand)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "#D9D9D9";
                        }

                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = debug_directory + "\\mapgeneration\\mountainsWithBreakApart.png";
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);
                    }















                    //  Final print the mountains
                    if (debug_mountains)
                    {
                        Bitmap montanteBitMap = createLandBitmap(naturalMap);
                        string filepath = debug_directory + "\\mapgeneration\\mountainPreNaturalizedNoiseMap.png";
                        Utility.Images.ImageFile.saveImage(montanteBitMap, filepath);
                    }


                   

                    


                    #endregion










                    #region Finishing touches land
                    //  Make sure that all water tiles are reset to their proper location
                    foreach (Coords coastTile in savedCoasttiles)
                    {
                        naturalMap[coastTile.x, coastTile.y] = landCode_lowLand;
                    }
                    foreach (Coords waterTile in savedWaterTiles)
                    {
                        naturalMap[waterTile.x, waterTile.y] = landCode_deepWater;
                    }
                    //  Set the coastlines to be proper
                    List<Coords> coastalLand = Matrices.Selection.IslandSelector.SelectSectionBordersList(naturalMap, landCode_lowLand, 9999, 2);
                    List<Coords> coastlineLand = Matrices.Selection.IslandSelector.SelectSectionBordersList(naturalMap, landCode_lowLand, 9999, 0);
                    foreach (Coords coord in coastalLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_Land;
                    }
                    foreach (Coords coord in coastlineLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_lowLand;
                    }
                    #endregion

                    List<List<Coords>> coastWater = Matrices.Selection.IslandSelector.SelectSectionsBorderLists(naturalMap, landCode_lowLand, 9999, 3, false);
                    List<List<Coords>> offcoastWater = Matrices.Selection.IslandSelector.SelectSectionsBorderLists(naturalMap, landCode_lowLand, 9999, 6, false);
                    foreach (List<Coords> offCoords in offcoastWater)
                    {
                        foreach (Coords coordinates in offCoords)
                        {
                            naturalMap[coordinates.x, coordinates.y] = landCode_subcoastWater;
                        }
                    }
                    foreach (List<Coords> coastCoords in coastWater)
                    {
                        foreach (Coords coordinates in coastCoords)
                        {
                            naturalMap[coordinates.x, coordinates.y] = landCode_coastalWater;
                        }

                    }

                    return naturalMap;
                }

                #region Independent World Generation
                public static int[,] generateDebugLandMap(string[] args)
                {
                    //  Load custom earthlike map at specified location 
                    //  IMPORTANT: debugLandMap must be 256x512 sized
                    Utility.Print.WriteLine("Loading debugging Map: ", "BD00FF"); 

                    string filepath = Utility.Files.GetValidPath("\\worldgen\\worldgenComp\\worldgenResource\\earthlike\\debugLandMap.png");
                    Utility.Print.WriteLine("Creating bitmap from filepath: " + filepath, "BD00FF");


                    Bitmap LandMapBitMap = Utility.Images.ImageFile.getBitmap(filepath);
                    Utility.Print.WriteLine("Converting bitmap to land map: ", "BD00FF");
                    int[,] LandMap = GeographyGenerator.LandGenerator.createLandMapFromBitmap(LandMapBitMap);
                    Utility.Print.WriteLine("Returning landMap: ", "BD00FF");
                    return LandMap;
                }
                #endregion







                #region Functions which scale maps to new size while randomizing borders
                public static int[,] scaleLandmapToNewSizeStepByStep(string[] args, int[,] grid, double sizeMultiplier)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    int mapRows = grid.GetLength(0);
                    int mapCols = grid.GetLength(1);
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2));
                    #endregion

                    bool debug_displayCurSize = true;

                    int debug_debugNumber = INITSEED;

                    int[,] sourceMap = grid;
                    int[,] modifiedMap = grid;

                    int numIterations = Convert.ToInt32(sizeMultiplier);
                    double remainder = sizeMultiplier - numIterations;

                    int mapRowSize = sourceMap.GetLength(0);
                    int mapColSize = sourceMap.GetLength(1);

                    //  Do the base conversion
                    for (int counter = 0; counter < numIterations; counter++)
                    {
                        mapRowSize = Convert.ToInt32(mapRowSize * 2);
                        if (mapRowSize % 2 != 0) // Check if the remainder when divided by 2 is not 0
                        {
                            mapRowSize--;
                        }
                        mapColSize = 2 * mapRowSize;
                        modifiedMap = Utility.Matrices.Misc.ScaleArray(modifiedMap, mapRowSize, mapColSize);
                        if (debug_displayCurSize)
                        {
                            string showValue = "Row:" + mapRowSize + ",Col:" + mapColSize;
                            Utility.Print.WriteLine(showValue, "FF99F4");
                        }
                        //  Do coastlinification
                        List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(modifiedMap, landCode_Land, landCode_Land, amplitude: (mapsizeModifier / 64), frequency: (mapsizeModifier / 64), smoothness: 5, random.Next(-99999, 99999), 8, 0.5);
                        //  Subtract
                        foreach (Coords coordinates in coordsList_border[0])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_deepWater;
                            //modifiedMap[coordinates.x, coordinates.y] = landCode_Land;
                        }
                        //  Add
                        foreach (Coords coordinates in coordsList_border[1])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_Land;
                        }
                        //  Select all islands
                        List<List<Coords>> alllandmass_removalbits = Matrices.Selection.IslandSelector.SelectSectionsLists(modifiedMap, landCode_Land, landCode_Land);
                        //  Sort islands by size
                        alllandmass_removalbits = Utility.Lists.SortBySubListSize(alllandmass_removalbits, false);
                        
                        
                        
                        //  Remove all islands below a certain size
                        int sizeMod = Convert.ToInt32((double)((modifiedMap.GetLength(0) + modifiedMap.GetLength(1)) / 2));
                        alllandmass_removalbits = Utility.Lists.RemoveListBelowSize(alllandmass_removalbits, Convert.ToInt32(((double)sizeMod) / 15.00));
                        foreach (List<Coords> alllandmass_shape in alllandmass_removalbits)
                        {
                            foreach (Coords cellin in alllandmass_shape)
                            {
                                modifiedMap[cellin.x, cellin.y] = landCode_deepWater;
                            }
                        }

                    }



                    //  Perform the remainder
                    int newRowRemainder = Convert.ToInt32(modifiedMap.GetLength(0) * Math.Pow(2, remainder));
                    if (newRowRemainder % 2 != 0) // Check if the remainder when divided by 2 is not 0
                    {
                        newRowRemainder--;
                    }
                    int newColRemainder = newRowRemainder * 2;
                    if (debug_displayCurSize)
                    {
                        string showValue = "Remainder: Row:" + newRowRemainder + ",Col:" + newColRemainder;
                        Utility.Print.WriteLine(showValue, "FF99F4");
                    }
                    modifiedMap = Utility.Matrices.Misc.ScaleArray(modifiedMap, newRowRemainder, newColRemainder);
                    //  Do a final coastlinification



                    return modifiedMap;
                }

                //  TODO: Play around with these so that borders are verified according to the sizing
                public static int[,] scaleLandMapToSizeWise(string[] args, int[,] grid, double sizeMultiplier)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    int mapRows = grid.GetLength(0);
                    int mapCols = grid.GetLength(1);
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2));
                    #endregion


                    bool debug_PrintMaps = true;
                    bool debug_verbose = true;
                    //  Copy the grid
                    int[,] sourceMap = grid;

                    //  This algorithm will work by seperating each isolated landmass out into its own sized map, modifying it and placing it in the new position
                    //  Remember, this is based on amplifying a 256x512 map
                    int newRow = Convert.ToInt32(256 * Math.Pow(2, sizeMultiplier));
                    int newCol = 2 * newRow;

                    //  Expand the map
                    int[,] expandedMap = Utility.Matrices.Misc.ScaleArray(sourceMap, newRow, newCol);
                    int[,] newLandMap = new int[newRow, newCol];
                    for (int i = 0; i < newLandMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < newLandMap.GetLength(1); j++)
                        {
                            newLandMap[i, j] = landCode_deepWater;
                        }
                    }

                    //  Get a list of the Coords of each landmass
                    List<List<Coords>> defaultLandmasses = Matrices.Selection.IslandSelector.SelectSectionsLists(expandedMap, landCode_lowLand, 999999, true, true);
                    //  Sort the landmasses into proper sizes
                    defaultLandmasses = Utility.Lists.SortBySubListSize(defaultLandmasses, false);

                    //  Get the central Coords for each landmass, and create a new landmass
                    List<Coords> coorespondingCenter = new List<Coords>();
                    List<int[,]> extendedMaps = new List<int[,]>();



                    if (debug_verbose)
                    {
                        Utility.Print.WriteLine("Selecting individual landmasses", "8AC8E6");
                    }
                    for (int landMassIndex = 0; landMassIndex < defaultLandmasses.Count; landMassIndex++)
                    {
                        //  Get the landmass at this index
                        List<Coords> shape = defaultLandmasses[landMassIndex];

                        //  Find and save the original central points
                        Coords centerPoint = Utility.Matrices.Geometry.GetCenterOfMass(shape);
                        coorespondingCenter.Add(centerPoint);

                        int shapeRow =  Utility.Matrices.Misc.MaxRowDistance(shape);
                        int shapeCol = Utility.Matrices.Misc.MaxColDistance(shape);

                        int stopgap = 20;

                        int newSizeRowMiddle = stopgap;
                        int newSizeColMiddle = stopgap;

                        //  Translate the landmass to a new array, sized to fit the landmass
                        List<Coords> translatedCoords =  Utility.Matrices.Geometry.TranslateSection(shape, expandedMap.GetLength(0), expandedMap.GetLength(1), new Coords(newSizeRowMiddle, newSizeColMiddle), false);

                        int scalingMapRow = shapeRow + (stopgap*2);
                        int scalingMapCol = shapeCol + (stopgap * 2);

                        int[,] translatedMap = new int[scalingMapRow, scalingMapCol];
                        for (int i = 0; i < translatedMap.GetLength(0); i++)
                        {
                            for (int j = 0; j < translatedMap.GetLength(1); j++)
                            {
                                translatedMap[i, j] = landCode_deepWater;
                            }
                        }


                        foreach (Coords coord in translatedCoords)
                        {
                            translatedMap[coord.x, coord.y] = landCode_Land;
                        }
                        extendedMaps.Add(translatedMap);

                        if (debug_PrintMaps)
                        {
                            string filepath = debug_directory + "\\scaling\\extendedmap" + landMassIndex + ".png";
                            Bitmap savable = createLandBitmap(translatedMap);
                            Utility.Images.ImageFile.saveImage(savable, filepath);
                        }
                    }


                    if (debug_verbose)
                    {
                        Utility.Print.WriteLine("Perfoming coastlinification", "8AC8E6");
                    }
                    for (int landMassIndex = 0; landMassIndex < defaultLandmasses.Count; landMassIndex++)
                    {
                        int[,] mapsection = extendedMaps[landMassIndex];

                        //  Modify the map to coastlinify it 
                        if (debug_verbose)
                        {
                            Utility.Print.WriteLine("Getting perimeter", "8AC8E6");
                        }



                        //  Get the mapsize
                        List<Coords> selectedTiles = Matrices.Selection.IslandSelector.SelectSectionsList(mapsection, landCode_lowLand, 9999, true, true);



                        

                        if (debug_verbose)
                        {
                            Utility.Print.WriteLine("Assigning values based on size", "8AC8E6");
                        }
                        double amplitude = (mapsizeModifier / 64);
                        double frequency = (mapsizeModifier / 64);
                        double smoothness = 50;
                        int octaves = 8;
                        double persistence = 0.5;

                        //  If big enough, remove little tiles
                        if (selectedTiles.Count() >= mapsizeModifier)
                        {
                            amplitude = (mapsizeModifier / 64);
                            frequency = (mapsizeModifier / 64);
                            smoothness = 50;
                            octaves = 8;
                            persistence = 0.5;
                        }
                        else
                        {
                            amplitude = (mapsizeModifier / 64);
                            frequency = (mapsizeModifier / 64);
                            smoothness = 50;
                            octaves = 8;
                            persistence = 0.5;
                        }



                        if (debug_verbose)
                        {
                            Utility.Print.WriteLine("Modifying coasts", "8AC8E6");
                        }
                        //  Coastline it 
                        List<List<Coords>> randomizableBorders = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(mapsection, landCode_lowLand, 99999, amplitude, frequency, smoothness, seed: random.Next(-99999, 99999), octaves, persistence);
                        //  Subtract land
                        foreach (Coords coordinates in randomizableBorders[0])
                        {
                            mapsection[coordinates.x, coordinates.y] = landCode_deepWater;

                        }
                        //  Add land
                        foreach (Coords coordinates in randomizableBorders[1])
                        {
                            mapsection[coordinates.x, coordinates.y] = landCode_Land;
                        }


                        //  If big enough, remove little tiles
                        if (selectedTiles.Count() >= mapsizeModifier)
                        {
                            List<List<Coords>> tilesBigCont = Matrices.Selection.IslandSelector.SelectSectionsLists(mapsection, landCode_lowLand, 9999, true, true);
                            List<Coords> removableTiles = new List<Coords>();
                            foreach (List<Coords> tilesList in tilesBigCont)
                            {
                                if (tilesList.Count() < mapsizeModifier / 16)
                                {
                                    removableTiles.AddRange(tilesList);
                                }
                            }
                            foreach (Coords removeTile in removableTiles)
                            {
                                mapsection[removeTile.x, removeTile.y] = landCode_deepWater;
                            }

                        }





                        //  Test that the land is modified
                        if (debug_verbose)
                        {
                            Utility.Print.WriteLine("Testing that land remains: ", "8AC8E6");
                        }
                        List<Coords> landTilesList = Matrices.Selection.IslandSelector.SelectSectionsList(mapsection, landCode_lowLand, 9999, true, true);
                        if (debug_verbose)
                        {
                            Utility.Print.WriteLine("Land remaining: " + landTilesList.Count(), "8AC8E6");
                        }
                        if (landTilesList.Count() < 1)
                        {
                            //  TODO: Implement a redo GOTO?
                            //  Reset it to the unmodified version
                            mapsection = extendedMaps[landMassIndex];
                        }





                        if (debug_PrintMaps)
                        {
                            
                            string filepath = debug_directory + "\\scaling\\extendedmap" + landMassIndex + "coastlined.png";
                            Bitmap savable = createLandBitmap(mapsection);
                            Utility.Images.ImageFile.saveImage(savable, filepath);
                        }


                        //  Select and paint the map to the new array
                        List<Coords> selectedMap = Matrices.Selection.IslandSelector.SelectSectionsList(mapsection, landCode_lowLand, 9999, true, true);
                        Coords newCenter = coorespondingCenter[landMassIndex];
                        selectedMap = Utility.Matrices.Geometry.TranslateSection(selectedMap, newLandMap.GetLength(0), newLandMap.GetLength(1), newCenter);


                        foreach (Coords coordinates in selectedMap)
                        {
                            newLandMap[coordinates.x, coordinates.y] = landCode_Land;
                        }

                    }

                    return newLandMap;
                }

                #endregion


                #region Misc Functions

                public static int[,] createLandMapFromBitmap(Bitmap image)
                {
                    string[,] bitmap = Utility.Images.ImageFile.BitmapToStringArray(image);
                    int[,] landMap = new int[image.Height, image.Width];
                    bool debug_showColor = false;

                    for (int i = 0; i < bitmap.GetLength(0); i++)
                    {
                        for (int j = 0; j < bitmap.GetLength(1); j++)
                        {
                            
                            if (debug_showColor)
                            {
                                Utility.Print.WriteLine(bitmap[i, j], "000000", bitmap[i, j]);
                            }


                            if (bitmap[i, j] == landColor_deepWater)
                            {
                                landMap[i, j] = landCode_deepWater;
                            }
                            else if (bitmap[i, j] == landColor_subcoastWater)
                            {
                                landMap[i, j] = landCode_subcoastWater;
                            }
                            else if (bitmap[i, j] == landColor_coastalWater)
                            {
                                landMap[i, j] = landCode_coastalWater;
                            }
                            //  Land
                            else if (bitmap[i, j] == landColor_lowLand)
                            {
                                landMap[i, j] = landCode_lowLand;
                            }
                            else if (bitmap[i, j] == landColor_Land)
                            {
                                landMap[i, j] = landCode_Land;
                            }
                            else if (bitmap[i, j] == landColor_hillLand)
                            {
                                landMap[i, j] = landCode_hillLand;
                            }
                            else if (bitmap[i, j] == landColor_mountain)
                            {
                                landMap[i, j] = landCode_mountain;
                            }
                            else if (bitmap[i, j] == landColor_outOfBounds)
                            {
                                landMap[i, j] = landCode_outOfBounds;
                            }
                            else 
                            {
                                landMap[i, j] = landCode_outOfBounds;
                            }





                            //
                        }
                    }

                    return landMap;
                }

                #endregion

            }
        }
    }
}
