using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ANN_test
{
    // Uses evolutionery algorithm to evolve an ANN for reading letters from an image
    // 1. Creates a generation of ANNs
    // 2. Evaluates them - have them read images and say the letter
    // 3. Populates a new generation from the top performers and repeat from step 2 again
	public class TrainANNReadLetters
	{
		private List<ANN> Generation = new List<ANN>();
        private List<Tuple<string, float[]>> imagesAsArrays = new List<Tuple<string, float[]>>();

        private int numberOfGenerations = 1;

        public enum EvaluationType {NewEvaluation, OldEvaluation1};

        public TrainANNReadLetters()
		{
			Console.WriteLine("Load ANNs ");
            Console.Write(0 + " ");
            List<ANN> generation = new List<ANN>();
			for (int i = 0; i < Settings.sizeOfGeneration; ++i)
            {
				Utility.ClearCurrentConsoleLine();
                Console.Write(i + " ");
                generation.Add(new ANN(Settings.layers));
			}
			Generation = generation;
			Utility.ClearCurrentConsoleLine();

            Console.WriteLine("Load Images ");
            initImagesAsArray();

            Console.WriteLine("Init Done");
		}

        // Load best ANN (if exists)
        // Starts a loop that "trains" ANN (rather evolves)
        // Runs till fitness goal is reached or a key is pressed
		public ANN RunTraining()
		{
			Generation.Last().Load(Settings.pathAndNameBestANN);
            EvaluateBasedOnType(Generation.Last(), Settings.EvaluationType);
            float topFitness = 0;
            Settings.PrintSettings();

            while (topFitness < Settings.FitnessGoal && !Console.KeyAvailable)
			{
                int startFromAnn = Settings.ReproductionType == ANN.ReproductionType.Selection && numberOfGenerations > 1 ? Settings.sizeOfGeneration / 2 : 0;
				for (int j = startFromAnn; j < Settings.sizeOfGeneration && !Console.KeyAvailable; ++j)
				{
                    EvaluateBasedOnType(Generation[j], Settings.EvaluationType);
                }

                Generation.Sort((a, b) => a.fitness.CompareTo(b.fitness));
                Generation.Reverse();

				topFitness = Generation.First().fitness;
                Generation.First().Save(Settings.pathAndNameBestANN);

                Console.WriteLine("Gen: " + numberOfGenerations + " TopFit: " + topFitness + " ");

				if (topFitness < Settings.FitnessGoal)
				{
					PopulateNewGeneration();
					numberOfGenerations++;
				}
            }

            return Generation.First();
        }

        // There are different methods for evaluating ANNs
        private void EvaluateBasedOnType(ANN ann, EvaluationType type)
        {
            if (type == EvaluationType.NewEvaluation)
                EvaluateANN(ann);
            else if (type == EvaluationType.OldEvaluation1)
                EvaluateANNold1(ann);
        }

        // This is new Evaluation
        // It gives a fitness between 0 and 1
        //
        // If a letter is read correctly fitness is increased with 1
        // If not, a value between 0 and 1 is given based on how close it was to reading the letter (uses GetFitnessFromLetterAsString)
        // In the end fitness is devided with the number of images in the test data
        private void EvaluateANN(ANN ann)
		{
            float fitness = 0;
            for (int i = 0; i < imagesAsArrays.Count; ++i)
			{
				float[] result = ann.FeedForward(imagesAsArrays[i].Item2);
                int answer = ANNReadLetters.ReadAnswerFromANN(result);

                TestManager.TestFloats(result, -1f, 1f);
                TestManager.TestInt(answer, 0, 26);

                if (answer != 0 && Utility.indexToLetter[answer - 1] == imagesAsArrays[i].Item1)
                {
                    fitness += 1;
                }
                else if (answer == 0)
                {
                    fitness += GetFitnessFromLetterAsString(result, imagesAsArrays[i].Item1);
                }
            }

            TestManager.TestFloat(fitness, 0f, imagesAsArrays.Count);

			ann.fitness = fitness / imagesAsArrays.Count;

            TestManager.TestFloat(ann.fitness, 0f, 1f);
        }

        // Returns fitness between 0 and 1
        // The closer each parameter in result is to be right, the better fitness
		private float GetFitnessFromLetterAsString(float[] result, string letter)
        {
            TestManager.TestFloats(result, -1f, 1f);
            TestManager.TestString(letter, 1);

            float fitness = 0;

            for (int j = 0; j < result.Length; ++j)
            {
                float resultForLetter = Math.Abs(result[j]);
                TestManager.TestFloat(resultForLetter, 0f, 1f);

                if (Utility.indexToLetter[j] == letter)
                {
                    fitness += Settings.rightAnswerWeightInFitness * resultForLetter;
                }
                else
                {
                    float resultForLetterInvert = 1 - resultForLetter;
                    TestManager.TestFloat(resultForLetterInvert, 0f, 1f);
                    fitness += (1 - Settings.rightAnswerWeightInFitness) / result.Length * resultForLetterInvert;
                }
            }
            return fitness;
		}

        // Allows the new fitness function to be tested
        // Prints fitness for one letter (normaly fittnes is calculatet by reading a lot of letters and deviding with number of letters)
        public void TestFitnessFunction(float[] result, string letter)
        {
            Console.WriteLine(GetFitnessFromLetterAsString(result, letter));
        }

        // This is the old Evaluation (but still in use sometimes)
        // It gives a fitness between 0 and 1
        //
        // If a letter is read correctly fitness is increased with 1
        // If not, a value between 0 and 1 is given based on how close it was to reading the letter,
        //     the value is based on if each parameter in result is correct, not how close ech parameter is to being correct
        // In the end fitness is devided with the number of images in the test data
        private void EvaluateANNold1(ANN ann)
        {
            float fitness = 0;
            for (int i = 0; i < imagesAsArrays.Count; ++i)
            {
                float[] result = ann.FeedForward(imagesAsArrays[i].Item2);
                int answer = ANNReadLetters.ReadAnswerFromANN(result);

                TestManager.TestFloats(result, -1f, 1f);
                TestManager.TestInt(answer, 0, 26);

                if (answer != 0 && Utility.indexToLetter[answer - 1] == imagesAsArrays[i].Item1)
                {
                    fitness += 1;
                }
                else if (answer == 0)
                {
                    for (int j = 0; j < result.Length; ++j)
                    {
                        if (Utility.indexToLetter[j] == imagesAsArrays[i].Item1)
                        {
                            if (Math.Abs(result[j]) > Settings.thresholdForPickingALetter)
                            {
                                fitness += 0.5f;
                            }
                        }
                        else
                        {
                            if (Math.Abs(result[j]) < Settings.thresholdForNotPickingALetter)
                            {
                                fitness += 0.5f / result.Length;
                            }
                        }
                    }
                }
            }

            TestManager.TestFloat(fitness, 0f, imagesAsArrays.Count);

            ann.fitness = fitness / imagesAsArrays.Count;

            TestManager.TestFloat(ann.fitness, 0f, 1f);
        }

        // Replaces the old generation with a new, based on reproduction type (see ANN)
        private void PopulateNewGeneration()
		{
            // Selects the top half of the population copies it and uses it to write over the bottom half
            // The coppies are mutated
            // No reproduction in ths "Reproduction Type"
			if (Settings.ReproductionType != ANN.ReproductionType.Selection)
			{
				int smallHalfOfLIst = Settings.sizeOfGeneration / 2;
				int bigHalfOfList = Settings.sizeOfGeneration - smallHalfOfLIst;

				//handel the case of sizeOfGenration beeing odd
				for (int i = 0; i < smallHalfOfLIst; ++i)
				{
					ANN ann = Generation[i].Copy();
					ann.Mutate(Settings.chanceInPercentOfMutationForEachValue);
					Generation[i + bigHalfOfList] = ann;
				}
			}
			else
			{
				List<ANN> newGenereation = new List<ANN>();

                TestManager.TestInt(Settings.topLayers, 0, 100);

				int numberOfTopANNs = Settings.sizeOfGeneration * Settings.topLayers / 100;

				for (int i = 0; i < numberOfTopANNs; ++i)
				{
					newGenereation.Add(Generation[i]);
                    ANN ann = Generation[i].Copy();
                    ann.Mutate(Settings.chanceInPercentOfMutationForEachValue);
                    newGenereation.Add(ann);
                }

				while (newGenereation.Count < Settings.sizeOfGeneration)
				{
					for (int i = 0; i < numberOfTopANNs; ++i)
					{
						for (int j = 0; j < numberOfTopANNs; ++j)
						{
							if (i != j && newGenereation.Count < Settings.sizeOfGeneration)
							{
								ANN nextAnn = Generation[i].Reproduce(Settings.layers, Generation[j], Settings.ReproductionType, Settings.numberOfCrossoverPoints);
								nextAnn.Mutate(Settings.chanceInPercentOfMutationForEachValue);
								newGenereation.Add(nextAnn);
							}
						}
					}
				}
                Generation = newGenereation;
			}
		}

        // Reads the set of training images
        // the set of images is devided, so we train on some images and test on others
		private void initImagesAsArray()
		{
            for (int i = 0; i < 26; ++i)
            {
                for (int j = 0; j < Settings.numberOfImagesForTraining; ++j)
                {
                    string loadImageFrom = Settings.pathImages + Utility.indexToLetter[i] + "/" + j + ".data";
                    imagesAsArrays.Add(new Tuple<string, float[]>(Utility.indexToLetter[i], ImageLoader.LoadImageAsArray(loadImageFrom, Settings.imageWidth, Settings.imageHeight)));
				}
            }
        }
    }
}

