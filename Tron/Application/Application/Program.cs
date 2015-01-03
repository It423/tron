// Program.cs
// <copyright file="Program.cs"> This code is protected under the MIT License. </copyright>
using System;
using Tron;

namespace Application
{
#if WINDOWS
    /// <summary>
    /// The main program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Any arguments/commands that the program is run/compiled with. </param>
        public static void Main(string[] args)
        {
            Console.Title = "Tron";
            Help();
            GetCommand();
        }

        /// <summary>
        /// Prints the help message into the console.
        /// </summary>
        public static void Help()
        {
            Console.WriteLine("Welcome to tron!\n");
            Console.WriteLine("How to play:");
            Console.WriteLine("The aim of the game is to drive your car without crashing.");
            Console.WriteLine("While driving you leave a trail of which will smash cars if driven into.");
            Console.WriteLine("Be the last car driving!\n");
            Console.WriteLine("Controls:");
            Console.WriteLine("W to go up");
            Console.WriteLine("A to go left");
            Console.WriteLine("S to go down");
            Console.WriteLine("D to go right");
            Console.WriteLine("Left shift to boost");
            Console.WriteLine("When playing local multiplayer player two will use:");
            Console.WriteLine("Arrow keys for directions");
            Console.WriteLine("Right control for boost.\n\n");
            Console.WriteLine("To play local multiplayer type 'local'");
            Console.WriteLine("To display this message again type 'help'");
            Console.WriteLine("To quit the game type 'quit'");
        }

        /// <summary>
        /// Gets and executes a command.
        /// </summary>
        public static void GetCommand()
        {
            while (true)
            {
                // Get a command
                Console.Write("Enter a command... ");
                string inp = Console.ReadLine();

                if (inp.ToLower() == "local")
                {
                    StartLocalGame();
                }
                else if (inp.ToLower() == "help")
                {
                    Help();
                }
                else if (inp.ToLower() == "quit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("That is not a valid command!");
                }
            }
        }

        /// <summary>
        /// Starts a game of local multiplayer tron.
        /// </summary>
        public static void StartLocalGame()
        {
            // Get amount of wins needed
            int pointsToWin;
            int? pointsToWinNullable = GetIntFromUser("What is the desired round win number to win the game?", 1, 30);
            if (pointsToWinNullable == null)
            {
                return;
            }
            else
            {
                pointsToWin = pointsToWinNullable.Value;
            }            

            // Setup the game data
            GameData.LocalTwoPlayer = true;
            GameData.Tron = new TronGame(2, pointsToWin);
            GameData.Tron.InitializeGame();

            // Start the game
            StartGame();
        }

        /// <summary>
        /// Gets an integer from the user.
        /// </summary>
        /// <returns> The integer. </returns>
        /// <remarks> If null is returned the user wishes to go back. </remarks>
        public static int? GetIntFromUser(string question, int min, int max)
        {
            Console.WriteLine("{0}\nEnter a whole integer above {1} and below {2} or b to go back:", question, min, max);
            while (true)
            {
                // Get the input
                string inp = Console.ReadLine();
                int parseRes;

                if (inp == "b")
                {
                    return null;
                }
                else if (int.TryParse(inp, out parseRes) && parseRes >= min && parseRes <= max)
                {
                    return int.Parse(inp);
                }
                else
                {
                    Console.WriteLine("Enter a whole integer above {0} and below {1} or b to go back:", min, max);
                }
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public static void StartGame()
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
#endif
}
