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
        private static int landCode_deepWater = 25;
        private static int landCode_subcoastWater = 29;
        private static int landCode_coastalWater = 30;

        private static readonly int landCode_outOfBounds = 15;
        private static readonly int landCode_lowLand = 40;
        private static readonly int landCode_Land = 41;
        private static readonly int landCode_hillLand = 42;
        private static readonly int landCode_mountain = 50;

        private static readonly double landGen_lakeSizeModifier = 0.5;
        public partial class GeographyGenerator
        {



            public class LandGenerator
            {

                public static int[,] GenerateLandMap(string[] args)
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
                    
                    string directoryLocation = Utility.Files.GetValidPath("\\debug\\geogeneration\\worldseed" + INITSEED + "\\");
                    if (debug_showProcess)
                    {
                        Utility.Files.CreateDirectory(directoryLocation);
                    }
                    


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

                        filepath = directoryLocation + "\\mapseed.png";
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
                    LandMap = scaleLandmapToNewSizeWithoutInterp(args, LandMap, sizeMultiplier);
                    


                    //  Naturalize Map
                    LandMap = GeographyGenerator.LandGenerator.naturalizeLandmap(args, LandMap);
                    if (debug_showProcess)
                    {
                        //filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\_ID" + INITSEED + "scaled_" + LandMap.GetLength(0) + "x" + LandMap.GetLength(1) + "_" + (sizeMultiplier) + ".png");
                        filepath = directoryLocation + "scale" + + LandMap.GetLength(0) + "x" + LandMap.GetLength(1) + "_sizeMult" + (sizeMultiplier)+ ".png";
                        Bitmap image = WorldGen.GeographyGenerator.LandGenerator.createLandBitmap(LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }
                   


                    return LandMap;
                }
                
                


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
                                landBitmapString[i, j] = "73B349";
                            }
                            else if (landMap[i, j] == landCode_lowLand)
                            {
                                landBitmapString[i, j] = "A8B349";
                            }
                            else if (landMap[i, j] == landCode_hillLand) 
                            {
                                landBitmapString[i, j] = "B38949";
                            }
                            else if (landMap[i, j] == landCode_mountain)
                            {
                                landBitmapString[i, j] = "858585";
                            }

                            else if (landMap[i, j] == landCode_coastalWater)
                            {
                                landBitmapString[i, j] = "009FFF";
                            }
                            else if (landMap[i, j] == landCode_subcoastWater)
                            {
                                landBitmapString[i, j] = "138FE5";
                            }
                            else if (landMap[i, j] == landCode_deepWater)
                            {
                                landBitmapString[i, j] = "1B80CB";
                            }

                            else if (landMap[i, j] == landCode_outOfBounds)
                            {
                                landBitmapString[i, j] = "FF0000";
                            }
                            else
                            {
                                landBitmapString[i, j] = "FF52FB";
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
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier / 64));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 1));
                            perlinFrequency = 15;
                            break;
                        case 2: //  Medium Sized Island
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 1));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 2.5));
                            break;
                        case 3: //  Large Island
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 4));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 7));
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
                            break;
                        case 7:
                            break;


                        default:
                            //  Get Continents of a small size, by default
                            minSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeLimit = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                    }
                    bool isValidContinent = false;



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

                        foreach (Coords coord in validContinent) {
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



                    //  Use the randomize border map
                    List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(landMap, landCode_Land, landCode_Land, amplitude: 2.5, frequency: 1, smoothness: 15, random.Next(-99999, 99999)); 
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

                    int[,] naturalMap = sourceMap;

                    //  Create the seeds for the voronoi
                    Console.WriteLine(mapsizeModifier);
                    List<List<Coords>> landCoords = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_coastalWater, 99999, mapsizeModifier / 64);

                    //  Remove landmasses below size
                    //landCoords = Utility.Lists.RemoveListBelowSize(landCoords, mapsizeModifier / 41);


                    List<Coords> seedsForVoronoi = new List<Coords>();

                    foreach (List<Coords> landCoordList in landCoords)
                    {
                        int sizeOfIsland = landCoordList.Count();
                        int sizeOfStartingCoords = 3;

                        if (sizeOfIsland >= mapsizeModifier * 10)
                        {
                            sizeOfStartingCoords = random.Next(10, 15);
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
                        string filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\_ID" + INITSEED + "Voronoi.png");
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


                    //  Create natural mountain borders
                    List<List<Coords>> mountainArray = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(naturalMap, landCode_mountain, landCode_mountain, 1, true);
                    foreach (List<Coords> mountainEdge in mountainArray)
                    {
                        foreach (Coords coord in mountainEdge)
                        {
                            int randomChance = random.Next(0, 101);
                            if (randomChance > 33)
                            {
                                List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(naturalMap, coord, 3);
                                foreach (Coords circoord in circoords)
                                {
                                    if (naturalMap[coord.x, coord.y] > landCode_lowLand)
                                    {
                                        naturalMap[coord.x, coord.y] = landCode_Land;
                                    }
                                    
                                }
                                
                            } 
                            else
                            {
                                List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(naturalMap, coord, 3);
                                foreach (Coords circoord in circoords)
                                {
                                    if (naturalMap[coord.x, coord.y] > landCode_lowLand)
                                    {
                                        naturalMap[coord.x, coord.y] = landCode_mountain;
                                    }
                                    
                                }
                                
                            }
                            
                        }
                    }











                    List<Coords> coastalLand = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(naturalMap, landCode_lowLand, 9999, mapsizeModifier / 64);
                    List<Coords> coastlineLand = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(naturalMap, landCode_lowLand, 9999, 1);
                    foreach (Coords coord in coastalLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_Land;
                    }
                    foreach (Coords coord in coastlineLand)
                    {
                        naturalMap[coord.x, coord.y] = landCode_lowLand;
                    }



                    //   mapsizeModifier / 64
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



                //  Given a land map, raise it to a new height and create borders
                public static int[,] scaleLandmapToNewSize(string[] args, int[,] grid, int sizeCategory)
                {
                    #region Parameters for Landform Generation
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    #endregion

                    bool debug_saveMap = true;
                    string debug_debugNumber = args[index_seed];

                    int[,] sourceMap = grid;
                    int[,] modifiedMap = grid;
                    double newRow = grid.GetLength(0);
                    double newCol = grid.GetLength(1);
                    if (debug_saveMap)
                    {
                        string[,] colorArray = new string[sourceMap.GetLength(0), sourceMap.GetLength(1)];
                        for (int i = 0; i < sourceMap.GetLength(0); i++)
                        {
                            for (int j = 0; j < sourceMap.GetLength(1); j++)
                            {
                                if (sourceMap[i, j] == landCode_Land)
                                {
                                    colorArray[i, j] = "73B349";
                                }
                                else if (sourceMap[i, j] == landCode_deepWater)
                                {
                                    colorArray[i, j] = "498AB3";
                                }
                                else if (sourceMap[i, j] == landCode_outOfBounds)
                                {
                                    colorArray[i, j] = "FF0000";
                                }
                                else
                                {
                                    colorArray[i, j] = "FF00FB";
                                }
                            }

                        }
                        Bitmap image = Utility.Images.ImageFile.StringArrayToBitmap(colorArray);
                        string filepath = Utility.Files.GetValidPath("\\debug\\geogeneration\\mapBeforeScaled_ID" + debug_debugNumber + ".png");
                        
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }

                    for (int counter = 0; counter < sizeCategory; counter++)
                    {
                        //  Get newly sized map
                        int rowSize = modifiedMap.GetLength(0);
                        int colSize = modifiedMap.GetLength(1);
                        if (debug_saveMap)
                        {
                            Console.WriteLine("Counter: " + counter + ";" + rowSize + "," + colSize);
                        }
                            
                        modifiedMap = Utility.Matrices.Misc.ScaleArray(modifiedMap, rowSize*2, colSize*2);

                        //  Modify the borders
                        List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(modifiedMap, landCode_Land, landCode_Land, amplitude: 0.0, frequency: 0, smoothness: 15, random.Next(-99999, 99999));
                        foreach (Coords coordinates in coordsList_border[0])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_deepWater;
                        }
                        foreach (Coords coordinates in coordsList_border[1])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_Land;
                        }
                        //  Break up the edges slightly
                        List<List<Coords>> landmapBordersLoL = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(modifiedMap, landCode_lowLand, 9999, 1);
                        List<Coords> landmapBorders = Utility.Lists.CollapseLists(landmapBordersLoL);
                        int countOfCyclesBorder = random.Next(3, 5);
                        for (int countCycles = 0; countCycles < countOfCyclesBorder; countCycles++)
                        {
                            foreach (Coords borderCoords in landmapBorders)
                            {
                                List<Coords> circleRegion = Utility.Matrices.Selection.SelectCircleRegion(modifiedMap, borderCoords, 2);
                                int randomChance = random.Next(0, 101);
                                if (randomChance < 45)
                                {
                                    foreach (Coords borderCirCoords in circleRegion)
                                    {
                                        if (modifiedMap[borderCirCoords.x, borderCirCoords.y] == landCode_Land)
                                        {
                                            modifiedMap[borderCirCoords.x, borderCirCoords.y] = landCode_deepWater;
                                        }

                                    }

                                }
                                else if (randomChance >= 45 && randomChance < 66)
                                {
                                    foreach (Coords borderCirCoords in circleRegion)
                                    {
                                        if (modifiedMap[borderCirCoords.x, borderCirCoords.y] == landCode_deepWater)
                                        {
                                            modifiedMap[borderCirCoords.x, borderCirCoords.y] = landCode_Land;
                                        }

                                    }
                                }

                            }
                        }
                        //  Remove any immediate small islands
                        List<List<Coords>> alllandmass_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(modifiedMap, landCode_Land, landCode_Land);
                        alllandmass_removalbits = Utility.Lists.SortBySubListSize(alllandmass_removalbits, false);
                        alllandmass_removalbits = Utility.Lists.RemoveListBelowSize(alllandmass_removalbits, alllandmass_removalbits.Count);
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
                                modifiedMap[cellin.x, cellin.y] = landCode_Land;
                            }
                        }
                        

                    }

                    if (debug_saveMap)
                    {
                        string[,] colorArray = new string[modifiedMap.GetLength(0), modifiedMap.GetLength(1)];
                        Console.WriteLine(modifiedMap.GetLength(0) + "," + modifiedMap.GetLength(1));
                        for (int i = 0; i < modifiedMap.GetLength(0); i++)
                        {
                            
                            for (int j = 0; j < modifiedMap.GetLength(1); j++)
                            {
                                if (modifiedMap[i, j] == landCode_Land)
                                {
                                    colorArray[i, j] = "73B349";
                                }
                                else if (modifiedMap[i, j] == landCode_deepWater)
                                {
                                    colorArray[i, j] = "498AB3";
                                }
                                else if (modifiedMap[i, j] == landCode_outOfBounds)
                                {
                                    colorArray[i, j] = "FF0000";
                                }
                                else
                                {
                                    colorArray[i, j] = "FF00FB";
                                }
                            }
                            
                        }
                        Bitmap image =  Utility.Images.ImageFile.StringArrayToBitmap(colorArray);
                        string filepath = Utility.Files.GetValidPath("\\debug\\mapScaling\\mapScaledID" + debug_debugNumber + "_x" + (sizeCategory+1) + ".png");
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }



                    return modifiedMap;
                }

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
                    int newColSize = newRowSize*2;
                    modifiedMap = Utility.Matrices.Misc.ScaleArray(modifiedMap, newRowSize, newColSize);





                    //  Perform coastlinification
                    for (int count = 0; count < 2; count++)
                    {
                        List<List<Coords>> coordsList_border = Utility.Matrices.Complex.BorderRandomizer.RandomizeBorders(modifiedMap, landCode_Land, landCode_Land, amplitude: 1.0f, frequency: 2.5f, smoothness: 20f, random.Next(-99999, 99999));
                        foreach (Coords coordinates in coordsList_border[0])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_deepWater;
                        }
                        foreach (Coords coordinates in coordsList_border[1])
                        {
                            modifiedMap[coordinates.x, coordinates.y] = landCode_Land;
                        }
                    }
                    //  Break up the edges slightly
                    List<List<Coords>> landmapBordersLoL = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(modifiedMap, landCode_lowLand, 9999, 1);
                    List<Coords> landmapBorders = Utility.Lists.CollapseLists(landmapBordersLoL);
                    int countOfCyclesBorder = random.Next(1, 3);
                    for (int countCycles = 0; countCycles < countOfCyclesBorder; countCycles++)
                    {
                        foreach (Coords borderCoords in landmapBorders)
                        {
                            List<Coords> circleRegion = Utility.Matrices.Selection.SelectCircleRegion(modifiedMap, borderCoords, 2);
                            int randomChance = random.Next(0, 101);
                            if (randomChance < 45)
                            {
                                foreach (Coords borderCirCoords in circleRegion)
                                {
                                    if (modifiedMap[borderCirCoords.x, borderCirCoords.y] == landCode_Land)
                                    {
                                        modifiedMap[borderCirCoords.x, borderCirCoords.y] = landCode_deepWater;
                                    }

                                }

                            }
                            else if (randomChance >= 45 && randomChance < 66)
                            {
                                foreach (Coords borderCirCoords in circleRegion)
                                {
                                    if (modifiedMap[borderCirCoords.x, borderCirCoords.y] == landCode_deepWater)
                                    {
                                        modifiedMap[borderCirCoords.x, borderCirCoords.y] = landCode_Land;
                                    }

                                }
                            }

                        }
                    }

                    //  Select all islands
                    List<List<Coords>> alllandmass_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(modifiedMap, landCode_Land, landCode_Land);
                    //  Sort islands by size
                    alllandmass_removalbits = Utility.Lists.SortBySubListSize(alllandmass_removalbits, false);
                    //  Remove all islands below a certain size
                    
                    int sizeMod = Convert.ToInt32((double)((modifiedMap.GetLength(0) + modifiedMap.GetLength(1)) / 2));
                    alllandmass_removalbits = Utility.Lists.RemoveListBelowSize(alllandmass_removalbits,Convert.ToInt32(((double)sizeMod) / 15.00));
                    
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
                    bool debug_printMapsInBigIslands = false;
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
                                        Utility.Print.Write("[]", "87DBFF");
                                    }
                                    else if (generatedContinent[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", "FF0000");
                                    }
                                    else if (generatedContinent[i, j] == landCode_deepWater)
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





                    //  Create big islands
                    List<List<Coords>> bigIslands = new List<List<Coords>>();
                    for (int count = 0; count < 10; count++)
                    {
                        string[] bigIslandArgs = args;
                        bigIslandArgs[index_seed] = Convert.ToString(random.Next(-99999, 99999));
                        int sizeCategory = random.Next(1, 2);
                        int[,] bigIsland = LandGenerator.generateLandMassSmall(bigIslandArgs, sizeCategory);

                        if (debug_printMapsInBigIslands)
                        {
                            for (int i = 0; i < bigIsland.GetLength(0); i++)
                            {
                                Utility.Print.Write(">|", "87DBFF");
                                for (int j = 0; j < bigIsland.GetLength(1); j++)
                                {
                                    if (bigIsland[i, j] == landCode_Land)
                                    {
                                        Utility.Print.Write("[]", "87DBFF");
                                    }
                                    else if (bigIsland[i, j] == landCode_outOfBounds)
                                    {
                                        Utility.Print.Write("XX", "FF0000");
                                    }
                                    else if (bigIsland[i, j] == landCode_deepWater)
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
                    //  Create valid island map
                    int[,] BitIslandAreaMap = new int[landMap.GetLength(0), landMap.GetLength(1)];
                    for (int i = 0; i < BitIslandAreaMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < BitIslandAreaMap.GetLength(1); j++)
                        {
                            BitIslandAreaMap[i, j] = landCode_Land;
                        }
                    }
                    List<Coords> areaEdgesClose = Utility.Matrices.Selection.SelectCellsWithinRangeEdgeCollapsed(landMap, landCode_lowLand, 9999, mapsizeModifier/128 ,false);
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
                        for (int i = 0; i < BitIslandAreaMap.GetLength(0); i++)
                        {
                            Utility.Print.Write(">|", "FFA800");
                            for (int j = 0; j < BitIslandAreaMap.GetLength(1); j++)
                            {
                                if (BitIslandAreaMap[i, j] == landCode_Land)
                                {
                                    Utility.Print.Write("[]", "FFA800");
                                }
                                else if (BitIslandAreaMap[i, j] == landCode_outOfBounds)
                                {
                                    Utility.Print.Write("XX", "FF0000");
                                }
                                else if (BitIslandAreaMap[i, j] == landCode_deepWater)
                                {
                                    Utility.Print.Write("  ", "FFA800");
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
                    //  Remove small water holes
                    List<List<Coords>> alllwater_removalbits = Utility.Matrices.Selection.SelectCellsWithinRange(landMap, landCode_deepWater, landCode_deepWater, true, true);
                    alllwater_removalbits = Utility.Lists.SortBySubListSize(alllwater_removalbits, false);
                    int sizeMod = Convert.ToInt32((double)((landMap.GetLength(0) + landMap.GetLength(1)) / 2));
                    alllwater_removalbits = Utility.Lists.RemoveListBelowSize(alllwater_removalbits, Convert.ToInt32(((double)sizeMod) / 123.00));
                    foreach (List<Coords> allwater_shape in alllwater_removalbits)
                    {
                        foreach (Coords cellin in allwater_shape)
                        {
                            landMap[cellin.x, cellin.y] = landCode_Land;
                        }
                    }
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



            }
        }
    }
}
