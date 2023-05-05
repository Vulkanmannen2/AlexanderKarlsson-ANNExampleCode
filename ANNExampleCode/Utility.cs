using System;

namespace ANN_test
{
	public static class Utility
	{
        public static string[] indexToLetter = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        // Make sure int is within range before indexing
        // Sett to zero if out of bounce
		public static int ReturnZeroIfIntIsOutOfIndex(int number, int itemsInList)
		{
			int index = number;

			if (number < 0 || number > itemsInList-1)
			{
				index = 0;
			}

			return index;
		}

        public static string intArrayAsString(int[] ints)
        {
            string answer = "";

            for (int i = 0; i < ints.Length; ++i)
            {
                answer += (ints[i].ToString() + " ");
            }

            return answer;
        }

        public static float RandomFloat(float min, float max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        public static int RandomInt(int min, int max)
        {
            System.Random random = new System.Random();
            int val = random.Next(min, max);
            return val;
        }

        public static bool RandomBool()
        {
            return Utility.RandomFloat(-1f, 1f) > 0;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        // Seems to cause some bug when used too much
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);
        }

        // Console.Readline returns "" when you press enter without input
        // This causes error for Convert.Single() and other
        // This function gives you a "0" when you press enter without input
        // Use when you read line as int of float and no input is same as 0
        public static string ReadLineAsNumber()
        {
            var input = Console.ReadLine();

            return input != null && input != "" ? input : "0";
        }

        public static DirectoryInfo GetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}

