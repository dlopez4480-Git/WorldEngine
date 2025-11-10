using System;
using System.Diagnostics;
using System.Drawing;

namespace testProgram
{
    public partial class WorldGen
    {
        #region Temperature Codes
        //  Note; these are normalized to farenehit, the morally correct temperature system. 
        public static readonly int temperatureCode_polarUltimate = -21;
        public static readonly int temperatureCode_polar5 = -13;
        public static readonly int temperatureCode_polar4 = 5;
        #endregion

        public partial class GeographyGenerator
        {
            public class TemperatureGenerator
            {
                public static Bitmap createTemperatureBitmap(int[,] temperatureMap, int[,] landMap)
                {
                    string[,] temperatureBitmapString = new string[temperatureMap.GetLength(0), temperatureMap.GetLength(1)];

                    for (int i = 0; i < temperatureMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < temperatureMap.GetLength(1); j++)
                        {
                            if (landMap[i, j] >= landCode_lowLand)
                            {
                                //  Polar Zone
                                if (temperatureMap[i, j] < temperatureCode_polarUltimate)
                                {
                                    temperatureBitmapString[i, j] = "27414f";
                                }
                                else if (temperatureMap[i, j] >= temperatureCode_polarUltimate && temperatureMap[i, j] < temperatureCode_polar5)
                                {
                                    temperatureBitmapString[i, j] = "45748c";
                                }
                                else if (temperatureMap[i, j] >= temperatureCode_polar5 && temperatureMap[i, j] < temperatureCode_polar4)
                                {
                                    temperatureBitmapString[i, j] = "548EAB";
                                }
                                else if (temperatureMap[i, j] >= temperatureCode_polar4 && temperatureMap[i, j] < 14)
                                {
                                    temperatureBitmapString[i, j] = "8ac8e6";
                                }
                                else if (temperatureMap[i, j] >= 14 && temperatureMap[i, j] < 23)
                                {
                                    temperatureBitmapString[i, j] = "00f0ff";
                                }
                                else if (temperatureMap[i, j] >= 23 && temperatureMap[i, j] < 32)
                                {
                                    temperatureBitmapString[i, j] = "#8af091";
                                }

                                else if (temperatureMap[i, j] >= 23 && temperatureMap[i, j] < 32)
                                {
                                    temperatureBitmapString[i, j] = "8af091";
                                }
                                else if (temperatureMap[i, j] >= 32 && temperatureMap[i, j] < 41)
                                {
                                    temperatureBitmapString[i, j] = "eaff91";
                                }
                                else if (temperatureMap[i, j] >= 41 && temperatureMap[i, j] < 50)
                                {
                                    temperatureBitmapString[i, j] = "ffff00";
                                }
                                else if (temperatureMap[i, j] >= 50 && temperatureMap[i, j] < 59)
                                {
                                    temperatureBitmapString[i, j] = "b69105";
                                }
                                else if (temperatureMap[i, j] >= 59 && temperatureMap[i, j] < 68)
                                {
                                    temperatureBitmapString[i, j] = "#ff8f00";
                                }
                                else if (temperatureMap[i, j] >= 68 && temperatureMap[i, j] < 86)
                                {
                                    temperatureBitmapString[i, j] = "ff5400";
                                }
                                //  Tropical Zone
                                else if (temperatureMap[i, j] >= 86 && temperatureMap[i, j] < 95)
                                {
                                    temperatureBitmapString[i, j] = "#ff0000";
                                }
                                else if (temperatureMap[i, j] >= 95 && temperatureMap[i, j] < 104)
                                {
                                    temperatureBitmapString[i, j] = "c20000";
                                }
                                else if (temperatureMap[i, j] >= 104)
                                {
                                    temperatureBitmapString[i, j] = "610000";
                                }

                            }
                            else
                            {
                                temperatureBitmapString[i, j] =  "D6D6D6";
                            }
                        }
                    }



                    Bitmap landBitmap = Utility.Images.ImageFile.StringArrayToBitmap(temperatureBitmapString);
                    return landBitmap;


                }

                public static int[,] GenerateTemperatureMap(string[] args, int[,] LandMap)
                {
                    #region Initial Parameters
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    int mapsizeModifier = Convert.ToInt32((double)((LandMap.GetLength(0) + LandMap.GetLength(1)) / 2));
                    #endregion


                    string directoryLocation = Utility.Files.GetValidPath("\\debug\\geogeneration\\temperature\\");
                    bool debug_printMapToImage = true;


                    int randomSeed = random.Next(-9999, 9999);
                    int minValTemperature = 0;
                    int maxValTemperature = 80;
                    double waveval = 5.5;
                    int[,] temperatureMap = Utility.Noise.Gradients.GenerateGradientNoise(LandMap.GetLength(0), LandMap.GetLength(1), minValTemperature, maxValTemperature, false, random.Next(-999789, 779999), waveval);
                    
                    




                    if (debug_printMapToImage)
                    {
                        string filepath = directoryLocation + "\\temperatureMap.png";
                        Bitmap image = WorldGen.GeographyGenerator.TemperatureGenerator.createTemperatureBitmap(temperatureMap, LandMap);
                        Utility.Images.ImageFile.saveImage(image, filepath);
                    }
                    return temperatureMap;
                }
            }
        }
    }
}
