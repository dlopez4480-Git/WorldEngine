using System;
using System.Diagnostics;
using System.Drawing;

namespace testProgram
{
    public partial class WorldGen
    {
        #region Temperature Codes
        //  Note; these are normalized to farenehit, the morally correct temperature system. 
        public static readonly int temperatureCode_polarUltimaHigh = -21;

        public static readonly int temperatureCode_polarDeltaLow = -13;
        public static readonly int temperatureCode_polarBetaLow = 5;
        public static readonly int temperatureCode_polarAlphaLow = 14;

        public static readonly int temperatureCode_subpolarBetaLow = 23;

        public static readonly int temperatureCode_subpolarAlphaLow = 32;

        public static readonly int temperatureCode_TemperateColdLow = 41;
        public static readonly int temperatureCode_TemperateLow = 50;
        public static readonly int temperatureCode_TemperateWarmLow = 59;
        

        public static readonly int temperatureCode_subtropicalLow = 68;
        public static readonly int temperatureCode_tropicalBetaLow = 86;

        public static readonly int temperatureCode_tropicalAlphaLow = 95;
        public static readonly int temperatureCode_tropicalScorching = 104;
        




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
                                /// Super-Frozen Zone
                                //  Frozen
                                if (temperatureMap[i, j] < temperatureCode_polarUltimaHigh)
                                {
                                    temperatureBitmapString[i, j] = "27414f";
                                }



                                /// Polar Zone
                                //  Polar Delta
                                else if (temperatureMap[i, j] >= temperatureCode_polarUltimaHigh && temperatureMap[i, j] < temperatureCode_polarDeltaLow)
                                {
                                    temperatureBitmapString[i, j] = "45748c";
                                }
                                //  Polar Gamma
                                else if (temperatureMap[i, j] >= temperatureCode_polarDeltaLow && temperatureMap[i, j] < temperatureCode_polarBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "548EAB";
                                }
                                //  Polar Beta
                                else if (temperatureMap[i, j] >= temperatureCode_polarBetaLow && temperatureMap[i, j] < temperatureCode_polarAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "8ac8e6";
                                }
                                //  Polar Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_polarAlphaLow && temperatureMap[i, j] < temperatureCode_subpolarBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "00f0ff";
                                }






                                /// Sub-Polar Zone
                                //  Sub-Polar Beta
                                else if (temperatureMap[i, j] >= temperatureCode_subpolarBetaLow && temperatureMap[i, j] < temperatureCode_subpolarAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "#8af091";
                                }
                                //  Sub-Polar Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_subpolarAlphaLow && temperatureMap[i, j] < temperatureCode_TemperateColdLow)
                                {
                                    temperatureBitmapString[i, j] = "eaff91";
                                }




                                /// Temperate Zone
                                //  Cold Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateColdLow && temperatureMap[i, j] < temperatureCode_TemperateLow)
                                {
                                    temperatureBitmapString[i, j] = "ffff00";
                                }
                                //  Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateLow && temperatureMap[i, j] < temperatureCode_TemperateWarmLow)
                                {
                                    temperatureBitmapString[i, j] = "b69105";
                                }
                                //  Warm Temperate 
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateWarmLow && temperatureMap[i, j] < temperatureCode_subtropicalLow)
                                {
                                    temperatureBitmapString[i, j] = "#ff8f00";
                                }





                                /// Sub-Tropical Zone
                                //  Subtropical
                                else if (temperatureMap[i, j] >= temperatureCode_subtropicalLow && temperatureMap[i, j] < temperatureCode_tropicalBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "ff5400";
                                }





                                /// Tropical Zone
                                //  Tropical Beta
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalBetaLow && temperatureMap[i, j] < temperatureCode_tropicalAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "#ff0000";
                                }
                                //  Tropical Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalAlphaLow && temperatureMap[i, j] < temperatureCode_tropicalScorching)
                                {
                                    temperatureBitmapString[i, j] = "c20000";
                                }


                                /// Super-Hot Zone
                                //  Scorching
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalScorching)
                                {
                                    temperatureBitmapString[i, j] = "610000";
                                }

                            }
                            else
                            {
                                /// Super-Frozen Zone
                                //  Frozen
                                if (temperatureMap[i, j] < temperatureCode_polarUltimaHigh)
                                {
                                    temperatureBitmapString[i, j] = "#989898";
                                }



                                /// Polar Zone
                                //  Polar Delta
                                else if (temperatureMap[i, j] >= temperatureCode_polarUltimaHigh && temperatureMap[i, j] < temperatureCode_polarDeltaLow)
                                {
                                    temperatureBitmapString[i, j] = "#9B9B9B";
                                }
                                //  Polar Gamma
                                else if (temperatureMap[i, j] >= temperatureCode_polarDeltaLow && temperatureMap[i, j] < temperatureCode_polarBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "#9E9E9E";
                                }
                                //  Polar Beta
                                else if (temperatureMap[i, j] >= temperatureCode_polarBetaLow && temperatureMap[i, j] < temperatureCode_polarAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "#A1A1A1";
                                }
                                //  Polar Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_polarAlphaLow && temperatureMap[i, j] < temperatureCode_subpolarBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "#A5A5A5";
                                }






                                /// Sub-Polar Zone
                                //  Sub-Polar Beta
                                else if (temperatureMap[i, j] >= temperatureCode_subpolarBetaLow && temperatureMap[i, j] < temperatureCode_subpolarAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "#A9A9A9";
                                }
                                //  Sub-Polar Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_subpolarAlphaLow && temperatureMap[i, j] < temperatureCode_TemperateColdLow)
                                {
                                    temperatureBitmapString[i, j] = "#AEAEAE";
                                }




                                /// Temperate Zone
                                //  Cold Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateColdLow && temperatureMap[i, j] < temperatureCode_TemperateLow)
                                {
                                    temperatureBitmapString[i, j] = "#B3B3B3";
                                }
                                //  Temperate
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateLow && temperatureMap[i, j] < temperatureCode_TemperateWarmLow)
                                {
                                    temperatureBitmapString[i, j] = "#B9B9B9";
                                }
                                //  Warm Temperate 
                                else if (temperatureMap[i, j] >= temperatureCode_TemperateWarmLow && temperatureMap[i, j] < temperatureCode_subtropicalLow)
                                {
                                    temperatureBitmapString[i, j] = "#C0C0C0";
                                }





                                /// Sub-Tropical Zone
                                //  Subtropical
                                else if (temperatureMap[i, j] >= temperatureCode_subtropicalLow && temperatureMap[i, j] < temperatureCode_tropicalBetaLow)
                                {
                                    temperatureBitmapString[i, j] = "#CFCFCF";
                                }





                                /// Tropical Zone
                                //  Tropical Beta
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalBetaLow && temperatureMap[i, j] < temperatureCode_tropicalAlphaLow)
                                {
                                    temperatureBitmapString[i, j] = "#D8D8D8";
                                }
                                //  Tropical Alpha
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalAlphaLow && temperatureMap[i, j] < temperatureCode_tropicalScorching)
                                {
                                    temperatureBitmapString[i, j] = "#E2E2E2";
                                }


                                /// Super-Hot Zone
                                //  Scorching
                                else if (temperatureMap[i, j] >= temperatureCode_tropicalScorching)
                                {
                                    temperatureBitmapString[i, j] = "EDEDED";
                                }




                                //
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
                    int minValTemperature = -50;
                    int maxValTemperature = 90;
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
