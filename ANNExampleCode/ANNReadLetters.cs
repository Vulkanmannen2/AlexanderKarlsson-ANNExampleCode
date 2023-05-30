using System;

namespace ANN_test
{
    public class ANNReadLetters
    {
        public ANN ann { private get; set; }
        private List<KeyValuePair<string, float[]>> imagesAsArrays = new List<KeyValuePair<string, float[]>>();

        public ANNReadLetters()
        {
            ann = new ANN(Settings.layers);
            ann.Load(Settings.pathAndNameBestANN);
            initImagesAsArray();
        }

        // Reads letter based on char, used when key is pressed
        // If no character is pressed, return Wrong Input
        public Tuple<string, string> ReadLetter(char input)
        {
            string answer = "Wrong input";
            string imageName = "--";

            int randomImage = Utility.RandomInt(Settings.numberOfImagesForTraining, 195);

            if (imagesAsArrays.Where(kvp => kvp.Key == input.ToString()+randomImage.ToString()).Count() > 0)
            {
                imageName = input.ToString() + randomImage.ToString();
                answer = ReadLetter(imagesAsArrays.First(c => c.Key == imageName).Value);
            }
            return new Tuple<string,string>(imageName, answer);
        }

        // Reads letter based on image as float array
        // Calls the FeedForward in ANN
        public string ReadLetter(float[] imageAsArray)
        {
            int answerAsInt = ReadAnswerFromANN(ann.FeedForward(imageAsArray));

            answerAsInt = Utility.ReturnZeroIfIntIsOutOfIndex(answerAsInt, Utility.indexToLetter.Length);

            return answerAsInt == 0 ? "Can't say" : Utility.indexToLetter[answerAsInt-1];
        }

        // Interpatates the answer from ANN as a Letter
        // Answer 0 == can't say
        // Answer 1 == A
        // Answer 2 == B
        // And so on
        public static int ReadAnswerFromANN(float[] result)
        {
            TestManager.PrintResultWhenReadingLetters(result);

            int numberOfOutputsOverThershold = 0;
            int answer = 0;

            for (int i = 0; i < Settings.numberOfOutputNodes; ++i)
            {
                if (Math.Abs(result[i]) > Settings.thresholdForPickingALetter)
                {
                    numberOfOutputsOverThershold++;
                    answer = i + 1;
                }
                else if (Math.Abs(result[i]) > Settings.thresholdForNotPickingALetter)
                {
                    numberOfOutputsOverThershold++;
                }

                if (numberOfOutputsOverThershold > 1)
                {
                    return 0;
                }
            }

            return answer;
        }

        // Loads the images used for testing
        // The test images are devided so some are used for training and some for testing
        private void initImagesAsArray()
        {
            for (int i = 0; i < 26; ++i)
            {
                for (int j = Settings.numberOfImagesForTraining; j < 195; ++j)
                {
                    string loadImageFrom = Settings.pathImages + Utility.indexToLetter[i] + "/" + j + ".data";
                    imagesAsArrays.Add(new KeyValuePair<string, float[]>(Utility.indexToLetter[i] + j.ToString(), ImageLoader.LoadImageAsArray(loadImageFrom, Settings.imageWidth, Settings.imageHeight)));
                }
            }
        }
    }
}

