using System;

namespace ANN_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ANNReadLetters readLetters = new ANNReadLetters();
            TrainANNReadLetters trainer = new TrainANNReadLetters();

            PrintInputInfo();

            while (true)
            {
                ConsoleKey key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape)
                {
                    break;
                }
                else if (key == ConsoleKey.D1)
                {
                    Console.WriteLine(" - Start Training");
                    readLetters.ann = trainer.RunTraining();
                    PrintInputInfo();
                }
                else if(key == ConsoleKey.D2)
                {
                    Settings.SetSettingsFromInput();
                    PrintInputInfo();
                }
                else if(key == ConsoleKey.D3)
                {
                    TestManager.SetPrintSettingsFromInput(trainer);
                    PrintInputInfo();
                }
                else
                {
                    // If charakter A-Z is pressed
                    // Print name of image (random image of chosen character)
                    // And print what the ANN reads (or "cant't read")
                    char press = ((char)key);
                    Tuple<string, string> pressedLetter = readLetters.ReadLetter(press);
                    Console.WriteLine(" " + pressedLetter.Item1 + " - " + pressedLetter.Item2);
                }
            }
        }

        static void PrintInputInfo()
        {
            Console.WriteLine("Press 1 to Train");
            Console.WriteLine("Press 2 to enter training settings");
            Console.WriteLine("Press 3 to enter print manager");
            Console.WriteLine("Press any letter to test current best ANN");
        }
    }
}