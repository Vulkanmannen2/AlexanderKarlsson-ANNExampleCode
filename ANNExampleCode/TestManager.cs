using System;
using static ANN_test.ANN;

namespace ANN_test
{
    // Used to test parameters and variables
    // If value is not acceptable we write ERROR to console and in some cases add stack trace

    // Also used to write test data to console
    // This is done in the Print Manager menu
    // Write tests to console or change settings for writing test data in other situations
	public static class TestManager
    {
        private static bool PrintData = false;
        private static bool PrintResultWhenReadingLettersBool = false;

        static int[] layers = { 20, 20, 20 };
		static int width = 20;
		static int height = 20;

        public static void SetPrintSettingsFromInput(TrainANNReadLetters trainer)
        {
            PrintPrintSettingsInputInfo();

            bool continueEditing = true;

            while (continueEditing)
            {
                int input = Convert.ToInt32(Utility.ReadLineAsNumber());

                if (input == 0)
                {
                    continueEditing = false;
                    break;
                }
                else if (input == 1)
                {
                    TestANNFunktions(trainer);
                }
                else if (input == 2)
                {
                    Console.WriteLine("Enter int for PrintResultWhenReadingLettersBool: 1 for true 0 for false");
                    PrintResultWhenReadingLettersBool = Convert.ToBoolean(Convert.ToInt32(Utility.ReadLineAsNumber()));
                    Console.WriteLine("PrintResultWhenReadingLettersBool set to: " + PrintResultWhenReadingLettersBool);
                }
               
                Console.WriteLine("Next");
            }
        }

        private static void PrintPrintSettingsInputInfo()
        {
            Console.WriteLine(" ");
            Console.WriteLine("Press 0 to exit Print Manager");
            Console.WriteLine("Press 1 to print tests of ANN functions");
            Console.WriteLine("Press 2 to set PrintResultWhenReadingLettersBool");
            Console.WriteLine("Hit Enter to confirm your choice");
        }

        // Run a number of tests and print results to console 
        public static void TestANNFunktions(TrainANNReadLetters trainer)
		{
            PrintData = true;

            Console.WriteLine(" ");
            Console.WriteLine("Test Multipoint Crossover Reproduction");
            ANN p1 = new ANN(layers);
            ANN p2 = new ANN(layers);

            ANN child = p1.Reproduce(layers, p2, ANN.ReproductionType.MultipointCrossover, 4);

            Console.WriteLine(" ");
            Console.WriteLine("Test Uniform Crossover Reproduction");
            child = p1.Reproduce(layers, p2, ANN.ReproductionType.UniformCrossover);

            Console.WriteLine(" ");
            Console.WriteLine("Test Copy ANN");
            ANN copy = p1.Copy();

            Console.WriteLine(" ");
            Console.WriteLine("Mutate one copy and compare");
            p1.Mutate(10);
            Console.WriteLine("Equal: " + p1.Compare(copy));
            Console.WriteLine(" ");

            Console.WriteLine("Test fitness based on ANN output from one image.");
            Console.WriteLine("Output node equivalent to correct letter is set to value Right, other outputs are set to value Wrong.");
            for (int i = 0; i < 26; ++i)
            {
                PrintFitnessFromLetter(trainer, i, 1, 0);
            }

            Console.WriteLine(" ");

            PrintFitnessFromLetter(trainer, 5, 1, 0.5f);
            PrintFitnessFromLetter(trainer, 5, 1, 0.9f);
            PrintFitnessFromLetter(trainer, 5, 0.5f, 0f);
            PrintFitnessFromLetter(trainer, 5, 0.1f, 0f);
            PrintFitnessFromLetter(trainer, 5, 0f, 1f);
            PrintFitnessFromLetter(trainer, 5, 0.1f, 0.9f);
            PrintFitnessFromLetter(trainer, 5, 0f, 0f);
            PrintFitnessFromLetter(trainer, 5, 0.5f, 0.5f);
            PrintFitnessFromLetter(trainer, 5, 0.25f, 0.25f);

            PrintData = false;
        }

		public static void PrintFitnessFromLetter(TrainANNReadLetters trainer, int letterAsInt, float setRightAnser, float setWrongAnswers)
        {
            
            Console.Write("Fitness for " + Utility.indexToLetter[letterAsInt] +": right " + setRightAnser + ", wrong " + setWrongAnswers + " - ");

            List<float> LetterAsList = new List<float>();

            for (int i = 0; i < 26; ++i)
            {
                if (i == letterAsInt)
                    LetterAsList.Add(setRightAnser);
                else
                    LetterAsList.Add(setWrongAnswers);
            }
            float[] Letter = LetterAsList.ToArray();

            trainer.TestFitnessFunction(Letter, Utility.indexToLetter[letterAsInt]);
        }

        public static void PrintResultWhenReadingLetters(float[] result)
        {
            if (!PrintResultWhenReadingLettersBool)
                return;

            Console.Write(" - ");
            for (int i = 0; i < 26; ++i)
            {
                Console.Write(Math.Abs(result[i]) > Settings.thresholdForPickingALetter ? Math.Round(Math.Abs(result[i]), 2) + " " : "0 ");
            }
        }

        public static void TestFloat(float value, float min, float max)
        {
            if (value < min || value > max)
            {
                Console.WriteLine("ERROR - Value is out of Range - Value: " + value + " min/max: " + min + "/" + max);
                Console.WriteLine(new System.Diagnostics.StackTrace().ToString());
            }
        }

        public static bool CompareFloats(float value1, float value2)
        {
            if (value1 != value2)
                Console.WriteLine("ERROR - Floats not equal - Value1: " + value1 + " value2: " + value2);

            return value1 == value2;
        }

        public static bool CompareInts(int value1, int value2)
        {
            if (value1 != value2)
                Console.WriteLine("ERROR - Ints not equal - Value1: " + value1 + " value2: " + value2);

            return value1 == value2;
        }

        public static void TestInt(int value, int min, int max)
        {
            if (value < min || value > max)
                Console.WriteLine("ERROR - Value is out of Range - Value: " + value + " min/max: " + min + "/" + max);
        }

        public static void TestFloats(float[] values, float min, float max, int startIndex = 0)
        {
            for (int i = startIndex; i < values.Length; ++i)
                TestFloat(values[i], min, max);
        }

        public static void TestFloats(float[][] values, float min, float max)
        {
            for (int i = 0; i < values.Length; ++i)
                for (int j = 0; j < values[i].Length; ++j)
                    TestFloat(values[i][j], min, max);
        }

        public static void TestFloats(float[][][] values, float min, float max)
        {
            for (int i = 0; i < values.Length; ++i)
                for (int j = 0; j < values[i].Length; ++j)
                    for (int k = 0; k < values[i][j].Length; ++k)
                        TestFloat(values[i][j][k], min, max);
        }

        public static bool CompareFloats(float[][][] values1, float[][][] values2)
        {
            bool equal = true;
            List<List<string>> PrintableFloatsAreEqual = new List<List<string>>();
            if (CompareInts(values1.Length, values1.Length))
            {
                for (int i = 0; i < values1.Length; ++i)
                {
                    if (CompareInts(values1[i].Length, values2[i].Length))
                    {
                        PrintableFloatsAreEqual.Add(new List<string>());
                        for (int j = 0; j < values1[i].Length; ++j)
                        {
                            if (CompareInts(values1[i][j].Length, values2[i][j].Length))
                            {
                                PrintableFloatsAreEqual[i].Add("");
                                for (int k = 0; k < values1[i][j].Length; ++k)
                                {
                                    bool floatEqual = CompareFloats(values1[i][j][k], values2[i][j][k]);
                                    PrintableFloatsAreEqual[i][j] += floatEqual ? "X" : "0";
                                    equal = !floatEqual ? false : equal;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            if (PrintData)
            {
                Console.WriteLine("Arrays of floats are equal - X yes - 0 no");
                for (int i = 0; i < PrintableFloatsAreEqual.Count; ++i)
                {
                    for (int j = 0; j < PrintableFloatsAreEqual[i].Count; ++j)
                    {
                        Console.WriteLine(PrintableFloatsAreEqual[i][j]);
                    }
                    Console.WriteLine(" ");
                }
            }

            return equal;
        }

        public static bool CompareFloats(float[][] values1, float[][] values2)
        {
            bool equal = true;
            List<string> PrintableFloatsAreEqual = new List<string>();
            if (CompareInts(values1.Length, values1.Length))
            {
                for (int i = 0; i < values1.Length; ++i)
                {
                    PrintableFloatsAreEqual.Add("");
                    if (CompareInts(values1[i].Length, values2[i].Length))
                    {
                        for (int j = 0; j < values1[i].Length; ++j)
                        {
                            bool floatEqual = CompareFloats(values1[i][j], values2[i][j]);
                            PrintableFloatsAreEqual[i] += floatEqual ? "X" : "0";
                            equal = !floatEqual ? false : equal;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            if (PrintData)
            {
                Console.WriteLine("Arrays of floats are equal - X yes - 0 no");
                for (int i = 0; i < PrintableFloatsAreEqual.Count; ++i)
                {
                    Console.WriteLine(PrintableFloatsAreEqual[i]);
                }
            }

            return equal;
        }

        public static void TestInts(int[] values, int min, int max)
        {
            for (int i = 0; i < values.Length; ++i)
                TestInt(values[i], min, max);
        }

        public static void TestInts(int[] values, bool cantBeNegative = true)
        {
            for (int i = 0; i < values.Length; ++i)
                TestInt(values[i], cantBeNegative);
        }

        public static void TestInt(int value, int exactValue)
        {
            if (value != exactValue)
                Console.WriteLine("ERROR - Value is not exact - Actual value: " + value + " Should be: " + exactValue);
        }

        public static void TestInt(int value, bool cantBeNegative = true)
        {
            if (value < 0 && cantBeNegative)
                Console.WriteLine("ERROR - Value is less than zero: " + value);
            if (value > 0 && !cantBeNegative)
                Console.WriteLine("ERROR - Value is more than zero: " + value);
        }

        public static void TestString(string String, int expectedLength)
        {
            if (String.Length != expectedLength)
                Console.WriteLine("ERROR - String not expected lengt - String: " + String + " Expected lenght: " + expectedLength);
        }

        public static void TestBiasisMatchLayers(float[][] biasis, int[] layers)
        {
            if(biasis.Length == layers.Length)
            {
                for (int i = 0; i < layers.Length; ++i)
                    if (biasis[i].Length != layers[i])
                        Console.WriteLine("ERROR - Length of layer not equal biasis");
            }
            else
            {
                Console.WriteLine("ERROR - Number of layers not equal biasis");
            }
        }

        public static void TestWeightsMatchLayers(float[][][] weights, int[] layers)
        {
            if (weights.Length + 1 == layers.Length)
            {
                for (int i = 1; i < layers.Length; ++i)
                {
                    if (weights[i - 1].Length == layers[i])
                    {
                        for (int j = 0; j < layers[i]; ++j)
                        {
                            if (weights[i - 1][j].Length != layers[i-1])
                                Console.WriteLine("ERROR - Numbre of weight not equal to last layer");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR - Lengt of layers not equal weights dimention 2");
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR - Number of layers not equal to dimetion 1 weights+1 - Layers: " + layers.Length + " Weights: " + weights.Length);
            }
        }

        public static bool CompareFormArrays(float[][] array1, float[][] array2)
        {
            bool equal = true;

            if (array1.Length == array2.Length)
            {
                for (int i = 0; i < array1.Length; ++i)
                {
                    if (array1[i].Length != array2[i].Length)
                    {
                        equal = false;
                        Console.WriteLine("ERROR - Arrays not equal dimetion 2");
                        return equal;
                    }
                }
            }
            else
            {
                equal = false;
                Console.WriteLine("ERROR - Arrays not equal dimetion 1");
            }

            return equal;
        }

        public static bool CompareFormArrays(float[][][] array1, float[][][] array2)
        {
            bool equal = true;

            if (array1.Length == array2.Length)
            {
                for (int i = 0; i < array1.Length; ++i)
                {
                    if (array1[i].Length == array2[i].Length)
                    {
                        for (int j = 0; j < array1[i].Length; ++j)
                        {
                            if (array1[i][j].Length != array2[i][j].Length)
                            {
                                equal = false;
                                Console.WriteLine("ERROR - Arrays not equal dimetion 3");
                                return equal;
                            }
                        }
                    }
                    else
                    {
                        equal = false;
                        Console.WriteLine("ERROR - Arrays not equal dimetion 2");
                        return equal;
                    }
                }
            }
            else
            {
                equal = false;
                Console.WriteLine("ERROR - Arrays not equal dimetion 1");
            }

            return equal;
        }

        public static void TestBiasisUniformCrossover(float[][] child, float[][] parent1, float[][] parent2)
        {
            bool allEqualForm = CompareFormArrays(child, parent1) && CompareFormArrays(child, parent2);

            if(allEqualForm)
            {
                List<string> PrintableBiasEqualParents = new List<string>();
                for (int i = 0; i < child.Length; ++i)
                {
                    PrintableBiasEqualParents.Add("");
                    for(int j = 0; j < child[i].Length; ++j)
                    {
                        if (child[i][j] != parent1[i][j] && child[i][j] != parent2[i][j])
                        {
                            Console.WriteLine("ERROR - One bias not equal ether parent");
                        }

                        if (child[i][j] == parent1[i][j] && child[i][j] == parent2[i][j])
                            PrintableBiasEqualParents[i] += "X";
                        else if (child[i][j] == parent1[i][j])
                            PrintableBiasEqualParents[i] += "1";
                        else if (child[i][j] == parent2[i][j])
                            PrintableBiasEqualParents[i] += "2";
                        else
                            PrintableBiasEqualParents[i] += "0";
                    }
                }

                if (PrintData)
                {
                    Console.WriteLine("Uniform Crossover - Printing biasis is equal parnet 1 or 2, X for both and 0 for none");
                    for (int i = 0; i < PrintableBiasEqualParents.Count; ++i)
                    {
                        Console.WriteLine(PrintableBiasEqualParents[i]);
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR - Biasis not equal form");
            }
        }

        public static void TestWeightsUniformCrossover(float[][][] child, float[][][] parent1, float[][][] parent2)
        {
            bool allEqualForm = CompareFormArrays(child, parent1) && CompareFormArrays(child, parent2);

            if (allEqualForm)
            {
                List<List<string>> PrintableWeightsEqualParents = new List<List<string>>();
                for (int i = 0; i < child.Length; ++i)
                {
                    PrintableWeightsEqualParents.Add(new List<string>());
                    for (int j = 0; j < child[i].Length; ++j)
                    {
                        PrintableWeightsEqualParents[i].Add("");
                        for (int k = 0; k < child[i][j].Length; ++k)
                        {
                            if (child[i][j][k] != parent1[i][j][k] && child[i][j][k] != parent2[i][j][k])
                            {
                                Console.WriteLine("ERROR - One weight not equal ether parent");
                            }

                            if (child[i][j][k] == parent1[i][j][k] && child[i][j][k] == parent2[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "X";
                            else if (child[i][j][k] == parent1[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "1";
                            else if (child[i][j][k] == parent2[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "2";
                            else
                                PrintableWeightsEqualParents[i][j] += "0";
                        }
                    }
                }

                if (PrintData)
                {
                    Console.WriteLine("Uniform Crossover - Printing weight is equal parnet 1 or 2, X for both and 0 for none");
                    for (int i = 0; i < PrintableWeightsEqualParents.Count; ++i)
                    {
                        for (int j = 0; j < PrintableWeightsEqualParents[i].Count; ++j)
                        {
                            Console.WriteLine(PrintableWeightsEqualParents[i][j]);
                        }
                        Console.WriteLine(" ");
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR - Weight not equal form");
            }
        }

        public static void TestBiasisMultipointCrossover(float[][] child, float[][] parent1, float[][] parent2, int[] crossingPoints)
        {
            bool allEqualForm = CompareFormArrays(child, parent1) && CompareFormArrays(child, parent2);

            if (allEqualForm)
            {
                List<string> PrintableBiasEqualParents = new List<string>();
                for (int i = 0; i < child.Length; ++i)
                {
                    bool p1 = true;
                    int nextPoint = 0;

                    PrintableBiasEqualParents.Add("");
                    for (int j = 0; j < child[i].Length; ++j)
                    {
                        if(j > crossingPoints[nextPoint])
                        {
                            p1 = !p1;
                            nextPoint = nextPoint >= crossingPoints.Length - 1 ? nextPoint : nextPoint + 1;
                        }

                        if ((p1 && child[i][j] != parent1[i][j]) || (!p1 && child[i][j] != parent2[i][j]))
                        {
                            Console.WriteLine("ERROR - One bias not equal the right parent");
                        }

                        if (child[i][j] == parent1[i][j] && child[i][j] == parent2[i][j])
                            PrintableBiasEqualParents[i] += "X";
                        else if (child[i][j] == parent1[i][j])
                            PrintableBiasEqualParents[i] += "1";
                        else if (child[i][j] == parent2[i][j])
                            PrintableBiasEqualParents[i] += "2";
                        else
                            PrintableBiasEqualParents[i] += "0";
                    }
                }

                if (PrintData)
                {
                    Console.WriteLine("Multipoint Crossover - Printing biasis is equal parnet 1 or 2, X for both and 0 for none");
                    for (int i = 0; i < PrintableBiasEqualParents.Count; ++i)
                    {
                        Console.WriteLine(PrintableBiasEqualParents[i]);
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR - Biasis not equal form");
            }
        }

        public static void TestWeightsMultipointCrossover(float[][][] child, float[][][] parent1, float[][][] parent2, int[] crossingPoints)
        {
            bool allEqualForm = CompareFormArrays(child, parent1) && CompareFormArrays(child, parent2);

            if (allEqualForm)
            {
                List<List<string>> PrintableWeightsEqualParents = new List<List<string>>();
                for (int i = 0; i < child.Length; ++i)
                {
                    bool p1 = true;
                    int nextPoint = 0;

                    PrintableWeightsEqualParents.Add(new List<string>());
                    for (int j = 0; j < child[i].Length; ++j)
                    {
                        if (j > crossingPoints[nextPoint])
                        {
                            p1 = !p1;
                            nextPoint = nextPoint >= crossingPoints.Length - 1 ? nextPoint : nextPoint + 1;
                        }

                        PrintableWeightsEqualParents[i].Add("");
                        for (int k = 0; k < child[i][j].Length; ++k)
                        {
                            if ((p1 && child[i][j][k] != parent1[i][j][k]) || (!p1 && child[i][j][k] != parent2[i][j][k]))
                            {
                                Console.WriteLine("ERROR - One weight not equal the right parent");
                            }

                            if (child[i][j][k] == parent1[i][j][k] && child[i][j][k] == parent2[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "X";
                            else if (child[i][j][k] == parent1[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "1";
                            else if (child[i][j][k] == parent2[i][j][k])
                                PrintableWeightsEqualParents[i][j] += "2";
                            else
                                PrintableWeightsEqualParents[i][j] += "0";
                        }
                    }
                }

                if (PrintData)
                {
                    Console.WriteLine("Multipoint Crossover - Printing weight is equal parnet 1 or 2, X for both and 0 for none");
                    for (int i = 0; i < PrintableWeightsEqualParents.Count; ++i)
                    {
                        for (int j = 0; j < PrintableWeightsEqualParents[i].Count; ++j)
                        {
                            Console.WriteLine(PrintableWeightsEqualParents[i][j]);
                        }
                        Console.WriteLine(" ");
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR - Weight not equal form");
            }
        }
    }
}

