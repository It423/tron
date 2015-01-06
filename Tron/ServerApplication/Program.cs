// Program.cs
// <copyright file="Program.cs"> This code is protected under the MIT License. </copyright>using System;
using System;
using System.Net;
using System.Threading;
using System.Timers;
using Networking;
using Tron;

namespace ServerApplication
{    
    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the instance of the server.
        /// </summary>
        public static Server Server { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game has started.
        /// </summary>
        public static bool GameStarted { get; set; }

        /// <summary>
        /// Gets or sets the inputted string.
        /// </summary>
        public static string Input { get; set; }

        /// <summary>
        /// Gets or sets the timer for running the game at the correct speed.
        /// </summary>
        private static System.Timers.Timer Timer { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Any arguments/commands that the program is run/compiled with. </param>
        public static void Main(string[] args)
        {
            // Add a shutdown hook to shutdown the server
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { Server.ShutdownServer(); Timer.Stop(); };

            // Initailize the game and timer
            TronData.Tron = new TronGame(0, 1);
            for (int i = 0; i < 12; i++)
            {
                // Create cars
                TronData.Tron.Cars.Add(null);
            }
            
            Timer = new System.Timers.Timer(1000 / 50);
            Timer.AutoReset = true;
            Timer.Elapsed += HandleGameUpdate;
            Timer.Start();
            GameStarted = false;

            // Start the server
            Server = new Server();
            Server.StartServer();

            // Get key inputs without blocking
            while (true)
            {
                DrawConsole();
                
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo inpKey = Console.ReadKey(true);
                    if (inpKey.Key == ConsoleKey.Enter)
                    {
                        if (Input == "close")
                        {
                            break;
                        }
                        else
                        {
                            RunCommand();
                        }

                        Input = string.Empty;
                    }
                    else if (inpKey.Key == ConsoleKey.Backspace && Input.Length > 0)
                    {
                        Input = Input.Remove(Input.Length - 1);
                    }
                    else if (char.IsLetterOrDigit(inpKey.KeyChar) || char.IsSeparator(inpKey.KeyChar))
                    {
                        Input += inpKey.KeyChar;
                    }
                }
            }

            // Exit application now loop has been broken
            Environment.Exit(0);
        }

        /// <summary>
        /// Runs a command inputted by the user.
        /// </summary>
        public static void RunCommand()
        {
            // For trying to parse integers
            int parsedInt;

            try
            {
                if (Input.ToLower().Substring(0, 6) == "start " && int.TryParse(Input.Substring(6), out parsedInt) && parsedInt > 0 && parsedInt <= 30 && TronData.Tron.Players >= 2)
                {
                    GameStarted = true;
                    TronData.Tron.InitializeGame();
                    TronData.Tron.TimerChanged += Server.HandleTimerUpdate;
                }
                else if (Input.ToLower().Substring(0, 5) == "kick " && int.TryParse(Input.Substring(5), out parsedInt) && parsedInt <= 12 && parsedInt >= 1)
                {
                    // Remove player
                    Server.RemovePlayer(parsedInt - 1);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch errors with message too small to run Input.Substring
            }
        }

        /// <summary>
        /// Draws information on the console.
        /// </summary>
        public static void DrawConsole()
        {
            Console.Clear();
            Console.WriteLine("Tron server:");
            ////Console.WriteLine("Your local IP: {0}", );
            ////Console.WriteLine("Your public IP: {0}", );
            Console.WriteLine("Game open on port: {0}\n", Server.Port);

            Console.WriteLine("Connected players:");
            for (int i = 0; i < 12; i++)
            {
                if (Server.PlayerIPs[i] == null)
                {
                    Console.WriteLine("{0}: Not connected", i + 1);
                }
                else
                {
                    Console.WriteLine("{0}: {1}:{2}", i + 1, Server.PlayerIPs[i].Address.ToString(), Server.PlayerIPs[i].Port);
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine("To start the game (needs two players min) type 'start (wins needed)'\nWins needed must be a whole positive integer between 1 and 30");
            Console.WriteLine("To kick a player type 'kick (player id)'\nPlayer id is next to their ip address.");
            Console.WriteLine("To close the server type 'close'.\n");

            Console.WriteLine(">>> {0}", Input);
        }

        /// <summary>
        /// Updates the game.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        public static void HandleGameUpdate(object sender, ElapsedEventArgs e)
        {
            if (GameStarted)
            {
                TronData.Tron.Update();

                if (TronData.Tron.Players <= 1)
                { 
                    // Kick users and stop game if not enough players
                    for (int i = 0; i < 12; i++)
                    {
                        Server.RemovePlayer(i);
                    }

                    GameStarted = false;
                    TronData.Tron = new TronGame(0, 1);
                    TronData.Tron.TimerChanged -= Server.HandleTimerUpdate;
                }
            }
        }
    }
}
