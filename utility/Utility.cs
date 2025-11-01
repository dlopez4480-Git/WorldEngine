using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;


//  To use this Utility tool, call the namespace from your function
namespace Utility
{
    
    //  Utility Guide
    /*
     * The Utility Guide is a program designed to automate many of the tasks I found tedious.
     * I wanted a single script that self-contained everything I needed so that I would very seldom need to import/export anything
     * Below is a list of the sections and capabilities of this script.
     * 
     *      STRUCTS
     * Below is a list of commonly used structures
     * 
     *      Coords: Represents a pair of coordinates on a 2D Matrix
     * 
     *      HANDLER METHODS
     * Below is a list of commonly used objects which can be statically called.
     * 
     *  
     *      >Files: File mangager, responsible for uploading, downloading and manipulating files
     *          >JSONAccess: File Manager for JSON files
     *          
     *      >Lists: Manipulates the contents of arrays and arraylists.
     *      
     *      >Matrices: manages and manipulates 2D arrays
     *          >Complex: Performs pathfinding, searches and expansions in a 2D array
     *          >Misc: Misc usage cases for matrices
     *      
     *      >Noise Generates pseudorandom 2D matrix noise of various forms
     *      
     *      >Images : Images is capable of image manipulation and modification
     *      
     *      X Relies on System.Drawing to function. TODO: Create an implementation without this
     * 
     *      >Print: Prints out console content in more complex ways (color printing, array representation, etc. 
     *      
     *      X Relies on ANSI Colors and windows support. TODO: Find a way to make this more reliable.
     *      
     * **/

    //  Coords is useful for denoting regions within a 2D (or 3D) array
    public struct Coords
    {
        ///  <summary>
        ///  Coords is meant to be used within the context of a multidiensional array.
        ///  It is meant to represent the location of a cell relative to the index [0,0,0] of a 2D or 3D array.
        ///  It is NOT meant to indicate the content of the cell
        ///  </summary>
        

        public int x { get; set; }      //  Equivalent to ROW
        public int y { get; set; }      //  Equivalent to COLUMN
        public int z { get; set; }      //  Equivalent to AISLE

        

        public Coords(int x, int y, int z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override string ToString() => $"({x}, {y})";
    }



    public struct DiceRoll
    {
        //2d6+5
        public int diceNum { get; set; }
        public int diceSize { get; set; }
        public int diceModifier { get; set; }


    }






    public class Files
    {
        public class JSONAccess()
        {
            public static T ReadJsonFile<T>(string filePath)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"JSON file not found: {filePath}");
                }

                try
                {
                    // Read the JSON file content
                    string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);

                    // Deserialize the JSON content to the specified type
                    T result = JsonSerializer.Deserialize<T>(jsonContent);

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"Failed to parse JSON file: {ex.Message}", ex);
                }
                catch (IOException ex)
                {
                    throw new IOException($"Error reading file: {ex.Message}", ex);
                }
            }
            public static void WriteJsonFile(object obj, string filePathShort)
            {
                //  Convert filepath to a usable directory
                string filePath = Utility.Files.GetValidPath(filePathShort);

                if (obj == null) throw new ArgumentNullException(nameof(obj));
                if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize with indentation for readability
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // Serialize and write to file
                string json = JsonSerializer.Serialize(obj, options);
                File.WriteAllText(filePath, json);
            }


        }


        #region Basic Handlers
        public static readonly string sourceDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

        //  This function converts a shortened string filepath to one that works as a valid filepath
        public static string GetValidPath(string path)
        {
            return sourceDirectory + path;
        }

        //  This function converts a valid directory into a short directory String
        public static string GetConcisePath(string path)
        {
            // Check if filepath is already dynamic; if true, return path, if false, proceed
            try
            {
                if (!path.Substring(0, sourceDirectory.Length).Equals(sourceDirectory))
                {
                    Console.WriteLine("ERROR: Path does NOT begin with @C:..., returning path");
                    return path;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}: returning valid filepath ");
            }

            string result = path.Remove(0, sourceDirectory.Length);
            return result;
        }



        //  Creates a directory at the specified filepath
        public static void CreateDirectory(string path)
        {
            string filepath = path;
            try
            {
                //  Test if Directory Exists
                if (Directory.Exists(filepath))
                {
                    Console.WriteLine("Directory " + path + " Already Exists");
                    return;
                }
                //  Create Directory
                DirectoryInfo di = Directory.CreateDirectory(filepath);
                Console.WriteLine("The directory " + path + " was created successfully at {0}.", Directory.GetCreationTime(filepath));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }



        //  Read all lines of a .txt file, returns an array of Strings
        public static string[] ReadAllLines(string path)
        {
            string filepath = GetValidPath(path);
            string[] strings = File.ReadAllLines(filepath);
            return strings;
        }


        public static void WriteAllLines(string path, string[] contents)
        {
            string filepath = GetValidPath(path);
            File.WriteAllLines(filepath, contents);
        }



        #endregion
    }


    public class Lists
    {
        

        //  Remove lists with an object count ABOVE minSize
        public static List<List<T>> RemoveListAboveSize<T>(List<List<T>> targetList, int maximumSize)
        {
            List<List<T>> listOfLists = targetList;
            if (listOfLists == null)
            {
                throw new ArgumentNullException(nameof(listOfLists));
            }
            listOfLists.RemoveAll(subList => subList.Count < maximumSize);
            return listOfLists;
        }


        //  Remove lists with an object count BELOW maxSize
        public static List<List<T>> RemoveListBelowSize<T>(List<List<T>> targetList, int minimumSize)
        {
            List<List<T>> listOfLists = targetList;
            if (listOfLists == null) 
            { 
                throw new ArgumentNullException(nameof(listOfLists)); 
            }
            listOfLists.RemoveAll(subList => subList.Count > minimumSize);
            return listOfLists;
        }

        //  Remove lists with an object count BELOW maxSize
        public static List<List<T>> RemoveListOutsizeRange<T>(List<List<T>> targetList, int minimumSize, int maximumSize)
        {
            List<List<T>> listOfLists = targetList;
            if (listOfLists == null)
            {
                throw new ArgumentNullException(nameof(listOfLists));
            }

            listOfLists.RemoveAll(subList => subList.Count > minimumSize);
            listOfLists.RemoveAll(subList => subList.Count < maximumSize);
            
            return listOfLists;
        }


        //  Get a random selection of count objects from a List. 
        public static List<T> GetRandomSelection<T>(List<T> source, int count, bool duplicatable, int seed)
        {
            if (source == null)
            { 
                throw new ArgumentNullException(nameof(source)); 
            }
            if (count < 0)
            { 
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative."); 
            }
            if (!duplicatable && count > source.Count)
            { 
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot exceed source size when duplicates are not allowed."); 
            }

            Random rng = new Random(seed);
            List<T> result = new List<T>(count);

            if (duplicatable)
            {
                // Sampling with replacement
                for (int i = 0; i < count; i++)
                {
                    int index = rng.Next(source.Count);
                    result.Add(source[index]);
                }
            }
            else
            {
                // Sampling without replacement (Fisher–Yates shuffle)
                List<T> copy = new List<T>(source);
                for (int i = 0; i < count; i++)
                {
                    int j = rng.Next(i, copy.Count);
                    (copy[i], copy[j]) = (copy[j], copy[i]);
                    result.Add(copy[i]);
                }
            }

            return result;
        }


        //  Shuffle a given list into a new list
        public static IList<T> Shuffle<T>(IList<T> input, int seed)
        {
            var copy = new List<T>(input); // copy into a List<T>
            Random rand = new Random(seed);
            int n = copy.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T temp = copy[n];
                copy[n] = copy[k];
                copy[k] = temp;
            }
            return copy;
        }



        //  Sorts a list of lists by the size of their contents
        public static List<List<T>> SortBySubListSize<T>(List<List<T>> listOfLists, bool ascendingOrder = true)
        {
            if (listOfLists == null)
            { 
                throw new ArgumentNullException(nameof(listOfLists)); 
            }

            // Use LINQ to sort based on sublist count
            var sorted = ascendingOrder ? listOfLists.OrderBy(subList => subList.Count).ToList() : listOfLists.OrderByDescending(subList => subList.Count).ToList();

            return sorted;
        }



        //  Collapses a list of lists into a new list
        public static List<T> CollapseLists<T>(List<List<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Pre-allocate to reduce reallocations if you want:
            int totalCount = 0;
            foreach (var inner in source)
            {
                if (inner != null) totalCount += inner.Count;
            }

            var result = new List<T>(totalCount);
            foreach (var inner in source)
            {
                if (inner == null) continue; // skip null inner lists
                result.AddRange(inner);
            }

            return result;
        }


        public class Sets
        {
            //  Get all intersecting values between the lists
            public static List<T> GetIntersection<T>(List<T> listA, List<T> listB)
            {
                if (listA == null)
                    throw new ArgumentNullException(nameof(listA));
                if (listB == null)
                    throw new ArgumentNullException(nameof(listB));

                // Use LINQ's Intersect method (uses default equality comparer)
                return listA.Intersect(listB).ToList();
            }
        }

        
    }


    public class Matrices
    {
        //  Select regions within a list
        public class Selection
        {

            #region Select and return all cells within ranges

            public static List<List<Coords>> SelectCellsWithinRange(int[,] grid, int rangeMin, int rangeMax, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);
                bool[,] visited = new bool[rows, cols];
                var islands = new List<List<Coords>>();

                // Direction vectors: Up, Down, Left, Right
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };

                bool InRange(int value) => value >= rangeMin && value <= rangeMax;

                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        if (visited[x, y]) continue;
                        if (!InRange(grid[x, y])) continue;

                        // Start a new island
                        var island = new List<Coords>();
                        var queue = new Queue<Coords>();
                        queue.Enqueue(new Coords(x, y));
                        visited[x, y] = true;

                        while (queue.Count > 0)
                        {
                            var current = queue.Dequeue();
                            island.Add(current);

                            for (int dir = 0; dir < 4; dir++)
                            {
                                int nx = current.x + dx[dir];
                                int ny = current.y + dy[dir];

                                // Handle wrapping
                                if (nx < 0)
                                    nx = verticalWrapping ? rows - 1 : -1;
                                else if (nx >= rows)
                                    nx = verticalWrapping ? 0 : -1;

                                if (ny < 0)
                                    ny = horizontalWrapping ? cols - 1 : -1;
                                else if (ny >= cols)
                                    ny = horizontalWrapping ? 0 : -1;

                                // Skip invalid moves
                                if (nx == -1 || ny == -1) continue;

                                // Check range and visited state
                                if (!visited[nx, ny] && InRange(grid[nx, ny]))
                                {
                                    visited[nx, ny] = true;
                                    queue.Enqueue(new Coords(nx, ny));
                                }
                            }
                        }

                        islands.Add(island);
                    }
                }

                return islands;
            }
            public static List<Coords> SelectCellsWithinRangeCollapsed(int[,] grid, int rangeMin, int rangeMax, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                List<List<Coords>> uncollpasedList = SelectCellsWithinRange(grid, rangeMin, rangeMax, horizontalWrapping, verticalWrapping);
                List<Coords> collapsedList = Utility.Lists.CollapseLists(uncollpasedList);
                return collapsedList;
            }



            //  Find the neighboring edges of orthogonally connected sections within a 2D Matrix within a given range
            public static List<List<Coords>> SelectCellsWithinRangeEdge(int[,] grid, int rangeMin, int rangeMax, int rangeEdge, bool innerSelect = true, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);
                bool[,] visited = new bool[rows, cols];
                var result = new List<List<Coords>>();

                int[] dxOrth = { -1, 1, 0, 0 };
                int[] dyOrth = { 0, 0, -1, 1 };
                int[] dxAll = { -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] dyAll = { -1, 0, 1, -1, 1, -1, 0, 1 };

                bool InRange(int val) => val >= rangeMin && val <= rangeMax;

                // Helper: Wrap coordinate if needed, or mark invalid
                (int x, int y)? Wrap(int x, int y)
                {
                    if (x < 0)
                        x = verticalWrapping ? rows - 1 : -1;
                    else if (x >= rows)
                        x = verticalWrapping ? 0 : -1;

                    if (y < 0)
                        y = horizontalWrapping ? cols - 1 : -1;
                    else if (y >= cols)
                        y = horizontalWrapping ? 0 : -1;

                    if (x == -1 || y == -1) return null;
                    return (x, y);
                }

                // Step 1: Identify all island cells
                bool[,] isIsland = new bool[rows, cols];
                for (int x = 0; x < rows; x++)
                    for (int y = 0; y < cols; y++)
                        if (InRange(grid[x, y]))
                            isIsland[x, y] = true;

                // Step 2: Identify border cells
                bool[,] isBorder = new bool[rows, cols];
                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        if (!isIsland[x, y]) continue;

                        // Check all 8 neighbors
                        foreach (var (dx, dy) in Neighbors(dxAll, dyAll))
                        {
                            var n = Wrap(x + dx, y + dy);
                            if (n == null) continue;
                            int nx = n.Value.x;
                            int ny = n.Value.y;

                            if (!InRange(grid[nx, ny]))
                            {
                                isBorder[x, y] = true;
                                break;
                            }
                        }
                    }
                }

                // Step 3: Determine selection region (inner or outer)
                bool[,] selectMask = new bool[rows, cols];

                if (innerSelect)
                {
                    // Expand border cells inward (still within islands)
                    ExpandBorders(isBorder, isIsland, selectMask, rangeEdge, true, Wrap);
                }
                else
                {
                    // Expand outward into non-island cells
                    ExpandBorders(isBorder, isIsland, selectMask, rangeEdge, false, Wrap);
                }

                // Step 4: Group connected cells (orthogonal BFS)
                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        if (!selectMask[x, y] || visited[x, y])
                            continue;

                        var region = new List<Coords>();
                        var queue = new Queue<Coords>();
                        queue.Enqueue(new Coords(x, y));
                        visited[x, y] = true;

                        while (queue.Count > 0)
                        {
                            var cur = queue.Dequeue();
                            region.Add(cur);

                            for (int d = 0; d < 4; d++)
                            {
                                var n = Wrap(cur.x + dxOrth[d], cur.y + dyOrth[d]);
                                if (n == null) continue;
                                int nx = n.Value.x;
                                int ny = n.Value.y;

                                if (selectMask[nx, ny] && !visited[nx, ny])
                                {
                                    visited[nx, ny] = true;
                                    queue.Enqueue(new Coords(nx, ny));
                                }
                            }
                        }

                        result.Add(region);
                    }
                }

                return result;
            }
            public static List<Coords> SelectCellsWithinRangeEdgeCollapsed(int[,] grid, int rangeMin, int rangeMax, int rangeEdge, bool innerSelect = true, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                List<List<Coords>> uncollpasedList = SelectCellsWithinRangeEdge(grid, rangeMin, rangeMax, rangeEdge, innerSelect, horizontalWrapping, verticalWrapping);
                List<Coords> collapsedList = Utility.Lists.CollapseLists(uncollpasedList);
                return collapsedList;
            }

            // Enumerate tuple pairs
            private static IEnumerable<(int, int)> Neighbors(int[] dx, int[] dy)
            {
                for (int i = 0; i < dx.Length; i++)
                    yield return (dx[i], dy[i]);
            }

            // Expands from border cells using Manhattan distance
            private static void ExpandBorders(bool[,] isBorder, bool[,] isIsland, bool[,] selectMask, int rangeEdge, bool inward, Func<int, int, (int x, int y)?> wrapFunc)
            {
                int rows = isBorder.GetLength(0);
                int cols = isBorder.GetLength(1);
                int[] dxAll = { -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] dyAll = { -1, 0, 1, -1, 1, -1, 0, 1 };

                var dist = new int[rows, cols];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        dist[i, j] = int.MaxValue;

                var queue = new Queue<(int x, int y)>();

                // Initialize border cells
                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        if (isBorder[x, y])
                        {
                            queue.Enqueue((x, y));
                            dist[x, y] = 0;
                        }
                    }
                }

                while (queue.Count > 0)
                {
                    var (cx, cy) = queue.Dequeue();

                    if (dist[cx, cy] <= rangeEdge)
                    {
                        if (inward && isIsland[cx, cy])
                            selectMask[cx, cy] = true;
                        else if (!inward && !isIsland[cx, cy])
                            selectMask[cx, cy] = true;
                    }

                    if (dist[cx, cy] == rangeEdge)
                        continue; // stop expanding

                    foreach (var (dx, dy) in Neighbors(dxAll, dyAll))
                    {
                        var n = wrapFunc(cx + dx, cy + dy);
                        if (n == null) continue;
                        int nx = n.Value.x;
                        int ny = n.Value.y;

                        int newDist = dist[cx, cy] + 1;
                        if (newDist < dist[nx, ny])
                        {
                            dist[nx, ny] = newDist;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }






            #endregion

            #region Select Sections within specified regions
            //  Get a list of Coords within a circular region around a center, in radius radius 
            public static List<Coords> SelectCircleRegion<T>(T[,] array, Coords center, int radius)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                List<Coords> coordsList = new List<Coords>();

                for (int r = Math.Max(0, center.x - radius); r <= Math.Min(rows - 1, center.x + radius); r++)
                {
                    for (int c = Math.Max(0, center.y - radius); c <= Math.Min(cols - 1, center.y + radius); c++)
                    {
                        int deltaX = r - center.x;
                        int deltaY = c - center.y;
                        if (deltaX * deltaX + deltaY * deltaY <= radius * radius)
                        {
                            coordsList.Add(new Coords(r, c));
                        }
                    }
                }

                return coordsList;
            }


            //  Get a circular region within an ovoid range
            public static List<Coords> SelectOvalRegion<T>(T[,] array, Coords center, int height, int width)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);


                List<Coords> ovalCoords = new List<Coords>();

                // Radii (floating point for precision)
                double rx = height / 2.0;
                double ry = width / 2.0;

                // Bounding box to search within
                int minX = Math.Max(0, (int)Math.Floor(center.x - rx));
                int maxX = Math.Min(rows - 1, (int)Math.Ceiling(center.x + rx));
                int minY = Math.Max(0, (int)Math.Floor(center.y - ry));
                int maxY = Math.Min(cols - 1, (int)Math.Ceiling(center.y + ry));

                for (int i = minX; i <= maxX; i++)
                {
                    for (int j = minY; j <= maxY; j++)
                    {
                        double dx = (i - center.x) / rx;
                        double dy = (j - center.y) / ry;

                        if (dx * dx + dy * dy <= 1.0)
                        {
                            ovalCoords.Add(new Coords(i, j));
                        }
                    }
                }

                return ovalCoords;
            }


            //  Given a list of Coords, get the set difference
            public static List<Coords> SelectSetDifference<T>(T[,] array, List<Coords> coordsList)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);

                // Use a HashSet for fast lookup of "inside" coords
                HashSet<(int, int)> insideSet = new HashSet<(int, int)>();
                foreach (var c in coordsList)
                {
                    insideSet.Add((c.x, c.y));
                }

                List<Coords> outsideCoords = new List<Coords>();

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (!insideSet.Contains((i, j)))
                        {
                            outsideCoords.Add(new Coords(i, j));
                        }
                    }
                }

                return outsideCoords;
            }

            #endregion

            

        }


        //  This section performs transformation of matrices and matrix content
        public class Geometry()
        {


            #region Transformation Helpers
            //  This function gets the Coords closest to index [0,0], priorizing rows over columns.
            public static Coords GetStartingPoint(IEnumerable<Coords> cells)
            {
                if (cells == null) { throw new ArgumentNullException(nameof(cells)); }
                if (!cells.Any()) { throw new ArgumentException("Collection is empty.", nameof(cells)); }

                return cells.OrderBy(c => c.x).ThenBy(c => c.y).First();
            }
            //  This function gets the Coords closest to the "center of mass" of a group of Coords
            public static Coords GetCenterOfMass(IEnumerable<Coords> cells)
            {
                if (cells == null) throw new ArgumentNullException(nameof(cells));

                long sumX = 0;
                long sumY = 0;
                int count = 0;

                foreach (var c in cells)
                {
                    sumX += c.x;
                    sumY += c.y;
                    count++;
                }

                if (count == 0)
                { throw new ArgumentException("Collection is empty.", nameof(cells)); }

                // Round to nearest integer
                int centerX = (int)Math.Round((double)sumX / count);
                int centerY = (int)Math.Round((double)sumY / count);

                return new Coords(centerX, centerY);
            }
            #endregion
            




            public static List<Coords> TranslateSection(IEnumerable<Coords> cells, int rows, int cols, Coords startingPoint, bool centerMassed = true, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                if (cells == null)
                {
                    throw new ArgumentNullException(nameof(cells));
                }

                // --- Helper to wrap negatives properly ---
                int Mod(int a, int m)
                {
                    int r = a % m;
                    return r < 0 ? r + m : r;
                }

                // Determine anchor
                Coords anchor = centerMassed
                    ? GetCenterOfMass(cells)      // from our earlier non-LINQ function
                    : GetStartingPoint(cells);

                int deltaX = startingPoint.x - anchor.x;
                int deltaY = startingPoint.y - anchor.y;

                var result = new List<Coords>();

                foreach (var c in cells)
                {
                    int newX = c.x + deltaX;
                    int newY = c.y + deltaY;

                    // Handle vertical
                    if (verticalWrapping)
                    {
                        newX = Mod(newX, rows);
                    }
                    else if (newX < 0 || newX >= rows)
                    {
                        continue; // skip if out of bounds
                    }

                    // Handle horizontal
                    if (horizontalWrapping)
                    {
                        newY = Mod(newY, cols);
                    }
                    else if (newY < 0 || newY >= cols)
                    {
                        continue; // skip if out of bounds
                    }

                    result.Add(new Coords(newX, newY));
                }

                return result;
            }

            public static List<Coords> RotateSection(IEnumerable<Coords> cells, int rows, int cols, int rotations, bool centerMassed = true, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                if (cells == null) { throw new ArgumentNullException(nameof(cells)); }

                // Local helper for wrapping with positive remainder
                int Mod(int a, int m) { int r = a % m; return r < 0 ? r + m : r; }

                // Determine pivot
                Coords pivot = centerMassed ? GetCenterOfMass(cells) : GetStartingPoint(cells);

                // Normalize rotation count to 0..3
                int steps = ((rotations % 4) + 4) % 4; // ensures positive 0-3

                var result = new List<Coords>();

                foreach (var c in cells)
                {
                    // Relative position to pivot
                    int dx = c.x - pivot.x;
                    int dy = c.y - pivot.y;

                    int rx = dx;
                    int ry = dy;

                    // Apply rotation steps clockwise
                    for (int i = 0; i < steps; i++)
                    {
                        // 90° clockwise: (dx,dy) -> (-dy, dx)
                        int temp = rx;
                        rx = -ry;
                        ry = temp;
                    }

                    // Back to absolute coordinates
                    int newX = pivot.x + rx;
                    int newY = pivot.y + ry;

                    // Wrapping / bounds
                    if (verticalWrapping) newX = Mod(newX, rows);
                    else if (newX < 0 || newX >= rows) continue;

                    if (horizontalWrapping) newY = Mod(newY, cols);
                    else if (newY < 0 || newY >= cols) continue;

                    result.Add(new Coords(newX, newY));
                }

                return result;
            }

            public static List<Coords> ReflectSection(IEnumerable<Coords> cells, int rows, int cols, bool horizontalFlip, bool verticalFlip, bool centerMassed = true, bool horizontalWrapping = false, bool verticalWrapping = false)
            {
                if (cells == null) { throw new ArgumentNullException(nameof(cells)); }

                int Mod(int a, int m) { int r = a % m; return r < 0 ? r + m : r; }

                // Determine pivot
                Coords pivot = centerMassed ? GetCenterOfMass(cells) : GetStartingPoint(cells);

                var result = new List<Coords>();

                foreach (var c in cells)
                {
                    int dx = c.x - pivot.x;
                    int dy = c.y - pivot.y;

                    if (verticalFlip) dx = -dx; // flip across horizontal axis
                    if (horizontalFlip) dy = -dy; // flip across vertical axis

                    int newX = pivot.x + dx;
                    int newY = pivot.y + dy;

                    if (verticalWrapping) newX = Mod(newX, rows);
                    else if (newX < 0 || newX >= rows) continue;

                    if (horizontalWrapping) newY = Mod(newY, cols);
                    else if (newY < 0 || newY >= cols) continue;

                    result.Add(new Coords(newX, newY));
                }

                return result;
            }
            
        }



        //  This section performs more complicated matrix manipulation, like pathfinding, expansion, etc.
        public class Complex()
        {
            //  Performs expansion algortithms
            public class Voronoi()
            {
                public static List<List<Coords>> CreateVoronoi(int[,] sourcegrid, List<Coords> seeds, int minimum, int maximum, int seed, bool horizontalWrapping = false, bool verticalWrapping = false)
                {
                    int rows = sourcegrid.GetLength(0);
                    int cols = sourcegrid.GetLength(1);

                    int[,] grid = sourcegrid;
                    Utility.Matrices.Misc.Rotate2DMatrix(grid, 1);
                    Utility.Matrices.Misc.Flip2DMatrix(grid, true, true);


                    // Ownership grid: -1 = unclaimed, otherwise index of team
                    int[,] owner = new int[rows, cols];
                    for (int r = 0; r < rows; r++)
                        for (int c = 0; c < cols; c++)
                            owner[r, c] = -1;

                    // Result teams
                    List<List<Coords>> teams = new List<List<Coords>>();
                    Queue<(Coords pos, int team)> frontier = new Queue<(Coords, int)>();

                    // Random for deterministic tie-breaking
                    Random rng = new Random(seed);

                    // Initialize seeds
                    for (int i = 0; i < seeds.Count; i++)
                    {
                        Coords s = seeds[i];
                        if (s.x < 0 || s.x >= rows || s.y < 0 || s.y >= cols)
                        {
                            teams.Add(new List<Coords>()); // invalid
                            continue;
                        }

                        int value = grid[s.x, s.y];
                        if (value < minimum || value > maximum)
                        {
                            teams.Add(new List<Coords>()); // invalid
                            continue;
                        }

                        // Valid seed
                        teams.Add(new List<Coords>() { s });
                        owner[s.x, s.y] = i;
                        frontier.Enqueue((s, i));
                    }

                    // Directions (orthogonal only)
                    int[,] dirs = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                    // BFS level-synchronous
                    while (frontier.Count > 0)
                    {
                        // Collect all expansion candidates for this wave
                        Dictionary<(int, int), List<int>> candidates = new Dictionary<(int, int), List<int>>();
                        int levelCount = frontier.Count;

                        for (int k = 0; k < levelCount; k++)
                        {
                            var (pos, team) = frontier.Dequeue();

                            for (int d = 0; d < 4; d++)
                            {
                                int nx = pos.x + dirs[d, 0];
                                int ny = pos.y + dirs[d, 1];

                                // Wrapping
                                if (horizontalWrapping)
                                {
                                    if (ny < 0) ny = cols - 1;
                                    else if (ny >= cols) ny = 0;
                                }
                                if (verticalWrapping)
                                {
                                    if (nx < 0) nx = rows - 1;
                                    else if (nx >= rows) nx = 0;
                                }

                                // Skip out of bounds if no wrapping
                                if (nx < 0 || nx >= rows || ny < 0 || ny >= cols)
                                    continue;

                                // Already claimed?
                                if (owner[nx, ny] != -1)
                                    continue;

                                // Valid value?
                                int value = grid[nx, ny];
                                if (value < minimum || value > maximum)
                                    continue;

                                var key = (nx, ny);
                                if (!candidates.ContainsKey(key))
                                    candidates[key] = new List<int>();
                                candidates[key].Add(team);
                            }
                        }

                        // Resolve conflicts
                        foreach (var kvp in candidates)
                        {
                            (int cx, int cy) = kvp.Key;
                            List<int> claimers = kvp.Value;

                            int winner;
                            if (claimers.Count == 1)
                            {
                                winner = claimers[0];
                            }
                            else
                            {
                                // Tie-breaker with seeded RNG
                                winner = claimers[rng.Next(claimers.Count)];
                            }

                            owner[cx, cy] = winner;
                            Coords newCoord = new Coords(cx, cy);
                            teams[winner].Add(newCoord);
                            frontier.Enqueue((newCoord, winner));
                        }
                    }


                    return teams;
                }


            }

            public class Pathfinding
            {
                public static List<Coords> FindPath(Coords start, Coords end, int[,] grid)
                {
                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);

                    bool[,] visited = new bool[rows, cols];
                    Dictionary<Coords, Coords?> parent = new Dictionary<Coords, Coords?>();
                    Queue<Coords> queue = new Queue<Coords>();

                    visited[start.x, start.y] = true;
                    queue.Enqueue(start);
                    parent[start] = null; // start has no parent

                    while (queue.Count > 0)
                    {
                        Coords current = queue.Dequeue();
                        if (current.x == end.x && current.y == end.y)
                        {
                            // Reconstruct path
                            return ReconstructPath(parent, end);
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = current.x + dx[i];
                            int ny = current.y + dy[i];

                            if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
                            {
                                if (!visited[nx, ny] && grid[nx, ny] <= grid[current.x, current.y])
                                {
                                    visited[nx, ny] = true;
                                    Coords next = new Coords(nx, ny);
                                    parent[next] = current;
                                    queue.Enqueue(next);
                                }
                            }
                        }
                    }

                    // No valid path
                    return new List<Coords>();
                }
                private static readonly int[] dx = { -1, 1, 0, 0 };
                private static readonly int[] dy = { 0, 0, -1, 1 };
                private static List<Coords> ReconstructPath(Dictionary<Coords, Coords?> parent, Coords end)
                {
                    List<Coords> path = new List<Coords>();
                    Coords? current = end;
                    while (current != null)
                    {
                        path.Add(current.Value);
                        current = parent[current.Value];
                    }
                    path.Reverse();
                    return path;
                }

                public static List<Coords> DownhillRandomWalk(int[,] grid, Coords start, int length, int targetVal, int seed)
                {
                    int width = grid.GetLength(0);
                    int height = grid.GetLength(1);
                    var path = new List<Coords>();
                    var rng = new Random(seed);

                    // Offsets for orthogonal movement
                    (int dx, int dy)[] orthogonal = { (1, 0), (-1, 0), (0, 1), (0, -1) };

                    // Track forbidden cells (visited + their neighbors)
                    bool[,] forbidden = new bool[width, height];

                    // Helper: mark cell and all 8 neighbors forbidden
                    void MarkForbidden(int cx, int cy)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = cx + dx;
                                int ny = cy + dy;
                                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                                    forbidden[nx, ny] = true;
                            }
                    }

                    // Initialize
                    path.Add(start);
                    MarkForbidden(start.x, start.y);

                    Coords current = start;

                    // Immediate success check
                    if (grid[current.x, current.y] == targetVal)
                        return path;

                    for (int step = 1; step < length; step++)
                    {
                        int currentVal = grid[current.x, current.y];

                        // Collect valid neighbors
                        var candidates = new List<Coords>();
                        foreach (var (dx, dy) in orthogonal)
                        {
                            int nx = current.x + dx;
                            int ny = current.y + dy;

                            if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                                continue;

                            // Must be downhill (value >= current value)
                            if (grid[nx, ny] < currentVal) continue;

                            // Only move to cell not forbidden,
                            // but allow moving back to immediate previous cell? No, per spec only the cell we are *currently on* is exempt,
                            // so we can't revisit previous anyway.
                            if (forbidden[nx, ny]) continue;

                            candidates.Add(new Coords(nx, ny));
                        }

                        if (candidates.Count == 0)
                        {
                            // No valid moves
                            return new List<Coords>();
                        }

                        // Choose random neighbor
                        var next = candidates[rng.Next(candidates.Count)];
                        current = next;
                        path.Add(current);
                        MarkForbidden(current.x, current.y);

                        // Check target
                        if (grid[current.x, current.y] == targetVal)
                        { return path; }
                    }

                    // Failed to reach target within allowed length
                    return new List<Coords>();
                }

            }

            public class Packing
            {
                //  Get the translated list of Coords (using the top left cell as the start.
                //  Return an empty list of Coords if it goes out of bounds



                //  This algorithm takes a LoL of Coords, representing a section, and returns all possible starting locations
                public static List<Coords[]> FindValidPlacements(int rows, int cols, List<List<Coords>> shapes, int maximumPlacements = -1)
                {
                    var placements = new List<Coords[]>();

                    // --- Basic input guards ---
                    if (rows <= 0 || cols <= 0) return placements;
                    if (shapes == null || shapes.Count == 0) return placements;
                    if (maximumPlacements == 0) return placements; // explicit: user asked for 0 results

                    // Quick area check: if total cells > grid capacity, impossible
                    int totalCells = 0;
                    foreach (var shape in shapes)
                    {
                        if (shape == null || shape.Count == 0) return placements;
                        totalCells += shape.Count;
                    }
                    if (totalCells > rows * cols) return placements;

                    // Work array that will hold the anchor (placement) for each shape index
                    Coords[] currentPlacement = new Coords[shapes.Count];

                    // Occupied set to detect overlap quickly
                    var occupied = new HashSet<(int, int)>();

                    // --- Precompute shape sizes for pruning ---
                    int[] shapeCellCounts = shapes.Select(s => s.Count).ToArray();

                    //  Phase 2b: PRECOMPUTE ALL VALID TRANSLATIONS FOR EACH SHAPE
                    var validPlacementsPerShape = new List<List<(Coords anchor, List<Coords> cells)>>();

                    foreach (var shape in shapes)
                    {
                        var validList = new List<(Coords, List<Coords>)>();

                        for (int r = 0; r < rows; r++)
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                var translated = Utility.Matrices.Geometry.TranslateSection(shape, rows, cols, new Coords(r, c), centerMassed: true, horizontalWrapping: false, verticalWrapping: false);

                                // Skip placements that fell out of bounds
                                if (translated.Count != shape.Count) continue;

                                validList.Add((new Coords(r, c), translated));
                            }
                        }

                        validPlacementsPerShape.Add(validList);
                    }

                    //  Phase 2b (optional enhancement): order shapes by fewest valid placements first (heuristic)
                    // This helps prune branches faster by placing constrained shapes first.
                    // NOTE: We must reorder both 'shapes' and 'validPlacementsPerShape' consistently.
                    var ordered = validPlacementsPerShape
                        .Select((placements, index) => new { index, placements, count = placements.Count, size = shapes[index].Count })
                        .OrderBy(x => x.count)
                        .ThenByDescending(x => x.size) // secondary: place larger shapes first if equal
                        .ToList();

                    var orderedShapes = ordered.Select(x => shapes[x.index]).ToList();
                    var orderedValidPlacements = ordered.Select(x => x.placements).ToList();
                    var orderedCellCounts = ordered.Select(x => shapeCellCounts[x.index]).ToArray();

                    // Recursive function
                    void PlaceShape(int shapeIndex)
                    {
                        if (maximumPlacements > 0 && placements.Count >= maximumPlacements) return;

                        // Phase 2a SHORT-CIRCUIT CHECK (kept from before)
                        int occupiedCount = occupied.Count;
                        int remainingArea = (rows * cols) - occupiedCount;

                        int remainingCellsNeeded = 0;
                        for (int i = shapeIndex; i < orderedShapes.Count; i++)
                            remainingCellsNeeded += orderedCellCounts[i];

                        if (remainingArea < remainingCellsNeeded)
                            return; // impossible to fit remaining shapes

                        // Base case: all shapes placed
                        if (shapeIndex == orderedShapes.Count)
                        {
                            placements.Add((Coords[])currentPlacement.Clone());
                            return;
                        }

                        // Phase 2b: iterate only through precomputed valid placements
                        foreach (var (anchor, translated) in orderedValidPlacements[shapeIndex])
                        {
                            bool fits = true;
                            foreach (var cell in translated)
                            {
                                if (occupied.Contains((cell.x, cell.y)))
                                {
                                    fits = false;
                                    break;
                                }
                            }
                            if (!fits) continue;

                            foreach (var cell in translated)
                                occupied.Add((cell.x, cell.y));

                            currentPlacement[shapeIndex] = anchor;

                            PlaceShape(shapeIndex + 1);

                            foreach (var cell in translated)
                                occupied.Remove((cell.x, cell.y));

                            if (maximumPlacements > 0 && placements.Count >= maximumPlacements) return;
                        }
                    }

                    // Start recursion
                    PlaceShape(0);

                    return placements;
                }


                //  Given an already loaded array, find a list of valid placements for one shape
                public static List<Coords> FindValidPlacementsInArray(int[,] inputArray, List<Coords> shape, int rangeMin, int rangeMax, int distance, bool horizontalWrapping = false, bool verticalWrapping = false)
                {
                    int rows = inputArray.GetLength(0);
                    int cols = inputArray.GetLength(1);
                    var validStarts = new List<Coords>();
                    if (shape == null || shape.Count == 0)
                    {
                        return validStarts;
                    }

                    //  Create a list of cells not to be checked
                    List<Coords> cellsExcluded = new List<Coords>();
                    List<List<Coords>> cellsWithinRangeLoL = Utility.Matrices.Selection.SelectCellsWithinRange(inputArray, rangeMin, rangeMax, horizontalWrapping, verticalWrapping);
                    List<List<Coords>> cellsWithinRangeDistLoL = Utility.Matrices.Selection.SelectCellsWithinRangeEdge(inputArray, rangeMin, rangeMax, distance, false, horizontalWrapping, verticalWrapping);
                    cellsExcluded.AddRange(Utility.Lists.CollapseLists(cellsWithinRangeLoL));
                    cellsExcluded.AddRange(Utility.Lists.CollapseLists(cellsWithinRangeDistLoL));

                    //  Iterate through the list, and test each translateList to see if any overlap
                    for (int i = 0; i < inputArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < inputArray.GetLength(1); j++)
                        {
                            Coords newStart = new Coords(i, j);
                            //  Iterates through all tiles not directly within a forbidden tile
                            if (!cellsExcluded.Contains(newStart))
                            {
                                List<Coords> translateList = Utility.Matrices.Geometry.TranslateSection(shape, rows, cols, newStart, true, horizontalWrapping, verticalWrapping);
                                bool overlaps = false;

                                //  Check that translateList is not out of bounds
                                if (translateList.Count < shape.Count)
                                {
                                    overlaps = true;
                                    goto overlapGoto;
                                }


                                //  Tests all cells with a translated list for overlapping
                                foreach (Coords cellcoord in translateList)
                                {
                                    if (inputArray[cellcoord.x, cellcoord.y] >= rangeMin && inputArray[cellcoord.x, cellcoord.y] <= rangeMax)
                                    {
                                        overlaps = true;
                                        goto overlapGoto;
                                    }
                                }
                            overlapGoto:
                                if (!overlaps)
                                {
                                    validStarts.Add(newStart);
                                }
                            }
                        }
                    }

                    //  Return all valid starts
                    return validStarts;
                }







            }

            public class BorderRandomizer
            {
                public static List<List<Coords>> RandomizeBorders(int[,] grid, int rangeMin, int rangeMax, double amplitude, double frequency, double smoothness, int seed, bool horizontalWrapping = false, bool verticalWrapping = false)
                {
                    var toRemove = new List<Coords>();
                    var toAdd = new List<Coords>();

                    // Step 1: Get border cells
                    var borderRegions = Matrices.Selection.SelectCellsWithinRangeEdge(grid, rangeMin, rangeMax, 1, innerSelect: true, horizontalWrapping, verticalWrapping);

                    bool InRange(int val) => val >= rangeMin && val <= rangeMax;

                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);

                    (int, int)? Wrap(int x, int y)
                    {
                        if (x < 0)
                            x = verticalWrapping ? rows - 1 : -1;
                        else if (x >= rows)
                            x = verticalWrapping ? 0 : -1;

                        if (y < 0)
                            y = horizontalWrapping ? cols - 1 : -1;
                        else if (y >= cols)
                            y = horizontalWrapping ? 0 : -1;

                        if (x == -1 || y == -1) return null;
                        return (x, y);
                    }

                    int[] dxAll = { -1, -1, -1, 0, 0, 1, 1, 1 };
                    int[] dyAll = { -1, 0, 1, -1, 1, -1, 0, 1 };

                    // Step 2-5: Process each border region
                    foreach (var region in borderRegions)
                    {
                        // Step 2: Create a stable order (optional simple heuristic)
                        var ordered = OrderBorder(region);

                        for (int i = 0; i < ordered.Count; i++)
                        {
                            var c = ordered[i];
                            double noise = Noise.Perlin1D.GetValue(i, amplitude, frequency, smoothness, seed);
                            int offset = (int)Math.Round(Math.Abs(noise));
                            if (offset == 0) continue;

                            // Compute outward direction
                            var outward = EstimateOutwardNormal(c, grid, InRange, Wrap, dxAll, dyAll);

                            // Apply offset steps
                            for (int step = 1; step <= offset; step++)
                            {
                                int nx = c.x + (int)Math.Round(outward.x * step);
                                int ny = c.y + (int)Math.Round(outward.y * step);
                                var n = Wrap(nx, ny);
                                if (n == null) continue;
                                nx = n.Value.Item1;
                                ny = n.Value.Item2;

                                if (noise > 0)
                                {
                                    // Expansion
                                    if (!InRange(grid[nx, ny]))
                                        toAdd.Add(new Coords(nx, ny));
                                }
                                else
                                {
                                    // Contraction
                                    if (InRange(grid[nx, ny]))
                                        toRemove.Add(new Coords(nx, ny));
                                }
                            }
                        }
                    }

                    return new List<List<Coords>> { toRemove, toAdd };
                }

                private static List<Coords> OrderBorder(List<Coords> border)
                {
                    // Simple heuristic: sort roughly by x+y (not perfect but stable)
                    return border.OrderBy(c => c.x + c.y * 0.001).ToList();
                }

                private static (float x, float y) EstimateOutwardNormal(Coords c, int[,] grid, Func<int, bool> InRange, Func<int, int, (int, int)?> Wrap, int[] dxAll, int[] dyAll)
                {
                    float nx = 0, ny = 0;
                    for (int i = 0; i < dxAll.Length; i++)
                    {
                        var n = Wrap(c.x + dxAll[i], c.y + dyAll[i]);
                        if (n == null) continue;
                        int nxCell = n.Value.Item1;
                        int nyCell = n.Value.Item2;

                        if (!InRange(grid[nxCell, nyCell]))
                        {
                            nx += dxAll[i];
                            ny += dyAll[i];
                        }
                    }
                    float length = (float)Math.Sqrt(nx * nx + ny * ny);
                    if (length == 0) return (0, 0);
                    return (nx / length, ny / length);
                }
            }


        }


        //  Perforn Miscellaneous tasks related to matrices
        public class Misc()
        {

            #region Convert a group of Coords into an array
            //  Given a List of List of Cells, convert it into an array of values. Each list is represented by an int; unclaimed cells are represented by a zero
            public static int[,] ConvertCoordstoArray(List<List<Coords>> superlistCoords, int rows, int cols)
            {
                int[,] array = new int[rows, cols];
                //  Initialize array

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = 0;
                    }
                }

                int ID = 1;
                foreach (List<Coords> coordslist in superlistCoords)
                {
                    foreach (Coords coord in coordslist)
                    {
                        array[coord.x, coord.y] = ID;
                    }
                    ID++;

                }



                return array;
            }
            public static int[,] ConvertCoordstoArray(List<Coords> listCoords, int rows, int cols)
            {
                List<List<Coords>> superlistCoords = new List<List<Coords>>();
                superlistCoords.Add(listCoords);

                int[,] array = new int[rows, cols];
                //  Initialize array

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = 0;
                    }
                }

                int ID = 1;
                foreach (List<Coords> coordslist in superlistCoords)
                {
                    foreach (Coords coord in coordslist)
                    {
                        array[coord.x, coord.y] = ID;
                    }
                    ID++;

                }



                return array;
            }
            #endregion




            //  TODO: Make this better
            //  It's a function meant to influence arrays
            public static int[,] InfluenceArray(double[,] influence, int[,] baseMap, double radiationStrength)
            {
                int rows = baseMap.GetLength(0);
                int cols = baseMap.GetLength(1);

                // Output array starts as a copy of baseMap
                int[,] result = new int[rows, cols];
                Array.Copy(baseMap, result, baseMap.Length);

                // Determine a practical radius of effect based on strength
                // e.g. radius = ceil(strength * 3) for significant falloff
                int radius = Math.Max(1, (int)Math.Ceiling(radiationStrength * 3));

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double sourceVal = influence[r, c];
                        if (Math.Abs(sourceVal) < double.Epsilon)
                            continue; // skip zero sources

                        // Affect neighbors within radius
                        for (int nr = Math.Max(0, r - radius); nr <= Math.Min(rows - 1, r + radius); nr++)
                        {
                            for (int nc = Math.Max(0, c - radius); nc <= Math.Min(cols - 1, c + radius); nc++)
                            {
                                double dist = Math.Sqrt((nr - r) * (nr - r) + (nc - c) * (nc - c));
                                if (dist > radius) continue;

                                // Influence decays exponentially with distance
                                double decay = Math.Exp(-dist / radiationStrength);

                                // Change is proportional to sourceVal * decay
                                double delta = sourceVal * decay;

                                result[nr, nc] = (int)Math.Round(result[nr, nc] + delta);
                            }
                        }
                    }
                }

                return result;
            }

            #region Return the dimensions of a specified shape
            //  Get the maximum column distance within a set of points
            public static int MaxColDistance(List<Coords> points)
            {
                if (points == null || points.Count < 2) return 0;

                int minY = int.MaxValue;
                int maxY = int.MinValue;

                foreach (var p in points)
                {
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                }

                return Math.Abs(maxY - minY);
            }

            //  Get the maximum x distance betweens two points
            public static int MaxRowDistance(List<Coords> points)
            {
                if (points == null || points.Count < 2) return 0;

                int minX = int.MaxValue;
                int maxX = int.MinValue;

                foreach (var p in points)
                {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                }

                return Math.Abs(maxX - minX);
            }

            #endregion

            #region Matrix Manipulation

            // Rotate a 2D matrix clockwise n times (each rotation = 90 degrees)
            public static T[,] Rotate2DMatrix<T>(T[,] input, int rotations)
            {
                int rowCount = input.GetLength(0);
                int colCount = input.GetLength(1);

                // Normalize rotations (4 rotations = original array)
                rotations = ((rotations % 4) + 4) % 4;

                T[,] result = input;

                for (int r = 0; r < rotations; r++)
                {
                    T[,] rotated = new T[colCount, rowCount];

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            rotated[j, rowCount - 1 - i] = result[i, j];
                        }
                    }

                    result = rotated;
                    rowCount = result.GetLength(0);
                    colCount = result.GetLength(1);
                }

                return result;
            }


            // Reflect a 2D matrix vertically and/or horizontally
            public static T[,] Flip2DMatrix<T>(T[,] input, bool verticalFlip, bool horizontalFlip)
            {
                int rowCount = input.GetLength(0);
                int colCount = input.GetLength(1);

                T[,] result = new T[rowCount, colCount];

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        int newI = verticalFlip ? rowCount - 1 - i : i;
                        int newJ = horizontalFlip ? colCount - 1 - j : j;

                        result[newI, newJ] = input[i, j];
                    }
                }

                return result;
            }


            //  Given an array, scale it to a new size. This function does not attempt to interpolate values.
            public static T[,] ScaleArray<T>(T[,] input, int newRows, int newCols)
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                if (newRows <= 0 || newCols <= 0)
                    throw new ArgumentOutOfRangeException("New dimensions must be positive.");

                int oldRows = input.GetLength(0);
                int oldCols = input.GetLength(1);

                T[,] result = new T[newRows, newCols];

                for (int r = 0; r < newRows; r++)
                {
                    // Map new row index to original row index
                    int srcRow = (int)((long)r * oldRows / newRows);

                    for (int c = 0; c < newCols; c++)
                    {
                        // Map new column index to original column index
                        int srcCol = (int)((long)c * oldCols / newCols);

                        result[r, c] = input[srcRow, srcCol];
                    }
                }

                return result;
            }
            #endregion


        }

    }


    public static class Noise
    {
        public static class Perlin1D
        {
            /// <summary>
            /// Deterministic 1D gradient-like noise using doubles.
            /// Returns a value in [-amplitude, +amplitude].
            /// </summary>
            /// <param name="x">Position along the 1D line (e.g., border index)</param>
            /// <param name="amplitude">Maximum absolute output (units = grid cells)</param>
            /// <param name="frequency">How rapidly the noise varies (higher -> more cycles per unit)</param>
            /// <param name="smoothness">Controls effective smoothing; values >= 1.0. Higher = smoother (slower) changes.</param>
            /// <param name="seed">Integer seed for deterministic results</param>
            public static double GetValue(double x, double amplitude, double frequency, double smoothness, int seed)
            {
                if (smoothness <= 0.0) smoothness = 1.0; // guard
                                                         // Map x into noise space: frequency controls oscillation; smoothness reduces effective frequency
                double xp = x * (frequency / Math.Max(1.0, smoothness));

                int x0 = (int)Math.Floor(xp);
                int x1 = x0 + 1;

                double t = xp - x0;
                // smoothstep: t -> 3t^2 - 2t^3 for smoother interpolation
                double tSmooth = t * t * (3.0 - 2.0 * t);

                double g0 = Gradient(x0, seed);
                double g1 = Gradient(x1, seed);

                double interpolated = Lerp(g0, g1, tSmooth);

                // Clamp for safety, then scale
                if (interpolated < -1.0) interpolated = -1.0;
                if (interpolated > 1.0) interpolated = 1.0;

                return interpolated * amplitude;
            }

            // Small deterministic hash -> gradient in [-1,1]
            private static double Gradient(int i, int seed)
            {
                unchecked
                {
                    long h = i;
                    h = (h << 13) ^ h;
                    h = h + (seed * 0x9E3779B9); // incorporate seed
                    long hash = (h * (h * h * 15731L + 789221L) + 1376312589L) & 0x7fffffffL;
                    // map to [0,1]
                    double normalized = (double)hash / (double)0x7fffffffL;
                    // map to [-1,1]
                    return normalized * 2.0 - 1.0;
                }
            }

            private static double Lerp(double a, double b, double t) => a + (b - a) * t;
        }
        public static class Perlin2D
        {

            public static double[,] GeneratePerlinNoise(int rows, int cols, double frequency, int seed)
            {
                double[,] noise = new double[rows, cols];
                PerlinNoise2D perlin = new PerlinNoise2D(seed);

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        // Scale coordinates so larger arrays sample the same underlying noise field
                        double sampleX = (x / (double)cols) * frequency;
                        double sampleY = (y / (double)rows) * frequency;

                        noise[y, x] = perlin.Noise(sampleX, sampleY);
                    }
                }

                return noise;
            }
            public static int[,] GeneratePerlinInt(int rows, int cols, double frequency, int minRange, int maxRange, int seed)
            {
                double[,] perlinArrayDouble = GeneratePerlinNoise(rows, cols, frequency, seed);
                int[,] perlinArray = Utility.Mathematics.NormalizeToInteger(perlinArrayDouble, minRange, maxRange);
                return perlinArray;
            }


            private class PerlinNoise2D
            {
                private readonly int[] permutation;
                public PerlinNoise2D(int seed)
                {
                    permutation = new int[512];
                    var random = new Random(seed);

                    int[] p = new int[256];
                    for (int i = 0; i < 256; i++) p[i] = i;

                    // Shuffle
                    for (int i = 255; i > 0; i--)
                    {
                        int swapIndex = random.Next(i + 1);
                        int temp = p[i];
                        p[i] = p[swapIndex];
                        p[swapIndex] = temp;
                    }

                    // Duplicate to avoid overflow
                    for (int i = 0; i < 512; i++) permutation[i] = p[i % 256];
                }
                public double Noise(double x, double y)
                {
                    int xi = (int)Math.Floor(x) & 255;
                    int yi = (int)Math.Floor(y) & 255;

                    double xf = x - Math.Floor(x);
                    double yf = y - Math.Floor(y);

                    double u = Fade(xf);
                    double v = Fade(yf);

                    int aa = permutation[permutation[xi] + yi];
                    int ab = permutation[permutation[xi] + yi + 1];
                    int ba = permutation[permutation[xi + 1] + yi];
                    int bb = permutation[permutation[xi + 1] + yi + 1];

                    double x1, x2;
                    x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
                    x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

                    return Lerp(x1, x2, v);
                }
                private static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
                private static double Lerp(double a, double b, double t) => a + t * (b - a);
                private static double Grad(int hash, double x, double y)
                {
                    int h = hash & 7; // 8 directions
                    double u = (h < 4) ? x : y;
                    double v = (h < 4) ? y : x;
                    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
                }
            }
        }

        public static class Gradients
        {
            public static int[,] GenerateGradientNoise(int mapRows, int mapCols, int minimumVal, int maximumVal, bool middleCenter, int seed, double distortStrength)
            {
                int[,] map = new int[mapRows, mapCols];

                // Create Linear Gradient
                for (int r = 0; r < mapRows; r++)
                {
                    double t;
                    if (middleCenter)
                    {
                        double distFromMid = Math.Abs((mapRows - 1) / 2.0 - r);
                        double maxDist = (mapRows - 1) / 2.0;
                        t = distFromMid / maxDist;           // 0 center -> 1 edge
                    }
                    else
                    {
                        double distFromEdge = Math.Min(r, mapRows - 1 - r);
                        double maxDist = (mapRows - 1) / 2.0;
                        t = 1.0 - (distFromEdge / maxDist);  // 1 edge -> 0 center
                    }

                    int rowVal = (int)Math.Round(
                        maximumVal + (minimumVal - maximumVal) * t);

                    for (int c = 0; c < mapCols; c++)
                        map[r, c] = rowVal;
                }

                //  Apply distortion (TODO: Create this so that it can use the previous noise smoother)
                double[,] noise = GenerateGradientDistortion(mapRows, mapCols, seed, scale: 0.1);


                for (int r = 0; r < mapRows; r++)
                {
                    for (int c = 0; c < mapCols; c++)
                    {
                        // noise[r,c] is in [-1,1]; scale by distortStrength
                        double offset = noise[r, c] * distortStrength;
                        int newVal = (int)Math.Round(map[r, c] + offset);

                        // Clamp to range
                        if (newVal < minimumVal) newVal = minimumVal;
                        if (newVal > maximumVal) newVal = maximumVal;

                        map[r, c] = newVal;
                    }
                }

                return map;
            }

            //  TODO: Modify this to use the Perlin2D generateDistortion
            private static double[,] GenerateGradientDistortion(int rows, int cols, int seed, double scale)
            {
                // Smaller scale => larger features (less wavy)
                Random rand = new Random(seed);
                int gridRows = (int)Math.Ceiling(rows * scale) + 2;
                int gridCols = (int)Math.Ceiling(cols * scale) + 2;

                // Create coarse random grid of values in [-1,1]
                double[,] coarse = new double[gridRows, gridCols];
                for (int i = 0; i < gridRows; i++)
                    for (int j = 0; j < gridCols; j++)
                        coarse[i, j] = rand.NextDouble() * 2.0 - 1.0;

                // Interpolate to full size
                double[,] noise = new double[rows, cols];
                for (int y = 0; y < rows; y++)
                {
                    double gy = y * scale;
                    int g0y = (int)Math.Floor(gy);
                    double ty = gy - g0y;

                    for (int x = 0; x < cols; x++)
                    {
                        double gx = x * scale;
                        int g0x = (int)Math.Floor(gx);
                        double tx = gx - g0x;

                        // Corners of the cell
                        double v00 = coarse[g0y, g0x];
                        double v10 = coarse[g0y, g0x + 1];
                        double v01 = coarse[g0y + 1, g0x];
                        double v11 = coarse[g0y + 1, g0x + 1];

                        // Bilinear interpolation
                        double v0 = Lerp(v00, v10, tx);
                        double v1 = Lerp(v01, v11, tx);
                        noise[y, x] = Lerp(v0, v1, ty);
                    }
                }
                return noise;
            }
            private static double Lerp(double a, double b, double t) => a + (b - a) * t;
        }

    }


    public static class Mathematics
    {
        //  Converts an integer within a range to a new range
        public static int ConvertRange(int originalStart, int originalEnd, int newStart, int newEnd, int value)
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }

        //  Given a 2D array of doubles, normalize the array to a new scale
        public static int[,] NormalizeToInteger(double[,] inputArray, int min, int max)
        {
            int row = inputArray.GetLength(0);
            int col = inputArray.GetLength(1);


            int newmax = max;
            int[,] intArray = new int[row, col];

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            // Find min and max values in the double array
            for (int x = 0; x < row; x++)
            {
                for (int y = 0; y < col; y++)
                {
                    if (inputArray[x, y] < minValue) minValue = inputArray[x, y];
                    if (inputArray[x, y] > maxValue) maxValue = inputArray[x, y];
                }
            }


            // Normalize and scale values to the new range
            for (int x = 0; x < row; x++)
            {
                for (int y = 0; y < col; y++)
                {
                    intArray[x, y] = (int)(min + (inputArray[x, y] - minValue) / (maxValue - minValue) * (newmax - min));
                }
            }

            return intArray;
        }

    }


    public static class Print
    {
        #region Prints in specified Color codes
        //  Prints text in color
        public static void Write(object value, string hexCodeForeground = "FFFFFF", string hexCodeBackground = "#000000")
        {
            var (fr, fg, fb) = HexToRgb(hexCodeForeground);
            var (br, bg, bb) = HexToRgb(hexCodeBackground);

            // \x1b is the ESC character (ASCII 27)
            string ansi = $"\x1b[38;2;{fr};{fg};{fb}m\x1b[48;2;{br};{bg};{bb}m{value}\x1b[0m";
            Console.Write(ansi);
        }

        //  Prints newline text in color
        public static void WriteLine(object value, string hexCodeForeground = "FFFFFF", string hexCodeBackground = "#000000")
        {
            Print.Write(value, hexCodeForeground, hexCodeBackground);
            Console.WriteLine();
        }


        private static (int r, int g, int b) HexToRgb(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("Color cannot be empty");
            }

            if (hex.StartsWith("#")) hex = hex[1..];

            if (hex.Length != 6)
            {
                //  TODO: If not valid, set it to plain white
                //throw new ArgumentException("Hex color " + hex + " must be in format #RRGGBB or RRGGBB"); 
                hex = "#FFFFFF";
            }

            int r = Convert.ToInt32(hex[..2], 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            return (r, g, b);
        }
        //  Parses a hex string (#RRGGBB or RRGGBB) into (R,G,B)
        

        #endregion

        public static void ClearLine(int numLines)
        {
            if (numLines <= 0) return;

            int currentLine = Console.CursorTop;
            int linesToClear = Math.Min(numLines, currentLine + 1);

            for (int i = 0; i < linesToClear; i++)
            {
                Console.SetCursorPosition(0, currentLine - i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLine - linesToClear + 1);
        }





        public static class MatrixPrint
        {
            private static readonly Random rng = new Random();
            // Characters to use for display (excluding problematic ones)
            private static readonly char[] AllowedChars = GenerateAllowedChars();

            private static char[] GenerateAllowedChars()
            {
                var list = new List<char>();
                for (int i = 33; i < 127; i++) // Printable ASCII
                {
                    char c = (char)i;
                    if (c != '/' && c != '"' && c != '\'' && c != '\\')
                        list.Add(c);
                }
                return list.ToArray();
            }

            /// <summary>
            /// Displays a 2D integer array as a rainbow-colored field.
            /// Values below floor are blank (no char, black background).
            /// </summary>
            public static void DisplayPerlin(int[,] data, int floor)
            {
                int height = data.GetLength(0);
                int width = data.GetLength(1);

                // Find min and max for normalization
                int min = int.MaxValue, max = int.MinValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int val = data[y, x];
                        if (val < min) min = val;
                        if (val > max) max = val;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int val = data[y, x];
                        if (val < floor)
                        {
                            // blank
                            //Console.Write(" ");
                            Utility.Print.Write(" ", "FFFFFF", "FFFFFF");
                            continue;
                        }

                        double t = (double)(val - min) / (max - min);
                        var (r, g, b) = RainbowColor(t);
                        string hex = $"#{r:X2}{g:X2}{b:X2}";
                        char c = AllowedChars[rng.Next(AllowedChars.Length)];

                        Utility.Print.Write(c, hex, "#000000");
                    }
                    Console.WriteLine();
                }
            }

            /// <summary>
            /// Returns an RGB value representing a rainbow color from red (0) to violet (1).
            /// </summary>
            private static (int r, int g, int b) RainbowColor(double t)
            {
                // Map through visible spectrum using HSV-like approach
                double hue = 270.0 * t; // Red → Violet (0°–270°)
                return HsvToRgb(hue, 1.0, 1.0);
            }

            private static (int r, int g, int b) HsvToRgb(double h, double s, double v)
            {
                double c = v * s;
                double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
                double m = v - c;
                double r = 0, g = 0, b = 0;

                if (h < 60) { r = c; g = x; b = 0; }
                else if (h < 120) { r = x; g = c; b = 0; }
                else if (h < 180) { r = 0; g = c; b = x; }
                else if (h < 240) { r = 0; g = x; b = c; }
                else { r = x; g = 0; b = c; }

                return ((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
            }
        }

    }


    public class Images
    {
        public class ImageFile
        {
            //  Given a filepath to an image, convert it to a bitmap
            public static Bitmap getBitmap(string path)
            {
                if (path == null)
                {
                    throw new ArgumentNullException(nameof(path), "The path cannot be null.");
                }
                Bitmap bitmap = new Bitmap(path);
                return bitmap;
            }

            //  Save a bitmap as a .png file at the specificed path
            public static void saveImage(Bitmap image, String path)
            {

                if (path == null)
                {
                    Console.WriteLine("Error: provided path is null");
                    return;
                }

                Console.WriteLine("Saving Image: ");


                Console.WriteLine(path);

                try { image.Save(path, System.Drawing.Imaging.ImageFormat.Png); }
                catch (Exception)
                {
                    Console.WriteLine("Error: filepath " + path + "is not valid");
                }

            }



            





            //  Given a Bitmap, convert it to a string array.
            public static string[,] BitmapToStringArray(Bitmap bitmap)
            {
                if (bitmap == null)
                    throw new ArgumentNullException(nameof(bitmap));

                int width = bitmap.Width;
                int height = bitmap.Height;
                string[,] hexArray = new string[height, width];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);
                        hexArray[y, x] = $"#{pixel.R:X2}{pixel.G:X2}{pixel.B:X2}";
                    }
                }

                return hexArray;
            }


            //  Given a string array, convert it to a Bitmap
            public static Bitmap StringArrayToBitmap(string[,] hexArray)
            {
                if (hexArray == null)
                {
                    throw new ArgumentNullException(nameof(hexArray));
                }

                int height = hexArray.GetLength(0);
                int width = hexArray.GetLength(1);

                Bitmap bitmap = new Bitmap(width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        string hex = hexArray[y, x];
                        if (string.IsNullOrWhiteSpace(hex))
                            throw new ArgumentException($"Null or empty hex string at ({y},{x})");

                        // Remove optional '#' and parse
                        string clean = hex.TrimStart('#');

                        if (clean.Length != 6)
                            throw new FormatException($"Invalid hex format at ({y},{x}): {hex}");

                        int r = Convert.ToInt32(clean.Substring(0, 2), 16);
                        int g = Convert.ToInt32(clean.Substring(2, 2), 16);
                        int b = Convert.ToInt32(clean.Substring(4, 2), 16);

                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                return bitmap;
            }

        }

        public class ImageDebug
        {
            public static void getHexArrayFromInt(int[,] array)
            {
                int mapRow = array.GetLength(0);
                int mapCol = array.GetLength(1);
                string[,] returnArray = new string[mapRow, mapCol];  
                //  Create a list of distinct values
                List<int> distinctValues = new List<int>();

                //  Find all distinct values
                for (int i = 0; i < mapRow; i++)
                {
                    for (int j = 0; j < mapCol; j++)
                    {
                        if (!distinctValues.Contains(array[i, j]))
                        {
                            distinctValues.Add(array[i, j]);
                        }
                    }
                }

                //  Assign each distinct value a color

            }

            /// <summary>
            /// Converts a 2D integer array into a 2D string array of hex color codes.
            /// Each distinct integer is mapped to a unique color ranging from red to violet.
            /// </summary>
            public static string[,] MapValuesToColors(int[,] input)
            {
                int rows = input.GetLength(0);
                int cols = input.GetLength(1);

                // Get distinct values and sort them
                var uniqueValues = new SortedSet<int>();
                foreach (var val in input) uniqueValues.Add(val);
                int count = uniqueValues.Count;

                // Assign each distinct value a color from red → violet
                Dictionary<int, string> colorMap = new Dictionary<int, string>();
                if (count == 1)
                {
                    // Single unique value → all red
                    colorMap[uniqueValues.First()] = "#FF0000";
                }
                else
                {
                    int i = 0;
                    foreach (var val in uniqueValues)
                    {
                        // Evenly spaced hues between 0° (red) and 270° (violet)
                        double hue = 0 + 270.0 * i / (count - 1);
                        Color color = FromHSV(hue, 1.0, 1.0);
                        colorMap[val] = ColorToHex(color);
                        i++;
                    }
                }

                // Create and fill the output array
                string[,] result = new string[rows, cols];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        result[r, c] = colorMap[input[r, c]];
                    }
                }

                return result;
            }

            // Convert HSV to RGB (for better spectrum mapping)
            private static Color FromHSV(double hue, double saturation, double value)
            {
                int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
                double f = hue / 60 - Math.Floor(hue / 60);

                value = value * 255;
                int v = Convert.ToInt32(value);
                int p = Convert.ToInt32(value * (1 - saturation));
                int q = Convert.ToInt32(value * (1 - f * saturation));
                int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

                return hi switch
                {
                    0 => Color.FromArgb(255, v, t, p),
                    1 => Color.FromArgb(255, q, v, p),
                    2 => Color.FromArgb(255, p, v, t),
                    3 => Color.FromArgb(255, p, q, v),
                    4 => Color.FromArgb(255, t, p, v),
                    _ => Color.FromArgb(255, v, p, q),
                };
            }

            // Convert Color to #RRGGBB format
            private static string ColorToHex(Color color)
            {
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }

        }

        public class ImageManipulation
        {




        }

    }


}
