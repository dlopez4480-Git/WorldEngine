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

        static void testWorldPacking()
        {
            #region MapCodes
            int landCode_deepWater = 25;
            int landCode_offcoastWater = 29;
            int landCode_coastalWater = 30;
            int landCode_coastalLand = 40;
            int landCode_Land = 41;
            int landCode_hillLand = 42;
            int landCode_mountain = 50;

            #endregion

            //  Setup
            #region Important Parameters
            Random rand = new Random();
            String randseed = Convert.ToString(rand.Next(-999999, 9999999));
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
            int[,] canvas = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);

            //  Get the landCode cells
            List<PrintableCell> landCodeCellsKey = new List<PrintableCell> {

                //  Water
                new PrintableCell(landCode_deepWater,     " ", "#000000", "#000050"),

                new PrintableCell(landCode_offcoastWater, " ", "#0D0D73", "#0D0D73"),

                new PrintableCell(landCode_coastalWater,  " ", "#151585", "#151585"),


                //  Land
                new PrintableCell(landCode_coastalLand,   "~", "#ffff00", "0D660D"),

                new PrintableCell(landCode_Land,          " ", "#000000", "0D660D"),

                new PrintableCell(landCode_hillLand,      "m", "#00b700", "0D660D"),

                new PrintableCell(landCode_mountain,      "@", "#DEDEDE", "4D4D4D")

            };
            PrintableCell errorPCell = new PrintableCell(null, "X", "#000000", "FF00F2");

            #endregion





            //  Amount of worlds to test
            int worldStackAmount = 3;
            //  Size of world to test
            int worldSizeVal = 5;
            Utility.Print.Write("Generating a worldStack of size ", "00FAFF", "000000");
            Utility.Print.Write(worldStackAmount, "#FF2BFF", "#000000");
            Console.WriteLine();
            bool printModified = true;


            //  Generate a list of potential continents
            List<List<Coords>> worldLists = new List<List<Coords>>();
            for (int worldCounter = 0; worldCounter < worldStackAmount; worldCounter++)
            {
                //  Generate a random seed for each map
                randseed = Convert.ToString(rand.Next(-999999, 9999999));
                String[] argsGeneration = {
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

                //  Generate and print a map
                int[,] testingarr = WorldGen.GeographyGenerator.GeographyGenComponents.GenerateLandmassNoise(argsGeneration, worldSizeVal);

                //  Select the map and add it to the list
                List<List<Coords>> mapCoordsLoL = Utility.Matrices.Selection.SelectSections(testingarr, landCode_coastalLand, 9999, false, false);
                List<Coords> mapCooords = Utility.Lists.CollapseLists(mapCoordsLoL);


                worldLists.Add(mapCooords);
            }


            //  Print the worldStack
            for (int count = 0; count < worldLists.Count; count++)
            {
                int[,] tester = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);
                List<Coords> continent = worldLists[count];
                foreach (Coords coordinates in continent)
                {
                    tester[coordinates.x, coordinates.y] = landCode_Land;

                }
                Console.WriteLine("Continent num: " + count + " generated ");
                Console.WriteLine();
                WorldGen.GeographyGenerator.GeographyGenerator.PrintIDMap(args, tester);
                Console.WriteLine();

            }
            Console.WriteLine("Completed Generation");



            //  Extend the map
            List<List<Coords>> packingList = new List<List<Coords>>();
            int gap = 2;
            foreach (List<Coords> worldCoords in worldLists)
            {
                List<List<Coords>> passableList = new List<List<Coords>>();
                passableList.Add(worldCoords);
                int[,] passableArr = Utility.Matrices.Misc.ConvertCoordstoArray(passableList, canvas.GetLength(0), canvas.GetLength(1));

                List<List<Coords>> borderedLists = Utility.Matrices.Selection.SelectionSectionEdges(passableArr, 1, 99999, gap);
                List<Coords> borderedList = Utility.Lists.CollapseLists(borderedLists);

                packingList.Add(borderedList);
            }



            //  Perform the packing algorithm
            List<Coords[]> startingValues = Utility.Matrices.Complex.Packing.FindValidPlacements(canvas.GetLength(0), canvas.GetLength(1), packingList, 15);



            //  Verify the list works
            Console.WriteLine("Current list count: " + startingValues.Count());
            //  Get a random coords list
            int randIndex = rand.Next(0, startingValues.Count());
            Coords[] chosenCoords = startingValues[randIndex];

            //  Add the conts to the map
            for (int count = 0; count < chosenCoords.GetLength(0); count++)
            {
                //  Get the list of Coords at that place
                List<Coords> translatableCoords = worldLists[count];

                //  Get the starting Coords for the respective start
                Coords startingCoords = chosenCoords[count];

                //  Get the translated Coords
                //  DNT
                List<Coords> newPosition = Utility.Matrices.Transformation.TranslateSection(translatableCoords, canvas.GetLength(0), canvas.GetLength(1), startingCoords);

                // Translate the Coords
                foreach (Coords coordinates in newPosition)
                {
                    canvas[coordinates.x, coordinates.y] = landCode_Land;
                }


            }





            // Print the new packed MAP
            WorldGen.GeographyGenerator.PrintIDMap(args, canvas);
            int[,] climateMap = WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.generateTemperatureMap(args, canvas);
            WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.printTemperatureMap(args, climateMap, canvas, true);



        }

        static void testTransformation()
        {
            #region MapCodes
            int landCode_deepWater = 25;
            int landCode_offcoastWater = 29;
            int landCode_coastalWater = 30;
            int landCode_coastalLand = 40;
            int landCode_Land = 41;
            int landCode_hillLand = 42;
            int landCode_mountain = 50;

            #endregion
            Random rand = new Random();
            String randseed = Convert.ToString(rand.Next(-999999, 9999999));
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

            int[,] testingarr = WorldGen.GeographyGenerator.GeographyGenComponents.GenerateLandmassNoise(args, 5);
            WorldGen.GeographyGenerator.PrintIDMap(args, testingarr);
            //  Get a coords list
            List<List<Coords>> coordsConts = Utility.Matrices.Selection.SelectSections(testingarr, landCode_coastalLand, 9999, false, false);
            List<Coords> coords = Utility.Lists.CollapseLists(coordsConts);
            coords.Add(Utility.Matrices.Transformation.GetStartingPoint(coords));

            //  Print the original Map
            int[,] originalMap = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);
            foreach (Coords coordsTile in coords)
            {
                originalMap[coordsTile.x, coordsTile.y] = landCode_Land;
            }
            WorldGen.GeographyGenerator.PrintIDMap(args, originalMap);




            //  Create a list of translated Coords
            List<Coords> translatedCoords = Utility.Matrices.Transformation.TranslateSection(coords, testingarr.GetLength(0), testingarr.GetLength(1), new Coords(testingarr.GetLength(0) / 2, testingarr.GetLength(1) / 2));
            int[,] testTranslation = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);
            foreach (Coords coordsTile in translatedCoords)
            {
                testTranslation[coordsTile.x, coordsTile.y] = landCode_Land;
            }
            WorldGen.GeographyGenerator.PrintIDMap(args, testTranslation);

            //  Create a list of reflected coords
            List<Coords> reflectedCoords = Utility.Matrices.Transformation.ReflectSection(coords, testingarr.GetLength(0), testingarr.GetLength(1), true, true);
            int[,] testReflection = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);
            foreach (Coords coordsTile in reflectedCoords)
            {
                testReflection[coordsTile.x, coordsTile.y] = landCode_Land;
            }
            WorldGen.GeographyGenerator.PrintIDMap(args, testReflection);

            //  Create a list of rotated coords
            List<Coords> rotatedCoords = Utility.Matrices.Transformation.RotateSection(coords, testingarr.GetLength(0), testingarr.GetLength(1), 3);
            int[,] testRotation = WorldGen.GeographyGenerator.GeographyGenComponents.getOceanicArray(args);
            foreach (Coords coordsTile in rotatedCoords)
            {
                testRotation[coordsTile.x, coordsTile.y] = landCode_Land;
            }
            WorldGen.GeographyGenerator.PrintIDMap(args, testRotation);







        }


        static void testGradient()
        {
            Random random = new Random();
            int randomSeed = random.Next(-9999, 9999);

            #region MapCodes
            int landCode_deepWater = 25;
            int landCode_offcoastWater = 29;
            int landCode_coastalWater = 30;
            int landCode_coastalLand = 40;
            int landCode_Land = 41;
            int landCode_hillLand = 42;
            int landCode_mountain = 50;

            #endregion





            Console.WriteLine("");
            Console.WriteLine("Random Seed: " + randomSeed);
            Console.WriteLine("");



            Console.WriteLine("");
            Console.WriteLine("MAP");


            Console.WriteLine("");
            String[] args = {
                Convert.ToString(randomSeed),     //  00 Seed: used for randomization
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
            int[,] testWorld = WorldGen.GeographyGenerator.GenerateGeography(args);
            WorldGen.GeographyGenerator.PrintIDMap(args, testWorld);
            Console.WriteLine("");
            Console.WriteLine("");


            Console.WriteLine("Modified Array Printable");
            int[,] temperatureMap = WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.generateTemperatureMap(args, testWorld);

            WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.printTemperatureMap(args, temperatureMap, testWorld, true);


            #region Bitmaps

            var altitudeDictionary = new Dictionary<int, string>();
            {
                altitudeDictionary.Add(landCode_deepWater, Utility.Files.GetDirectory("\\res\\debug\\alitude\\deepwater.png"));
                altitudeDictionary.Add(landCode_offcoastWater, Utility.Files.GetDirectory("\\res\\debug\\alitude\\coastoffwater.png"));
                altitudeDictionary.Add(landCode_coastalWater, Utility.Files.GetDirectory("\\res\\debug\\alitude\\coastwater.png"));
                altitudeDictionary.Add(landCode_coastalLand, Utility.Files.GetDirectory("\\res\\debug\\alitude\\coastline.png"));
                altitudeDictionary.Add(landCode_Land, Utility.Files.GetDirectory("\\res\\debug\\alitude\\land.png"));
                altitudeDictionary.Add(landCode_hillLand, Utility.Files.GetDirectory("\\res\\debug\\alitude\\hillland.png"));
                altitudeDictionary.Add(landCode_mountain, Utility.Files.GetDirectory("\\res\\debug\\alitude\\mountain.png"));
            }

            int[,] temperatureArray = new int[temperatureMap.GetLength(0), temperatureMap.GetLength(1)];
            for (int i = 0; i < temperatureMap.GetLength(0); i++)
            {
                for (int j = 0; j < temperatureMap.GetLength(1); j++)
                {

                    //  Generate color scheme
                    if (temperatureMap[i, j] < 20)
                    {
                        temperatureArray[i, j] = 0;
                    }
                    else if (temperatureMap[i, j] >= 20 && temperatureMap[i, j] < 40)
                    {
                        temperatureArray[i, j] = 20;
                    }
                    else if (temperatureMap[i, j] >= 40 && temperatureMap[i, j] < 60)
                    {
                        temperatureArray[i, j] = 40;
                    }
                    else if (temperatureMap[i, j] >= 60 && temperatureMap[i, j] < 80)
                    {
                        temperatureArray[i, j] = 60;
                    }
                    else if (temperatureMap[i, j] >= 80 && temperatureMap[i, j] < 90)
                    {
                        temperatureArray[i, j] = 80;
                    }
                    else if (temperatureMap[i, j] >= 90)
                    {
                        temperatureArray[i, j] = 90;
                    }

                }
            }



            #endregion



        }

        static void matrixSizeTest()
        {
            Random rand = new Random();
            int seed = rand.Next(-99999999, 99999999);
            double freq1 = 3;

            int minimum = 16;

            //  64x256
            int[,] testArray1 = Utility.Noise.Perlin.GeneratePerlinInt(128, 512, freq1, 0, 31, seed);

            //  128x512
            int[,] testArray2 = Utility.Noise.Perlin.GeneratePerlinInt(256, 1024, freq1, 0, 31, seed);

            //  256x1024
            int[,] testArray3 = Utility.Noise.Perlin.GeneratePerlinInt(512, 2048, freq1, 0, 31, seed);




            Console.WriteLine("");
            for (int i = 0; i < testArray1.GetLength(0); i++)
            {
                for (int j = 0; j < testArray1.GetLength(1); j++)
                {
                    if (testArray1[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");


            Console.WriteLine("");
            for (int i = 0; i < testArray2.GetLength(0); i++)
            {
                for (int j = 0; j < testArray2.GetLength(1); j++)
                {
                    if (testArray2[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");

            //  256x384
            Console.WriteLine("");
            for (int i = 0; i < testArray3.GetLength(0); i++)
            {
                for (int j = 0; j < testArray3.GetLength(1); j++)
                {
                    if (testArray3[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");




            String MAPSIZE = "SMALL";

            switch (MAPSIZE)
            {
                case "VERY_SMALL":

                    break;
                case "SMALL":

                    break;
                case "MEDIUM":

                    break;
                case "LARGE":

                    break;
                case "VERY_LARGE":

                    break;
            }

        }
        static void matrixTest()
        {
            Random rand = new Random();
            int seed = rand.Next(-99999999, 99999999);
            double freq1 = 0.05;
            double freq2 = 2;
            double freq3 = 10;
            int minimum = 18;
            int percentage = 0;


            //  64x256
            int[,] testArray1 = Utility.Noise.Perlin.GeneratePerlinInt(128, 256, freq1, 0, 31, seed);
            List<List<Coords>> selectIslands1 = Utility.Matrices.Selection.SelectSections(testArray1, minimum, 99999, false, false);
            foreach (List<Coords> selectIsland in selectIslands1)
            {
                int randChance = rand.Next(0, 101);

                if (randChance <= percentage)
                {
                    foreach (Coords coord in selectIsland)
                    {
                        testArray1[coord.x, coord.y] = 0;
                    }
                }
            }






            //  64x256
            int[,] testArray2 = Utility.Noise.Perlin.GeneratePerlinInt(128, 256, freq2, 0, 31, seed);
            List<List<Coords>> selectIslands2 = Utility.Matrices.Selection.SelectSections(testArray2, minimum, 99999, false, false);
            foreach (List<Coords> selectIsland in selectIslands2)
            {
                int randChance = rand.Next(0, 101);

                if (randChance <= percentage)
                {
                    foreach (Coords coord in selectIsland)
                    {
                        testArray2[coord.x, coord.y] = 0;
                    }
                }
            }

            //  64x256
            int[,] testArray3 = Utility.Noise.Perlin.GeneratePerlinInt(128, 256, freq3, 0, 31, seed);
            List<List<Coords>> selectIslands3 = Utility.Matrices.Selection.SelectSections(testArray3, minimum, 99999, false, false);
            foreach (List<Coords> selectIsland in selectIslands3)
            {
                int randChance = rand.Next(0, 101);

                if (randChance <= percentage)
                {
                    foreach (Coords coord in selectIsland)
                    {
                        testArray3[coord.x, coord.y] = 0;
                    }
                }
            }



            Console.WriteLine("");
            for (int i = 0; i < testArray1.GetLength(0); i++)
            {
                for (int j = 0; j < testArray1.GetLength(1); j++)
                {
                    if (testArray1[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");



            Console.WriteLine("");
            for (int i = 0; i < testArray2.GetLength(0); i++)
            {
                for (int j = 0; j < testArray2.GetLength(1); j++)
                {
                    if (testArray2[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");


            //  256x384
            Console.WriteLine("");
            for (int i = 0; i < testArray3.GetLength(0); i++)
            {
                for (int j = 0; j < testArray3.GetLength(1); j++)
                {
                    if (testArray3[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");





        }
        static void testVoronoi()
        {

            #region Parameters

            Random rand = new Random();

            int randseedint = rand.Next(0, 9999999);
            String randseed = Convert.ToString(randseedint);


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




            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[0]);
            Random random = new Random(INITSEED);

            //  Loads the requested dimensions
            string MAPSIZE = Convert.ToString(args[1]);
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
            string MAPTYPE = args[5];
            #endregion



            #region Class Parameters
            //Land Codes Below Sealine
            int landCode_coastalLand = 40;
            #endregion



            //  Print the status
            #region Verbose
            Console.WriteLine("This is the console test for the Voronoi Partitioner. ");
            //  Generate a world according to the init args
            Console.WriteLine("Parameters:");
            Console.WriteLine("SEED:                " + args[0]);
            Console.WriteLine("MAPSIZE:             " + args[1]);
            Console.WriteLine("MAPTYPE:             " + args[5]);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-");



            #endregion

            //  Generate map
            #region Generate Map
            int[,] testWorld = WorldGen.GeographyGenerator.GeographyGenComponents.GenerateLandmassNoise(args, 4);
            //WorldGen.GeographyGenerator.GeographyGenComponents.PrintIDMap(args, testWorld);
            Console.WriteLine("");
            #endregion

            #region Create a list of sources and verify 
            //  Generate source seeds
            List<List<Coords>> islandsCoords = Utility.Matrices.Selection.SelectSections(testWorld, landCode_coastalLand, 999, false, false);
            List<Coords> sources = new List<Coords>();
            foreach (List<Coords> coordslists in islandsCoords)
            {
                List<Coords> addable = Utility.Lists.GetRandomSelection<Coords>(coordslists, 3, false, randseedint);
                sources.AddRange(addable);
            }
            #endregion



            #region Perform the Voronoi Expansion and Create voronoiArray
            //  Perform the Voronoi
            List<List<Coords>> voronoiLists = Utility.Matrices.Complex.Voronoi.ExpandTerritories(testWorld, sources, landCode_coastalLand, 999, randseedint);
            //  Print the Voronoi
            //Utility.Matrices.Complex.Voronoi.VoronoiTestPrint(testWorld, voronoiLists, sources);

            int[,] voronoiArray = Utility.Matrices.Misc.ConvertCoordstoArray(voronoiLists, mapRows, mapCols);
            //Utility.Print.RandomPrintColoredGrid(voronoiArray, 9999, true);
            #endregion




            //  For each voronoiSection, perform this
            int maxVint = 0;
            for (int i = 0; i < voronoiArray.GetLength(0); i++)
            {
                for (int j = 0; j < voronoiArray.GetLength(1); j++)
                {
                    if (voronoiArray[i, j] > maxVint)
                    {
                        maxVint = voronoiArray[i, j];
                    }


                }
            }





            //  Select a random Voronoi Section

            int randomCont = rand.Next(1, voronoiLists.Count());
            Console.WriteLine();
            Console.WriteLine("Continent selected:" + randomCont);
            Console.WriteLine();

            List<List<Coords>> testBorders = Utility.Matrices.Selection.SelectionSectionEdges(voronoiArray, randomCont, randomCont, mapsizeModifier / 128);
            List<Coords> badCoords = new List<Coords>();
            List<Coords> validCoordsList = new List<Coords>();

            //  Verify that the coords are valid, then copy to a new list
            foreach (List<Coords> coordsList in testBorders)
            {
                foreach (Coords coords in coordsList)
                {
                    List<Coords> circoords = Utility.Matrices.Selection.SelectCircleRegion(testWorld, coords, mapsizeModifier / 128);
                    foreach (Coords circoord in circoords)
                    {
                        if (testWorld[circoord.x, circoord.y] < landCode_coastalLand)
                        {
                            badCoords.Add(circoord);
                        }
                    }
                }

            }
            foreach (List<Coords> coordsList in testBorders)
            {
                foreach (Coords coords in coordsList)
                {
                    if (!badCoords.Contains(coords))
                    {
                        validCoordsList.Add(coords);
                    }
                }

            }

            List<List<Coords>> validCoords = new List<List<Coords>>();
            validCoords.Add(validCoordsList);

            int[,] borderArray = Utility.Matrices.Misc.ConvertCoordstoArray(validCoords, mapRows, mapCols);





            for (int i = 0; i < borderArray.GetLength(0); i++)
            {
                for (int j = 0; j < borderArray.GetLength(1); j++)
                {

                    if (borderArray[i, j] == 1)
                    {
                        Console.Write("^");
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                }
                Console.WriteLine();
            }
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


        static void noiseTests()
        {
            Random random = new Random();
            int randomSeed = random.Next(0, 9999999);
            double scale = 3;
            int size = 32;
            int rangestr = 0;
            int rangeend = 31;

            int floor = 15;

            int[,] testPerlin = Utility.Noise.Perlin.GeneratePerlinInt(size, size, scale, rangestr, rangeend, randomSeed);
            
            //double[,] testGraddUB = Utility.Noise.Gradients.GenerateGradientDistortion(size, size, randomSeed, scale);
            //int[,] testGrad = Utility.Mathematics.NormalizeToInteger(testGraddUB, rangestr, rangeend);

            Utility.Print.WriteLine("testPerlin", "61EAFF");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (testPerlin[i,j] > floor)
                    {
                        Console.Write("-");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

            Utility.Print.WriteLine("testGrad", "FF29C7");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    
                }
                Console.WriteLine();
            }
        }

    }
}
