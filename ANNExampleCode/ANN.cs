using System;

namespace ANN_test
{
    // Artificial Neural Network
    // Writen genaraly, can be trained to different tasks
	public class ANN
	{
		private int[] layers;
		private float[][] neurons;
		private float[][] biasis;
        private float[][][] weights;

		public float fitness = 0;

        public enum ReproductionType { Selection, UniformCrossover, MultipointCrossover }

        public ANN(int[] layers)
		{
            TestManager.TestInts(layers);

			this.layers = new int[layers.Length];
			for (int i = 0; i < layers.Length; ++i)
			{
				this.layers[i] = layers[i];
			}
			initNeurons();
			initBiasis();
			initWeights();
		}

        // Reproduction constructor
        // Used internaly by Reproduction funtion
        // Depending on ReproductionType it will use different ways of crossing two parent ANNs and make a child
        private ANN(int[] layers, ANN parent1, ANN parent2, ReproductionType reproductionType, int numberOfCrossingPointsForMultiCrossover)
        {
            TestManager.TestInts(layers);
            TestManager.TestInt(numberOfCrossingPointsForMultiCrossover);

            this.layers = new int[layers.Length];
            for (int i = 0; i < layers.Length; ++i)
            {
                this.layers[i] = layers[i];
            }
            initNeurons();

            if (reproductionType == ReproductionType.UniformCrossover)
            {
                initBiasisUniformCrossover(parent1, parent2);
                initWeightsUniformCrossover(parent1, parent2);
            }
            else if (reproductionType == ReproductionType.MultipointCrossover)
            {
                // Generate crossing point to use in making biasis and weights
                numberOfCrossingPointsForMultiCrossover = numberOfCrossingPointsForMultiCrossover < 1 ? 1 : numberOfCrossingPointsForMultiCrossover;
                List<int> crossingPoints = new List<int>();
                int maxValueForCrossingPoint = layers[1] - 2;

                TestManager.TestInt(maxValueForCrossingPoint);

                // Can add multiple crossingpoints of same value, this is okey
                // For each crossingpoint it will take at least one gene from the other parent
                for (int i = 0; i < numberOfCrossingPointsForMultiCrossover; ++i)
                {
                    crossingPoints.Add(Utility.RandomInt(0, maxValueForCrossingPoint));
                }
                // Add extra point, used in init function
                crossingPoints.Add(maxValueForCrossingPoint + 1);
                crossingPoints.Sort();

                initBiasisMultipointCrossover(parent1, parent2, crossingPoints, numberOfCrossingPointsForMultiCrossover);
                initWeightsMultipointCrossover(parent1, parent2, crossingPoints, numberOfCrossingPointsForMultiCrossover);
            }
        }

        // "copy" constructor
        // used internaly by copy function who makes deep copy of arrays
        private ANN(int[] layers, float[][] biasis, float[][][] weights)
		{
            TestManager.TestInts(layers);
            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);

            this.layers = new int[layers.Length];
            for (int i = 0; i < layers.Length; ++i)
            {
                this.layers[i] = layers[i];
            }
            initNeurons();
			this.biasis = biasis;
			this.weights = weights;
        }

		private void initNeurons()
		{
			List<float[]> neuronList = new List<float[]>();
			for (int i = 0; i < layers.Length; ++i)
			{
				neuronList.Add(new float[layers[i]]);
			}

			neurons = neuronList.ToArray();
		}

		private void initBiasis()
		{
			List<float[]> biasisList = new List<float[]>();
			for (int i = 0; i < layers.Length; ++i)
			{
				float[] bias = new float[layers[i]];
				for (int j = 0; j < layers[i]; ++j)
				{
					bias[j] = Utility.RandomFloat(0.5f, -0.5f);
				}
				biasisList.Add(bias);
			}

			biasis = biasisList.ToArray();

            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestBiasisMatchLayers(biasis, layers);
		}

        // Gives child biasis from either parent randomly
        private void initBiasisUniformCrossover(ANN parent1, ANN parent2)
        {
            TestManager.TestBiasisMatchLayers(parent1.biasis, layers);
            TestManager.TestBiasisMatchLayers(parent2.biasis, layers);

            List<float[]> biasisList = new List<float[]>();
            for (int i = 0; i < layers.Length; ++i)
            {
                float[] bias = new float[layers[i]];
                for (int j = 0; j < layers[i]; ++j)
                {
                    bias[j] = Utility.RandomBool() ? parent1.biasis[i][j] : parent2.biasis[i][j];
                }
                biasisList.Add(bias);
            }

            biasis = biasisList.ToArray();

            TestManager.TestFloats(biasis, -1f, 1f);
            TestManager.TestBiasisMatchLayers(biasis, layers);
            TestManager.TestBiasisUniformCrossover(biasis, parent1.biasis, parent2.biasis);
        }

        // Gives child biasis from different parent based on crossingpoints
        // The biasis are copied from parent one untill we reach a crossingpoin
        // Then we start copying from parent two, next crossingpoint we change again and so forth
        // The crossingpoints are randomly generated in constructor
        private void initBiasisMultipointCrossover(ANN parent1, ANN parent2, List<int> crossingPoints, int numberOfCrossingPoints)
        {
            TestManager.TestBiasisMatchLayers(parent1.biasis, layers);
            TestManager.TestBiasisMatchLayers(parent2.biasis, layers);
            TestManager.TestInts(crossingPoints.ToArray());
            TestManager.TestInt(numberOfCrossingPoints);
            TestManager.CompareInts(numberOfCrossingPoints, crossingPoints.Count - 1);

            List<float[]> biasisList = new List<float[]>();

            for (int i = 0; i < layers.Length; ++i)
            {
                float[] bias = new float[layers[i]];

                bool p1 = true;
                int nextPoint = 0;

                for (int j = 0; j < layers[i]; ++j)
                {
                    // If we pass a crossingpoint we change parent and use the next crossing point
                    // The last point in the list is extra. It's equal to the max index of the list
                    // This is to prevent from swaping parent every time after the last real crossingpoint
                    if (j > crossingPoints[nextPoint])
                    {
                        p1 = !p1;
                        nextPoint = nextPoint < numberOfCrossingPoints ? nextPoint + 1 : nextPoint;
                    }

                    bias[j] = p1 ? parent1.biasis[i][j] : parent2.biasis[i][j];
                }
                biasisList.Add(bias);
            }

            biasis = biasisList.ToArray();

            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestBiasisMatchLayers(biasis, layers);
            TestManager.TestBiasisMultipointCrossover(biasis, parent1.biasis, parent2.biasis, crossingPoints.ToArray());
        }

        // Add weights to all layers except input layer
        private void initWeights()
		{
			List<float[][]> weightList = new List<float[][]>();
			for (int i = 1; i < layers.Length; ++i)
			{
				List<float[]> layerWeightList = new List<float[]>();
				int neuronsInPreviusLayer = layers[i - 1];
				for (int j = 0; j < neurons[i].Length; ++j)
				{
					float[] neuronWeights = new float[neuronsInPreviusLayer];
					for (int k = 0; k < neuronsInPreviusLayer; ++k)
					{
						neuronWeights[k] = Utility.RandomFloat(0.5f, -0.5f);
					}
					layerWeightList.Add(neuronWeights);
				}
				weightList.Add(layerWeightList.ToArray());
			}
			weights = weightList.ToArray();

            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestWeightsMatchLayers(weights, layers);
        }

        // Gives child weight from either parent randomly, skip input layer
        private void initWeightsUniformCrossover(ANN parent1, ANN parent2)
        {
            TestManager.TestWeightsMatchLayers(parent1.weights, layers);
            TestManager.TestWeightsMatchLayers(parent2.weights, layers);

            List<float[][]> weightList = new List<float[][]>();
            for (int i = 1; i < layers.Length; ++i)
            {
                List<float[]> layerWeightList = new List<float[]>();
                int neuronsInPreviusLayer = layers[i - 1];
                for (int j = 0; j < neurons[i].Length; ++j)
                {
                    float[] neuronWeights = new float[neuronsInPreviusLayer];
                    for (int k = 0; k < neuronsInPreviusLayer; ++k)
                    {
                        neuronWeights[k] = Utility.RandomBool() ? parent1.weights[i - 1][j][k] : parent2.weights[i - 1][j][k];
                    }
                    layerWeightList.Add(neuronWeights);
                }
                weightList.Add(layerWeightList.ToArray());
            }
            weights = weightList.ToArray();

            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestWeightsMatchLayers(weights, layers);
            TestManager.TestWeightsUniformCrossover(weights, parent1.weights, parent2.weights);
        }

        // Gives child weights from different parent based on crossingpoints, skip input layer
        // The weights are copied from parent one untill we reach a crossingpoin
        // Then we start copying from parent two, next crossingpoint we change again and so forth
        // The crossingpoints are randomly generated in constructor
        private void initWeightsMultipointCrossover(ANN parent1, ANN parent2, List<int> crossingPoints, int numberOfCrossingPoints)
        {
            TestManager.TestWeightsMatchLayers(parent1.weights, layers);
            TestManager.TestWeightsMatchLayers(parent2.weights, layers);
            TestManager.TestInts(crossingPoints.ToArray());
            TestManager.TestInt(numberOfCrossingPoints);
            TestManager.CompareInts(numberOfCrossingPoints, crossingPoints.Count - 1);

            List<float[][]> weightList = new List<float[][]>();

            for (int i = 1; i < layers.Length; ++i)
            {
                List<float[]> layerWeightList = new List<float[]>();
                int neuronsInPreviusLayer = layers[i - 1];

                bool p1 = true;
                int nextPoint = 0;

                for (int j = 0; j < neurons[i].Length; ++j)
                {
                    // If we pass a crossingpoint we change parent and use the next crossing point
                    // The last point in the list is extra. It's equal to the max index of the list
                    // This is to prevent from swaping parent every time after the last real crossingpoint
                    if (j > crossingPoints[nextPoint])
                    {
                        p1 = !p1;
                        nextPoint = nextPoint < numberOfCrossingPoints ? nextPoint + 1 : nextPoint;
                    }

                    float[] neuronWeights = new float[neuronsInPreviusLayer];
                    for (int k = 0; k < neuronsInPreviusLayer; ++k)
                    {
                        // All weights conected to a neuron is from the same parent
                        neuronWeights[k] = p1 ? parent1.weights[i - 1][j][k] : parent2.weights[i - 1][j][k];
                    }
                    layerWeightList.Add(neuronWeights);
                }
                weightList.Add(layerWeightList.ToArray());
            }
            weights = weightList.ToArray();

            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestWeightsMatchLayers(weights, layers);
            TestManager.TestWeightsMultipointCrossover(weights, parent1.weights, parent2.weights, crossingPoints.ToArray());
        }

        public float Activate (float value)
		{
            float answer = (float)Math.Tanh(value);
            TestManager.TestFloat(answer, -1f, 1f);
            return answer;
		}

        // 1. Sets input to input layer
        // 2. Next layer calculates input from each node in previus layer, value1(neuron) * weight1 + value.. * weight..
        // 3. Runs result + biasis through Tanh function
        // 4. Repeat from step 2 for each layer, when last layer reached return output values (neurons)
		public float[] FeedForward(float[] inputs)
		{
			for (int i = 0; i < inputs.Length; ++i)
			{
				neurons[0][i] = inputs[i];
			}
			for (int i = 1; i < layers.Length; ++i)
			{
				for (int j = 0; j < neurons[i].Length; ++j)
				{
					float value = 0;
					for (int k = 0; k < neurons[i - 1].Length; ++k)
					{
                        // Weight is indexed i - 1 becouse there is no weights in the input layer
						value += weights[i - 1][j][k] * neurons[i - 1][k];
					}
					neurons[i][j] = Activate(value + biasis[i][j]);
				}
			}
			return neurons[neurons.Length - 1];
		}

		public int CompareTo(ANN other)
		{
			if (other == null)
			{ return 1; }
			else if (fitness > other.fitness)
			{ return 1; }
			else if (fitness < other.fitness)
			{ return -1; }
			else
			{ return 0; }
		}
       
        // Read from data file
        // Expects the input to be in right format
        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Couldn't load file at path: " + path);
                return;
            }

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fs);

            for (int i = 0; i < biasis.Length; ++i)
            {
                for (int j = 0; j < biasis[i].Length; ++j)
                {
                    biasis[i][j] = Utility.Clamp(reader.ReadSingle(), -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
                }
            }

            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);

            for (int i = 0; i < weights.Length; ++i)
            {
                for (int j = 0; j < weights[i].Length; ++j)
                {
                    for (int k = 0; k < weights[i][j].Length; ++k)
                    {
                        weights[i][j][k] = Utility.Clamp(reader.ReadSingle(), -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
                    }
                }
            }

            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);

            fs.Close();
        }

        // save to data file
        public void Save(String path)
        {
            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);

            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            for (int i = 0; i < biasis.Length; ++i)
            {
                for (int j = 0; j < biasis[i].Length; ++j)
                {
                    writer.Write(biasis[i][j]);
                }
            }
            for (int i = 0; i < weights.Length; ++i)
            {
                for (int j = 0; j < weights[i].Length; ++j)
                {
                    for (int k = 0; k < weights[i][j].Length; ++k)
                    {
                        writer.Write(weights[i][j][k]);
                    }
                }
            }

            fs.Close();
        }

        // Each biasis and weight has the same chance of being altered 
        public void Mutate(float chanceInPercentOfMutationForEachValue, float rangeOfMutation = 0.1f)
		{
            TestManager.TestFloat(chanceInPercentOfMutationForEachValue, 0f, 100f);
            TestManager.TestFloat(rangeOfMutation, 0f, 1f);
            rangeOfMutation = Utility.Clamp(rangeOfMutation, 0f, 1f);
			chanceInPercentOfMutationForEachValue = Utility.Clamp(chanceInPercentOfMutationForEachValue, 0f, 100f);

            Random rand = new Random();

            for (int i = 0; i < biasis.Length; ++i)
            {
                for (int j = 0; j < biasis[i].Length; ++j)
                {
					if (Utility.RandomFloat(0, 100) < chanceInPercentOfMutationForEachValue)
					{
						biasis[i][j] = Utility.Clamp(biasis[i][j] + Utility.RandomFloat(rangeOfMutation, -rangeOfMutation), -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
                    }
                }
            }

            for (int i = 0; i < weights.Length; ++i)
            {
                for (int j = 0; j < weights[i].Length; ++j)
                {
                    for (int k = 0; k < weights[i][j].Length; ++k)
                    {
                        if (rand.Next(0, 100) < chanceInPercentOfMutationForEachValue)
                        {
							weights[i][j][k] = Utility.Clamp(weights[i][j][k] + Utility.RandomFloat(rangeOfMutation, -rangeOfMutation), -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
                        }
                    }
                }
            }

            TestManager.TestFloats(biasis, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
            TestManager.TestFloats(weights, -Settings.MaxValueForBiasisAndWeights, Settings.MaxValueForBiasisAndWeights);
        }

        public ANN Reproduce(int [] layers, ANN parent2, ReproductionType reproductionType, int numberOfCrossovers = 2)
        {
            return new ANN(layers, this, parent2, reproductionType, numberOfCrossovers);
        }

        // Makes deep copies of biasis and weights, then runns the "copy" constructor
		public ANN Copy()
		{
            List<float[]> biasisList = new List<float[]>();
            for (int i = 0; i < layers.Length; ++i)
            {
                float[] bias = new float[layers[i]];
                for (int j = 0; j < layers[i]; ++j)
                {
                    bias[j] = biasis[i][j];
                }
                biasisList.Add(bias);
            }

            List<float[][]> weightList = new List<float[][]>();
            for (int i = 1; i < layers.Length; ++i)
            {
                List<float[]> layerWeightList = new List<float[]>();
                int neuronsInPreviusLayer = layers[i - 1];
                for (int j = 0; j < neurons[i].Length; ++j)
                {
                    float[] neuronWeights = new float[neuronsInPreviusLayer];
                    for (int k = 0; k < neuronsInPreviusLayer; ++k)
                    {
                        neuronWeights[k] = weights[i-1][j][k];
                    }
                    layerWeightList.Add(neuronWeights);
                }
                weightList.Add(layerWeightList.ToArray());
            }

            float[][] newBiaasis = biasisList.ToArray();
            float[][][] newWeights = weightList.ToArray();

            TestManager.CompareFloats(newBiaasis, biasis);
            TestManager.CompareFloats(newWeights, weights);

            return new ANN(this.layers, newBiaasis, newWeights);
		}

        public bool Compare(ANN other)
        {
            bool biasisEqual = TestManager.CompareFloats(biasis, other.biasis);
            bool weightsEqual = TestManager.CompareFloats(weights, other.weights);

            return biasisEqual && weightsEqual;
        }
    }
}

