using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        private static readonly string landColor_hillLand = "#4B742F";
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
                            LandMap = generateContinentsAndIslands(args);
                            break;
                        default:
                            LandMap = generateContinentsAndIslands(args);
                            break;
                            break;
                    }
                    if (debug_showProcess) {

                        filepath = debug_directory + "\\mapseed.png";
                        Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }



                    //  Scale the map
                    double sizeMultiplier = 2.69148;
                    switch (MAPSIZE)
                    {
                        case "VERYSMALL":
                            sizeMultiplier = 2.69148;
                            break;
                        case "SMALL":
                            sizeMultiplier = 2.69148;
                            break;
                        case "MEDIUM":  // Earth Sized 
                            sizeMultiplier = 2.69148;
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
                    //  TODO: Test new scaleLandmap
                    LandMap = scaleLandmapToNewSizeStepByStep(args, LandMap, sizeMultiplier);
                    

                    




                    //  Naturalize Map
                    LandMap = GeographyGenerator.LandGenerator.naturalizeLandmap(args, LandMap);
                    if (debug_showProcess)
                    {
                        //filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\_ID" + INITSEED + "scaled_" + LandMap.GetLength(0) + "x" + LandMap.GetLength(1) + "_" + (sizeMultiplier) + ".png");
                        filepath = debug_directory + "scale" + + LandMap.GetLength(0) + "x" + LandMap.GetLength(1) + "_sizeMult" + (sizeMultiplier)+ ".png";
                        Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }
                   


                    return LandMap;
                }


                #region Independent World Generation
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

                //  Generate a single contiguos land mass of specified size category, in dimensions 128x256
                public static int[,] generateLandMassSmall(string[] args, int sizeCategory)
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
                    //  
                    bool debug_printGeneratedNoise = false;
                    bool debug_verbose = true;
                    bool debug_printProcess = false;
                    bool debug_printLakes = true;


                    restartFunction:

                    int[,] landMap = new int[mapRows, mapCols];
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }


                    //  Create the limits of the size, as well as initialize the frequency
                    double perlinFrequency = 7.0;
                    int minSizeLimit = 0;
                    int maxSizeLimit = 0;
                    //  Dimensions Values for cutoff
                    int bordermodifierEdgesRow = mapRows / 64;
                    int bordermodifierEdgesCol = mapCols / 64;
                    switch (sizeCategory)
                    {
                        case 1: //  Small Island (This should be the smallest available land size 
                            minSizeLimit = 0;
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 1));
                            perlinFrequency = 15;
                            break;
                        case 2: //  Medium Sized Island
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 1));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 5));
                            break;
                        case 3: //  Large Island
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 5));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 10));
                            break;
                        case 4: //  Smaller sized continent

                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 15));
                            break;
                        case 5: //  Medium Sized Continent
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 15));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 20));
                            perlinFrequency = 3;
                            break;
                        case 6: //This is the theoretical max size
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 20));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 25));
                            perlinFrequency = 3;
                            break;
                        case 7:
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 25));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 30));
                            perlinFrequency = 3;
                            break;
                        case 8:
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 30));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 35));
                            perlinFrequency = 3;
                            break;


                        default:
                            //  Get Continents of a small size, by default
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                    }
                    bool isValidContinent = false;


                    #region Generate the base map
                    //  Generate the base map
                    int counterForGeneration = random.Next(2, 6);
                    for (int layerNumber = 0; layerNumber < counterForGeneration; layerNumber++)
                    {
                        //  Generate initial perlin map
                        restartContinent:
                        int[,] generatedNoise = Utility.Noise.Perlin2D.GeneratePerlinInt(mapRows, mapCols, perlinFrequency, 0, 64, random.Next(-999999, 9999999));
                        int lowerlimit = 24;

                        //  Convert the map to hold only landCode versions
                        for (int i = 0; i < generatedNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j < generatedNoise.GetLength(1); j++)
                            {
                                if (generatedNoise[i, j] <= lowerlimit)
                                {
                                    generatedNoise[i, j] = landCode_Land;
                                }
                                else
                                {
                                    generatedNoise[i, j] = landCode_deepWater;
                                }
                            }


                        }



                        //  Shave off the edges
                        for (int i = 0; i < generatedNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j <= bordermodifierEdgesCol; j++)
                            {
                                generatedNoise[i, j] = landCode_deepWater;
                            }
                            for (int j = generatedNoise.GetLength(1) - bordermodifierEdgesCol; j < generatedNoise.GetLength(1); j++)
                            {
                                generatedNoise[i, j] = landCode_deepWater;
                            }

                        }
                        for (int j = 0; j < generatedNoise.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                generatedNoise[i, j] = landCode_deepWater;
                            }
                            for (int i = generatedNoise.GetLength(0) - bordermodifierEdgesRow; i < generatedNoise.GetLength(0); i++)
                            {
                                generatedNoise[i, j] = landCode_deepWater;
                            }
                        }


                        //  Select all continents
                        List<List<Coords>> continentsList = Utility.Matrices.Selection.SelectCellsWithinRange(generatedNoise, landCode_lowLand, 99999);
                        //  Sort out continents below size
                        List<List<Coords>> continentsList_belowSize = Utility.Lists.RemoveListBelowSize(continentsList, minSizeLimit);
                        foreach (List<Coords> shape in continentsList_belowSize)
                        {
                            foreach (Coords coord in shape)
                            {
                                generatedNoise[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }
                        List<List<Coords>> continentsList_aboveSize = Utility.Lists.RemoveListBelowSize(continentsList, maxSizeLimit);
                        foreach (List<Coords> shape in continentsList_aboveSize)
                        {
                            foreach (Coords coord in shape)
                            {
                                generatedNoise[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }



                        //  Select all continents with imrpoper dimensions
                        continentsList = Utility.Matrices.Selection.SelectCellsWithinRange(generatedNoise, landCode_lowLand, 99999);
                        //  List of continents to remove
                        List<List<Coords>> badConts = new List<List<Coords>>();
                        int rowMinimum = Convert.ToInt32((double)(mapsizeModifier / 2));
                        int colMaximum = Convert.ToInt32((double)(mapsizeModifier / 2));

                        foreach (List<Coords> shape in continentsList)
                        {
                            int continentColCount = Utility.Matrices.Misc.MaxColDistance(shape);
                            int continentRowCount = Utility.Matrices.Misc.MaxRowDistance(shape);

                            //  Check that these two do not vary signficiantly
                            if (continentColCount > Convert.ToInt32((double)(continentRowCount * (1.10))) || continentColCount < Convert.ToInt32((double)(continentRowCount * (0.90))))
                            {
                                badConts.Add(shape);
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
                                generatedNoise[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }


                        //  Verify that there are valid continents
                        continentsList = Utility.Matrices.Selection.SelectCellsWithinRange(generatedNoise, landCode_lowLand, 99999);
                        if (continentsList.Count < 1)
                        {
                            if (debug_verbose)
                            {
                                Utility.Print.WriteLine("No continents remaining", "FF0000");
                            }
                            goto restartContinent;
                        }

                        if (debug_printGeneratedNoise)
                        {
                            for (int i = 0; i < generatedNoise.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "FFF200");
                                for (int j = 0; j < generatedNoise.GetLength(1); j++)
                                {
                                    if (generatedNoise[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", "FFF200");
                                    }
                                    else if (generatedNoise[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", "FF0000");
                                    }
                                    else
                                    {
                                        Console.Write("  ");
                                    }
                                }
                                Utility.Print.Write("|<", "FFF200");
                                Console.WriteLine();
                            }
                            Console.WriteLine();
                        }



                        //  If so, find a single valid continent
                        int randomContinentIndex = random.Next(0, continentsList.Count);
                        List<Coords> validContinent = continentsList[randomContinentIndex];

                        int[,] continentMap = new int[mapRows, mapCols];
                        for (int i = 0; i < continentMap.GetLength(0); i++)
                        {
                            for (int j = 0; j < continentMap.GetLength(1); j++)
                            {
                                continentMap[i, j] = landCode_deepWater;
                            }
                        }

                        validContinent = Utility.Matrices.Geometry.TranslateSection(validContinent, mapRows, mapCols, new Coords(mapRows / 2, mapCols / 2), true);

                        foreach (Coords coord in validContinent)
                        {
                            landMap[coord.x, coord.y] = landCode_Land;
                        }

                    }
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j <= bordermodifierEdgesCol; j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int j = landMap.GetLength(1) - bordermodifierEdgesCol; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }

                    }
                    for (int j = 0; j < landMap.GetLength(1); j++)
                    {
                        for (int i = 0; i < bordermodifierEdgesRow; i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int i = landMap.GetLength(0) - bordermodifierEdgesRow; i < landMap.GetLength(0); i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    if (debug_printProcess)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "91FF00");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "91FF00");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "91FF00");
                                }
                                else
                                {
                                    Console.Write("  ");
                                }
                            }
                            Utility.Print.Write("|<", "91FF00");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    #endregion

                    #region Create borders
                    //  Use the randomize border map
                    List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(landMap, landCode_Land, landCode_Land, amplitude: 3, frequency: 0.5, smoothness: 2, random.Next(-99999, 99999));
                    foreach (Coords coordinates in coordsList_border[0])
                    {
                        landMap[coordinates.x, coordinates.y] = landCode_deepWater;
                    }
                    foreach (Coords coordinates in coordsList_border[1])
                    {
                        landMap[coordinates.x, coordinates.y] = landCode_Land;
                    }
                    if (debug_printProcess)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "91FF00");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "91FF00");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (landMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FF0000");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "91FF00");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    //  Break up the edges slightly
                    List<List<Coords>> landmapBordersLoL = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(landMap, landCode_lowLand, 9999, 1);
                    List<Coords> landmapBorders = Utility.Lists.CollapseLists(landmapBordersLoL);
                    int countOfCyclesBorder = random.Next(3, 5);
                    for (int countCycles = 0; countCycles < countOfCyclesBorder; countCycles++)
                    {
                        foreach (Coords borderCoords in landmapBorders)
                        {
                            List<Coords> circleRegion = Utility.Matrices.Selection.SelectCircleRegion(landMap, borderCoords, 2);
                            int randomChance = random.Next(0, 101);
                            if (randomChance < 45)
                            {
                                foreach (Coords borderCirCoords in circleRegion)
                                {
                                    if (landMap[borderCirCoords.x, borderCirCoords.y] == landCode_Land)
                                    {
                                        landMap[borderCirCoords.x, borderCirCoords.y] = landCode_deepWater;
                                    }

                                }

                            }
                            else if (randomChance >= 45 && randomChance < 66)
                            {
                                foreach (Coords borderCirCoords in circleRegion)
                                {
                                    if (landMap[borderCirCoords.x, borderCirCoords.y] == landCode_deepWater)
                                    {
                                        landMap[borderCirCoords.x, borderCirCoords.y] = landCode_Land;
                                    }

                                }
                            }

                        }
                    }
                    if (debug_printProcess)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "91FF00");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "91FF00");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (landMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FF0000");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "91FF00");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    //  TODO: Renable
                    //  Remove any immediate small islands
                    List<List<Coords>> alllandmass_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(landMap, landCode_Land, landCode_Land);
                    alllandmass_removalbits = Utility.Lists.SortBySubListSize(alllandmass_removalbits, false);
                    alllandmass_removalbits = Utility.Lists.RemoveListBelowSize(alllandmass_removalbits, alllandmass_removalbits.Count);
                    foreach (List<Coords> alllandmass_shape in alllandmass_removalbits)
                    {
                        foreach (Coords cellin in alllandmass_shape)
                        {
                            landMap[cellin.x, cellin.y] = landCode_deepWater;
                        }
                    }
                    if (debug_printProcess)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "91FF00");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "91FF00");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (landMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FF0000");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "91FF00");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    //  Remove small water holes
                    List<List<Coords>> alllwater_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(landMap, landCode_deepWater, landCode_deepWater, true, true);
                    alllwater_removalbits = Utility.Lists.SortBySubListSize(alllwater_removalbits, false);
                    alllwater_removalbits = Utility.Lists.RemoveListBelowSize(alllwater_removalbits, alllwater_removalbits.Count);
                    foreach (List<Coords> allwater_shape in alllwater_removalbits)
                    {
                        foreach (Coords cellin in allwater_shape)
                        {
                            landMap[cellin.x, cellin.y] = landCode_Land;
                        }
                    }
                    if (debug_printProcess)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "91FF00");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "91FF00");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (landMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FF0000");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "91FF00");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    #endregion

                    #region Create lakes
                    //  Add addition of lakes if big enough
                    List<Coords> lakesCheckList = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(landMap, landCode_lowLand, 999999, true, true);
                    int minSizeLimitLakes = Convert.ToInt32(((double)mapsizeModifier * 10));
                    if (lakesCheckList.Count >= minSizeLimitLakes)
                    {
                        if (debug_printLakes)
                        {
                            Utility.Print.WriteLine("Lake Min triggered", "00A8FF");
                        }
                        int[,] arrayForLakes = Utility.Noise.Perlin2D.GeneratePerlinInt(landMap.GetLength(0), landMap.GetLength(1), 50, 0, 64, random.Next(0, 999999));
                        int threshHills = 40;
                        for (int i = 0; i < arrayForLakes.GetLength(0); i++)
                        {
                            for (int j = 0; j < arrayForLakes.GetLength(1); j++)
                            {
                                //  If on land, generate hills
                                if (landMap[i, j] >= landCode_lowLand)
                                {
                                    if (arrayForLakes[i, j] > threshHills)
                                    {
                                        arrayForLakes[i, j] = landCode_coastalWater;
                                    }
                                    else
                                    {
                                        arrayForLakes[i, j] = landCode_Land;
                                    }
                                }
                                //  Else, set to water
                                else
                                {
                                    arrayForLakes[i, j] = landCode_deepWater;
                                }
                            }


                        }
                        List<List<Coords>> lakesList = Utility.Matrices.Selection.SelectCellsWithinRange(arrayForLakes, landCode_coastalWater, landCode_coastalWater, true, true);
                        //  TODO Check for no lakes

                        int amountDesiredLakes = random.Next(Convert.ToInt32(lakesList.Count / 2), lakesList.Count);
                        List<List<Coords>> removableLakes = Utility.Lists.GetRandomSelection(lakesList, amountDesiredLakes, false, random.Next(0, 99999));
                        foreach (List<Coords> listCoords in removableLakes)
                        {
                            foreach (Coords remova in listCoords)
                            {
                                arrayForLakes[remova.x, remova.y] = landCode_Land;
                            }
                        }
                        if (debug_printLakes)
                        {
                            Utility.Print.WriteLine("Lake Min triggered", "00A8FF");
                            for (int i = 0; i < arrayForLakes.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "87DBFF");
                                for (int j = 0; j < arrayForLakes.GetLength(1); j++)
                                {
                                    if (arrayForLakes[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", landColor_Land);
                                    }
                                    else if (arrayForLakes[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", landColor_outOfBounds);
                                    }
                                    else if (arrayForLakes[i, j] == landCode_deepWater)
                                    {
                                        Utility.Print.Write("  ", landColor_deepWater);
                                    }
                                    else if (arrayForLakes[i, j] == landCode_coastalWater)
                                    {
                                        Utility.Print.Write("!!", landColor_coastalWater);
                                    }
                                    else
                                    {
                                        Utility.Print.Write("??", landColor_error);
                                    }
                                }
                                Utility.Print.Write("|<", "87DBFF");
                                Console.WriteLine();
                            }
                            Console.WriteLine();
                        }


                        List<Coords> lakesListValid = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(arrayForLakes, landCode_coastalWater, landCode_coastalWater, true, true);
                        foreach (Coords remova in lakesListValid)
                        {
                            landMap[remova.x, remova.y] = landCode_deepWater;
                        }

                    }
                    #endregion




                    List<Coords> finalCheck = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(landMap, landCode_lowLand, 999999, true, true);
                    if (finalCheck.Count < 1)
                    {
                        if (debug_verbose)
                        {
                            Utility.Print.Write("ERROR: REGEN", "91FF00");
                        }
                        goto restartFunction;


                    }

                    //  Do some last minute shaving
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j <= bordermodifierEdgesCol; j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int j = landMap.GetLength(1) - bordermodifierEdgesCol; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    for (int j = 0; j < landMap.GetLength(1); j++)
                    {
                        for (int i = 0; i < bordermodifierEdgesRow; i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int i = landMap.GetLength(0) - bordermodifierEdgesRow; i < landMap.GetLength(0); i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    return landMap;
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

                    bool debug_voronoi = true;
                    bool debug_hills = false;
                    int fileNumber = INITSEED;

                    int[,] naturalMap = sourceMap;

                    //  Save the location of coastlines and oceans to make sure no issues crop up later
                    List<Coords> savedCoasttiles = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(naturalMap, landCode_lowLand, 9999, 1);
                    List<Coords> savedWaterTiles = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(naturalMap, landCode_deepWater, landCode_coastalWater);

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
                    List<List<Coords>> hillsCoords = Utility.Matrices.Selection.SelectCellsWithinRange(arrayforHills, landCode_hillLand, landCode_hillLand);
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
                    hillsCoords = Utility.Matrices.Selection.SelectCellsWithinRange(arrayforHills, landCode_hillLand, landCode_hillLand);
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
                        string filepath = debug_directory + "\\debug\\geogeneration\\_HillsNoise.png";
                        Utility.Images.ImageFile.saveImage(hillBitMap, filepath);

                    }
                    //  Apply hills to the real map
                    List<Coords> hillsSelected = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(arrayforHills, landCode_hillLand, landCode_hillLand);
                    foreach (Coords coord in hillsSelected)
                    {
                        naturalMap[coord.x, coord.y] = landCode_hillLand;
                    }
                    #endregion




                    #region Montane Generation
                    List<List<Coords>> landCoordsMountainGen = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_coastalWater, 99999, mapsizeModifier / 64);
                    List<Coords> seedsForVoronoi = new List<Coords>();

                    foreach (List<Coords> landCoordList in landCoordsMountainGen)
                    {
                        int sizeOfIsland = landCoordList.Count();
                        int sizeOfStartingCoords = 1;

                        if (sizeOfIsland >= mapsizeModifier * 10)
                        {
                            sizeOfStartingCoords = random.Next(5, 10);
                        }
                        else
                        {
                            sizeOfStartingCoords = 1;
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

                    if (debug_voronoi)
                    {
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(voronoiArray);

                        //  Set water to zero
                        List<Coords> waterCoords = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(voronoiArray, 0, 0, true, false);
                        foreach (Coords coord in waterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "000000";
                        }




                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\_IDVoronoi.png");
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);

                    }

                    //  Get voronoi onion slices
                    List<List<Coords>> onionSlices = new List<List<Coords>>();
                    foreach (List<Coords> voronoiSection in voronoiLists)
                    {
                        int[,] voronoiSlice = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiSection, mapRows, mapCols);
                        List<Coords> listEdges = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(voronoiSlice, 1, 99999, 1, true);
                        onionSlices.Add(listEdges);

                    }
                    //  Create mountains
                    foreach (List<Coords> slice in onionSlices)
                    {
                        int randomChance = random.Next(0, 101);
                        if (randomChance > 33)
                        {
                            foreach (Coords onionCoord in slice)
                            {
                                naturalMap[onionCoord.x, onionCoord.y] = landCode_mountain;
                            }
                        }
                    }


                    //  Naturalize Mountaints
                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> mountainArray = new List<List<Coords>>();
                    int chanceCrater = 60;
                    bool cleanup = true;
                    for (int cyclesCount = 0; cyclesCount < 3; cyclesCount++)
                    {
                        mountainArray = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_mountain, landCode_mountain, 1);

                        foreach (List<Coords> mountCoords in mountainArray)
                        {
                            foreach (Coords coord in mountCoords)
                            {
                                //  Generate the probability outwards or inwards
                                int coastlinebulgeProb = random.Next(0, 101);

                                //  Raise Land
                                if (coastlinebulgeProb <= chanceCrater)
                                {
                                    int radius = random.Next(1, 1);

                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(naturalMap, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (naturalMap[circoord.x, circoord.y] >= landCode_lowLand && naturalMap[circoord.x, circoord.y] < landCode_mountain)
                                        {
                                            naturalMap[circoord.x, circoord.y] = landCode_mountain;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    int radius = random.Next(1, 1);
                                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(naturalMap, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (naturalMap[circoord.x, circoord.y] == landCode_mountain)
                                        {
                                            naturalMap[circoord.x, circoord.y] = landCode_Land;
                                        }

                                    }
                                }
                            }
                        }
                    }

                    #endregion



                    if (debug_voronoi)
                    {
                        int[,] voronoiList = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiLists, mapRows, mapCols);
                        string[,] voronoiRepresentation = Utility.Images.ImageDebug.MapValuesToColors(voronoiList);

                        //  Set water to zero
                        List<Coords> waterCoords = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(voronoiList, 0, 0, true, false);
                        foreach (Coords coord in waterCoords)
                        {
                            voronoiRepresentation[coord.x, coord.y] = "000000";
                        }



                        Bitmap voroMap = Utility.Images.ImageFile.StringArrayToBitmap(voronoiRepresentation);
                        string filepath = Utility.Files.GetValidPath("\\debug\\mapScaling\\mapVoronoi" + INITSEED + ".png");
                        Utility.Images.ImageFile.saveImage(voroMap, filepath);

                    }

















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
                    List<Coords> coastalLand = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(naturalMap, landCode_lowLand, 9999, 2);
                    List<Coords> coastlineLand = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(naturalMap, landCode_lowLand, 9999, 0);
                    foreach (Coords coord in coastalLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_Land;
                    }
                    foreach (Coords coord in coastlineLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_lowLand;
                    }
                    #endregion

                    List<List<Coords>> coastWater = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_lowLand, 9999, 1, false);
                    List<List<Coords>> offcoastWater = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_lowLand, 9999, 2, false);
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


                //  Generates a map in 128x256 form
                public static int[,] generateContinentsAndIslands(string[] args)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the dimensions of a map of this specified size
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows = 128;
                    int mapCols = 256;

                    int mapsizeModifier = Convert.ToInt32(((double)(mapRows + mapCols) / 2)); // 640 on a 256x1024 grid




                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion
                    bool verbose = true;
                    bool debug_showgenconts = true;
                    bool debug_printMapsInContinent = false;
                    bool debug_printMapsInBigIslands = true;
                    bool debug_printMapsInBigIslands2 = true;

                restartTheProcess:
                    // generate a world map of initial size
                    int[,] landMap = new int[mapRows, mapCols];
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    int bordermodifierEdgesRow = mapRows / 64;
                    int bordermodifierEdgesCol = mapCols / 64;

                    if (verbose)
                    {
                        Utility.Print.WriteLine("Generating Landform Maps", "8DB953");
                    }

                    #region Generate Large Continents
                    //  Generate a List of continents and extended continents
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
                        if (sizeOfContinent >= 6)
                        {
                            int randomChanceSize = random.Next(0, 101);
                            if (randomChanceSize < 25)
                            {
                                sizeOfContinent = 5;
                            }
                        }


                        int[,] generatedContinent = generateLandMassSmall(passableArgs, sizeOfContinent);
                        if (debug_printMapsInContinent)
                        {
                            for (int i = 0; i < generatedContinent.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "87DBFF");
                                for (int j = 0; j < generatedContinent.GetLength(1); j++)
                                {
                                    if (generatedContinent[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", landColor_Land);
                                    }
                                    else if (generatedContinent[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", landColor_outOfBounds);
                                    }
                                    else if (generatedContinent[i, j] == landCode_deepWater)
                                    {
                                        Utility.Print.Write("  ", landColor_deepWater);
                                    }
                                    else
                                    {
                                        Utility.Print.Write("??", landColor_error);
                                    }
                                }
                                Utility.Print.Write("|<", "87DBFF");
                                Console.WriteLine();
                            }
                            Console.WriteLine();
                        }

                        //  Select its Coords and collapse it to a list of coords
                        List<List<Coords>> mapCoordsLoL = Utility.Matrices.Selection.SelectCellsWithinRange(generatedContinent, landCode_lowLand, 9999);
                        List<Coords> mapCoords = Utility.Lists.CollapseLists(mapCoordsLoL);
                        //  Select its Coords edges and collapse it to a list of coords
                        List<List<Coords>> mapCoordseExtended = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(generatedContinent, landCode_lowLand, 9999, 3, false);
                        List<Coords> mapCoordsExtended = Utility.Lists.CollapseLists(mapCoordseExtended);
                        //  Combine extended with Coords
                        mapCoordsExtended.AddRange(mapCoords);


                        //  Add them to the lists
                        continentsList.Add(mapCoords);
                        continentsListExtended.Add(mapCoordsExtended);

                        if (debug_showgenconts)
                        {
                            int[,] printableVersion = new int[generatedContinent.GetLength(0), generatedContinent.GetLength(1)];
                            for (int i = 0; i < printableVersion.GetLength(0); i++)
                            {
                                for (int j = 0; j < printableVersion.GetLength(1); j++)
                                {
                                    printableVersion[i, j] = landCode_deepWater;
                                }
                            }

                            foreach (Coords coordd in mapCoordsExtended)
                            {
                                printableVersion[coordd.x, coordd.y] = landCode_outOfBounds;
                            }
                            foreach (Coords coordd in mapCoords)
                            {
                                printableVersion[coordd.x, coordd.y] = landCode_Land;
                            }

                            for (int i = 0; i < printableVersion.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "87FFE7");
                                for (int j = 0; j < printableVersion.GetLength(1); j++)
                                {
                                    if (printableVersion[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", "87FFE7");
                                    }
                                    else if (printableVersion[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", "FF0000");
                                    }
                                    else if (printableVersion[i, j] == landCode_deepWater)
                                    {
                                        Utility.Print.Write("  ", "FF0000");
                                    }
                                    else
                                    {
                                        Utility.Print.Write("??", "FF00FB");
                                    }
                                }
                                Utility.Print.Write("|<", "87DBFF");
                                Console.WriteLine();
                            }
                            Console.WriteLine();

                        }


                    }



                    //  Sort landmasses by size
                    int placementsCount = 50;
                    if (verbose)
                    {
                        Utility.Print.WriteLine("Looking for valid placements", "8DB953");
                    }
                    List<Coords[]> placements = Utility.Matrices.Complex.Packing.FindValidPlacements(landMap.GetLength(0), landMap.GetLength(1), continentsListExtended, placementsCount);
                    if (verbose)
                    {
                        Utility.Print.WriteLine("Placements found: generating map", "8DB953");
                    }
                    if (placements.Count < 1)
                    {
                        if (verbose)
                        {
                            Utility.Print.WriteLine("No placements found: restarting", "B95353");
                        }
                        goto restartTheProcess;
                    }
                    int randIndex = random.Next(0, placements.Count);
                    Coords[] placement = placements[randIndex];


                    //Make the placeholder map
                    int[,] placeholdermap = new int[landMap.GetLength(0), landMap.GetLength(1)];
                    for (int i = 0; i < placeholdermap.GetLength(0); i++)
                    {
                        for (int j = 0; j < placeholdermap.GetLength(1); j++)
                        {
                            placeholdermap[i, j] = landCode_deepWater;
                        }
                    }
                    for (int count = 0; count < placement.GetLength(0); count++)
                    {
                        List<Coords> continentSelected = continentsList[count];
                        Coords newStartingPos = placement[count];
                        continentSelected = Utility.Matrices.Geometry.TranslateSection(continentSelected, landMap.GetLength(0), landMap.GetLength(1), newStartingPos);
                        foreach (Coords coords in continentSelected)
                        {
                            landMap[coords.x, coords.y] = landCode_Land;
                        }

                        if (debug_printMapsInContinent)
                        {
                            //  Load each continent, show when overlapping occurs
                            foreach (Coords coords in continentSelected)
                            {
                                if (placeholdermap[coords.x, coords.y] == landCode_Land)
                                {
                                    placeholdermap[coords.x, coords.y] = landCode_outOfBounds;
                                }
                                else
                                {
                                    placeholdermap[coords.x, coords.y] = landCode_Land;
                                }
                            }
                            //  Print this map
                            for (int i = 0; i < placeholdermap.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "87FFE7");
                                for (int j = 0; j < placeholdermap.GetLength(1); j++)
                                {
                                    if (placeholdermap[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", "87FFE7");
                                    }
                                    else if (placeholdermap[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", "FF0000");
                                    }
                                    else if (placeholdermap[i, j] == landCode_deepWater)
                                    {
                                        Utility.Print.Write("  ", "FF0000");
                                    }
                                    else
                                    {
                                        Utility.Print.Write("??", "FF00FB");
                                    }
                                }
                                Utility.Print.Write("|<", "87DBFF");
                                Console.WriteLine();
                            }

                        }
                    }
                    #endregion


                    #region Generate Big Islands

                    //  Calculate valid locations for islands
                    int[,] BitIslandAreaMap = new int[landMap.GetLength(0), landMap.GetLength(1)];
                    for (int i = 0; i < BitIslandAreaMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < BitIslandAreaMap.GetLength(1); j++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                    }
                    List<Coords> areaEdgesClose = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(landMap, landCode_lowLand, 9999, mapsizeModifier / 128, false);
                    List<Coords> areaEdgesGood = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(landMap, landCode_lowLand, 9999, mapsizeModifier / 8, false);
                    List<Coords> areaEdgesFar = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(landMap, landCode_lowLand, 9999, mapsizeModifier / 4, false);
                    foreach (Coords areaEdge in areaEdgesFar)
                    {
                        BitIslandAreaMap[areaEdge.x, areaEdge.y] = landCode_Land;
                    }
                    foreach (Coords areaEdge in areaEdgesGood)
                    {
                        BitIslandAreaMap[areaEdge.x, areaEdge.y] = landCode_deepWater;
                    }
                    foreach (Coords areaEdge in areaEdgesClose)
                    {
                        BitIslandAreaMap[areaEdge.x, areaEdge.y] = landCode_Land;
                    }
                    //  Do some last minute shaving
                    for (int i = 0; i < BitIslandAreaMap.GetLength(0); i++)
                    {
                        for (int j = 0; j <= bordermodifierEdgesCol; j++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                        for (int j = BitIslandAreaMap.GetLength(1) - bordermodifierEdgesCol; j < BitIslandAreaMap.GetLength(1); j++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                    }
                    for (int j = 0; j < BitIslandAreaMap.GetLength(1); j++)
                    {
                        for (int i = 0; i < bordermodifierEdgesRow; i++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                        for (int i = BitIslandAreaMap.GetLength(0) - bordermodifierEdgesRow; i < BitIslandAreaMap.GetLength(0); i++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                    }
                    if (debug_printMapsInBigIslands2)
                    {
                        Utility.Print.Write("Region where islands are valid is highligheted below", "FFA800");
                        for (int i = 0; i < BitIslandAreaMap.GetLength(0); i++)
                        {

                            Utility.Print.Write(">|", "FFA800");
                            for (int j = 0; j < BitIslandAreaMap.GetLength(1); j++)
                            {
                                if (BitIslandAreaMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "FF0000", "FF0000");
                                }
                                else if (BitIslandAreaMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (BitIslandAreaMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("[]", "00FF03", "00FF03");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "FFA800");
                            Console.WriteLine();
                        }
                        Console.WriteLine();

                    }



                    List<Coords> validStarts = Utility.Matrices.Selection.SelectCellsWithinRangeCollapsed(BitIslandAreaMap, landCode_deepWater, landCode_deepWater);
                    foreach (Coords isCoords in validStarts)
                    {
                        int randomChance = random.Next(0, 101);
                        if (randomChance < 5)
                        {
                            landMap[isCoords.x, isCoords.y] = landCode_Land;
                        }
                    }

                    #endregion









                    #region Centralize the map
                    //  Get coords centralized
                    List<List<Coords>> allLandLoL = Utility.Matrices.Selection.SelectCellsWithinRange(landMap, landCode_lowLand, 999999);
                    List<Coords> allLand = Utility.Lists.CollapseLists(allLandLoL);
                    Coords newCenter = new Coords(Convert.ToInt32(landMap.GetLength(0) / 2), Convert.ToInt32(landMap.GetLength(1) / 2));
                    List<Coords> translated = Utility.Matrices.Geometry.TranslateSection(allLand, landMap.GetLength(0), landMap.GetLength(1), newCenter);
                    //  Reset the map and repaint
                    landMap = new int[landMap.GetLength(0), landMap.GetLength(1)];
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    foreach (Coords translatedCoords in translated)
                    {
                        landMap[translatedCoords.x, translatedCoords.y] = landCode_Land;
                    }
                    if (debug_printMapsInContinent)
                    {
                        for (int i = 0; i < landMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "38FFD8");
                            for (int j = 0; j < landMap.GetLength(1); j++)
                            {
                                if (landMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "38FFD8");
                                }
                                else if (landMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (landMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FF0000");
                                }
                                else
                                {
                                    Utility.Print.Write("??", "FF00FB");
                                }
                            }
                            Utility.Print.Write("|<", "38FFD8");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    #endregion







                    //  Do some last minute shaving
                    for (int i = 0; i < landMap.GetLength(0); i++)
                    {
                        for (int j = 0; j <= bordermodifierEdgesCol; j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int j = landMap.GetLength(1) - bordermodifierEdgesCol; j < landMap.GetLength(1); j++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }
                    for (int j = 0; j < landMap.GetLength(1); j++)
                    {
                        for (int i = 0; i < bordermodifierEdgesRow; i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                        for (int i = landMap.GetLength(0) - bordermodifierEdgesRow; i < landMap.GetLength(0); i++)
                        {
                            landMap[i, j] = landCode_deepWater;
                        }
                    }


                    return landMap;
                }


                #endregion






                #region Functions which scale maps to new size while randomizing borders
                public static int[,] scaleLandmapToNewSizeWithoutInterp(string[] args, int[,] grid, double sizeMultiplier)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    int mapRows = grid.GetLength(0);
                    int mapCols = grid.GetLength(1);
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2));
                    #endregion


                    int debug_debugNumber = INITSEED;


                    int[,] sourceMap = grid;
                    int[,] modifiedMap = grid;



                    //  Scale the map to a new size
                    int newRowSize = Convert.ToInt32(sourceMap.GetLength(0) * Math.Pow(2, sizeMultiplier));
                    if (newRowSize % 2 != 0) // Check if the remainder when divided by 2 is not 0
                    {
                        newRowSize--;
                    }
                    int newColSize = newRowSize * 2;
                    modifiedMap = Utility.Matrices.Misc.ScaleArray(modifiedMap, newRowSize, newColSize);


                    //  Perform coastlinification
                    for (int count = 0; count < 1; count++)
                    {
                        //List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(modifiedMap, landCode_Land, landCode_Land, amplitude: 3.0, frequency: 4.5, smoothness: 5, random.Next(-99999, 99999));
                        // RandomizeBorders(landMap, landCode_Land, landCode_Land, amplitude: 2.5, frequency: 1, smoothness: 15, random.Next(-99999, 99999))
                        List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(modifiedMap, landCode_Land, landCode_Land, amplitude: 10, frequency: 15, smoothness: 5, random.Next(-99999, 99999), 8, 0.5);

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
                    }


                    //  Select all islands
                    List<List<Coords>> alllandmass_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(modifiedMap, landCode_Land, landCode_Land);
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


                    //  Remove small water holes
                    List<List<Coords>> alllwater_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(modifiedMap, landCode_deepWater, landCode_deepWater, true, true);
                    alllwater_removalbits = Utility.Lists.SortBySubListSize(alllwater_removalbits, false);
                    alllwater_removalbits = Utility.Lists.RemoveListBelowSize(alllwater_removalbits, alllwater_removalbits.Count);
                    foreach (List<Coords> allwater_shape in alllwater_removalbits)
                    {
                        foreach (Coords cellin in allwater_shape)
                        {
                            //modifiedMap[cellin.x, cellin.y] = landCode_Land;
                        }
                    }

                    return modifiedMap;
                }

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
                        List<List<Coords>> alllandmass_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(modifiedMap, landCode_Land, landCode_Land);
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
