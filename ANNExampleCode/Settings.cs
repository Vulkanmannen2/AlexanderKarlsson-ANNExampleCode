using System;
namespace ANN_test
{
    // ANNReadLetters and TrainANNReadLettes uses these settings
    // The settings can be alterd in the Settings menu
	public static class Settings
	{
        // settings that can't be changed runtime
        public const int numberOfOutputNodes = 26;
        public static readonly int[] layers = { 401, 100, numberOfOutputNodes };
        public const int imageWidth = 20;
        public const int imageHeight = 20;
        public const string resourceFolder = "Resources";
        public const string nameBestANNFile = "BestANN";
        // Only when executable is in same folder system as .sln
        public static readonly string pathImages = Utility.GetSolutionDirectoryInfo().ToString() + "/" + resourceFolder + "/";
        //public static readonly string pathImages = AppDomain.CurrentDomain.BaseDirectory + "/" + resourceFolder + "/";
        public const int numberOfImagesForTraining = 30;

        // settings that can be changed runtime
        // Only when executable is in same folder system as .sln
        public static string pathAndNameBestANN { get; private set; } = Utility.GetSolutionDirectoryInfo().ToString() + "/" + resourceFolder + "/" + nameBestANNFile + ".data";
        //public static string pathAndNameBestANN { get; private set; } = AppDomain.CurrentDomain.BaseDirectory + "/" + resourceFolder + "/" + nameBestANNFile + ".data";
        public static float thresholdForPickingALetter { get; private set; } = 0.95f;
        public static float thresholdForNotPickingALetter { get; private set; } = 0.95f;
        public static int sizeOfGeneration { get; private set; } = 100;
        public static float FitnessGoal { get; private set; } = 1f;
        public static int topLayers { get; private set; } = 10; // percent of sizeOfGeneration
        public static float chanceInPercentOfMutationForEachValue { get; private set; } = 0.5f;
        public static ANN.ReproductionType ReproductionType { get; private set; } = ANN.ReproductionType.MultipointCrossover;
        public static int numberOfCrossoverPoints { get; private set; } = 8;
        public static float rightAnswerWeightInFitness { get; private set; } = 0.7f;
        public static TrainANNReadLetters.EvaluationType EvaluationType = TrainANNReadLetters.EvaluationType.NewEvaluation;
        public static float MaxValueForBiasisAndWeights = 1.3f;

        private static void PrintSettingsInputInfo()
        {
            Console.WriteLine("Press 0 to exit settings");
            Console.WriteLine("Press 1 to set number to best ann name");
            Console.WriteLine("Press 2 to set thresholdForPickingALetter");
            Console.WriteLine("Press 3 to set thresholdForNotPickingALetter");
            Console.WriteLine("Press 4 to set sizeOfGeneration");
            Console.WriteLine("Press 5 to set FitnessGoal");
            Console.WriteLine("Press 6 to set topLayers");
            Console.WriteLine("Press 7 to set chanceInPercentOfMutationForEachValue");
            Console.WriteLine("Press 8 to set ReproductionType");
            Console.WriteLine("Press 9 to set numberOfCrossoverPoints");
            Console.WriteLine("Press 10 to set rightAnswerWeightInFitness");
            Console.WriteLine("Press 11 to set EvaluationType");
            Console.WriteLine("Press 12 to set MaxValueForBiasisAndWeights");
            Console.WriteLine("Then press enter to confirm choise");
        }

        public static void PrintSettings()
        {
            Console.WriteLine("Value 1 pathAndNameBestANN - " + pathAndNameBestANN);
            Console.WriteLine("Value 2 thresholdForPickingALetter - " + thresholdForPickingALetter);
            Console.WriteLine("Value 3 thresholdForNotPickingALetter - " + thresholdForNotPickingALetter);
            Console.WriteLine("Value 4 sizeOfGeneration - " + sizeOfGeneration);
            Console.WriteLine("Value 5 FitnessGoal - " + FitnessGoal);
            Console.WriteLine("Value 6 topLayers - " + topLayers);
            Console.WriteLine("Value 7 chanceInPercentOfMutationForEachValue - " + chanceInPercentOfMutationForEachValue);
            Console.WriteLine("Value 8 ReproductionType - " + ReproductionType);
            Console.WriteLine("Value 9 numberOfCrossoverPoints - " + numberOfCrossoverPoints);
            Console.WriteLine("Value 10 rightAnswerWeightInFitness - " + rightAnswerWeightInFitness);
            Console.WriteLine("Value 11 EvaluationType - " + EvaluationType);
            Console.WriteLine("Value 12 MaxValueForBiasisAndWeights - " + MaxValueForBiasisAndWeights);
        }

        public static void SetSettingsFromInput()
        {
            Console.WriteLine(" ");
            PrintSettingsInputInfo();

            bool continueEditing = true;

            while(continueEditing)
            {
                int input = Convert.ToInt32(Utility.ReadLineAsNumber());

                if (input == 0)
                {
                    continueEditing = false;
                    break;
                }
                else if (input == 1)
                {
                    Console.WriteLine("Enter int to best ann name");
                    updatePathAndNameBestANN(Convert.ToInt32(Utility.ReadLineAsNumber()));
                    Console.WriteLine("best ann name set to: " + pathAndNameBestANN);
                }
                else if (input == 2)
                {
                    Console.WriteLine("Enter float thresholdForPickingALetter");
                    thresholdForPickingALetter = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("thresholdForPickingALetter set to: " + thresholdForPickingALetter);
                }
                else if (input == 3)
                {
                    Console.WriteLine("Enter float thresholdForNotPickingALetter");
                    thresholdForNotPickingALetter = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("thresholdForNotPickingALetter set to: " + thresholdForNotPickingALetter);
                }
                else if (input == 4)
                {
                    Console.WriteLine("Enter int sizeOfGeneration");
                    sizeOfGeneration = Convert.ToInt32(Utility.ReadLineAsNumber());
                    Console.WriteLine("sizeOfGeneration set to: " + sizeOfGeneration);
                }
                else if (input == 5)
                {
                    Console.WriteLine("Enter float FitnessGoal");
                    FitnessGoal = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("FitnessGoal set to: " + FitnessGoal);
                }
                else if (input == 6)
                {
                    Console.WriteLine("Enter int topLayers (a persent of generation size)");
                    topLayers = Convert.ToInt32(Utility.ReadLineAsNumber());
                    Console.WriteLine("topLayers set to: " + topLayers);
                }
                else if (input == 7)
                {
                    Console.WriteLine("Enter float chanceInPercentOfMutationForEachValue");
                    chanceInPercentOfMutationForEachValue = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("chanceInPercentOfMutationForEachValue set to: " + chanceInPercentOfMutationForEachValue);
                }
                else if (input == 8)
                {
                    Console.WriteLine("Enter int ReproductionType");
                    ReproductionType = GetReproductionTypeFromInput();
                    Console.WriteLine("ReproductionType set to: " + ReproductionType);
                }
                else if (input == 9)
                {
                    Console.WriteLine("Enter int numberOfCrossoverPoints");
                    numberOfCrossoverPoints = Convert.ToInt32(Utility.ReadLineAsNumber());
                    Console.WriteLine("numberOfCrossoverPoints set to: " + numberOfCrossoverPoints);
                }
                else if (input == 10)
                {
                    Console.WriteLine("Enter float rightAnswerWeightInFitness");
                    rightAnswerWeightInFitness = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("rightAnswerWeightInFitness set to: " + rightAnswerWeightInFitness);
                }
                else if (input == 11)
                {
                    Console.WriteLine("Enter int EvaluationType");
                    EvaluationType = GetEvaluationTypeFromInput();
                    Console.WriteLine("EvaluationType set to: " + EvaluationType);
                }
                else if (input == 12)
                {
                    Console.WriteLine("Enter float MaxValueForBiasisAndWeights");
                    MaxValueForBiasisAndWeights = Convert.ToSingle(Utility.ReadLineAsNumber());
                    Console.WriteLine("MaxValueForBiasisAndWeights set to: " + MaxValueForBiasisAndWeights);
                }

                Console.WriteLine("Next");
            }
        }

        private static void updatePathAndNameBestANN(int number = 0)
        {
            if (number <= 0)
                return;
            else
            {
                // Only when executable is in same folder system as .sln
                Settings.pathAndNameBestANN = Utility.GetSolutionDirectoryInfo().ToString() + "/" + Settings.resourceFolder + "/" + Settings.nameBestANNFile + number.ToString() + ".data";
                //Settings.pathAndNameBestANN = AppDomain.CurrentDomain.BaseDirectory + "/" + Settings.resourceFolder + "/" + Settings.nameBestANNFile + number.ToString() + ".data";
            }
        }

        private static ANN.ReproductionType GetReproductionTypeFromInput()
        {
            int input = Convert.ToInt32(Utility.ReadLineAsNumber());
            ANN.ReproductionType inputAsType = (ANN.ReproductionType)(input > 2 ? 2 : input < 0 ? 0 : input);
            return inputAsType;
        }

        private static TrainANNReadLetters.EvaluationType GetEvaluationTypeFromInput()
        {
            int input = Convert.ToInt32(Utility.ReadLineAsNumber());
            TrainANNReadLetters.EvaluationType inputAsType = (TrainANNReadLetters.EvaluationType)(input > 1 ? 1 : input < 0 ? 0 : input);
            return inputAsType;
        }
    }
}

