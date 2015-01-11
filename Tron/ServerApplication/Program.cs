// Program.cs
// <copyright file="Program.cs"> This code is protected under the MIT License. </copyright>using System;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Networking;

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
        /// Gets or sets the string the user has inputted.
        /// </summary>
        public static string Input { get; set; }

        /// <summary>
        /// Gets or sets the local ip address.
        /// </summary>
        public static IPAddress LocalIP { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Any arguments/commands that the program is run/compiled with. </param>
        public static void Main(string[] args)
        {
            // Add a shutdown hook to shutdown the server
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { Server.Shutdown(); };

            // Initalize server
            Server = new Server();
            Server.Start();

            // Get local ip
            LocalIP = GetLocalIP();

            while (true)
            {
                // Draw user interface
                DisplayUI();

                // Get inputs without blocking
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo inpKey = Console.ReadKey(true);
                    
                    // Parse the input
                    if (inpKey.Key == ConsoleKey.Enter)
                    {
                        if (Input.ToLower() == "shutdown")
                        {
                            // Breaking the loop will shutdown the server
                            break;
                        }
                        else
                        {
                            // If not shutting down run the command
                            RunCommand(Input);
                        }

                        // Clear the input
                        Input = string.Empty;
                    }
                    else if (inpKey.Key == ConsoleKey.Backspace && Input.Length > 0)
                    {
                        // Remove a character from the end
                        Input = Input.Remove(Input.Length - 1);
                    }
                    else if (char.IsLetterOrDigit(inpKey.KeyChar) || char.IsSeparator(inpKey.KeyChar))
                    {
                        // Add the key to the input
                        Input += inpKey.KeyChar;
                    }
                }
            }

            // Exit application if loop has been broken
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Displays the user interface.
        /// </summary>
        public static void DisplayUI()
        {
            // Set window height to incude everything
            Console.WindowHeight = 28;

            // Clear the console and wait to avoid display issues
            Thread.Sleep(50);
            Console.Clear();

            Console.WriteLine("Tron server:");
            Console.WriteLine("Your local IP: {0}", LocalIP.ToString());
            Console.WriteLine("Game open on port: {0}\n", Server.Port);

            Console.WriteLine("Connected players:");
            for (int i = 0; i < 12; i++)
            {
                if (Server.ClientEPs[i] == null)
                {
                    Console.WriteLine("{0}: Not connected", i + 1);
                }
                else
                {
                    Console.WriteLine("{0}: {1}:{2}", i + 1, Server.ClientEPs[i].Address.ToString(), Server.ClientEPs[i].Port);
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine("To start the game (needs two players min) type 'start (wins needed)'\nWins needed must be a whole positive integer between 1 and 30");
            Console.WriteLine("To kick a player type 'kick (player id)'\nPlayer id is next to their ip address.");
            Console.WriteLine("To close the server type 'close'.\n");

            Console.WriteLine(">>> {0}_", Input);
        }

        /// <summary>
        /// Runs a command.
        /// </summary>
        /// <param name="input"> The command. </param>
        public static void RunCommand(string input)
        {
            // For trying to parse integers
            int parsedInt;

            try
            {
                if (Input.ToLower().Substring(0, 6) == "start " && int.TryParse(Input.Substring(6), out parsedInt) && parsedInt > 0 && parsedInt <= 30)
                {
                    // Start the game
                }
                else if (Input.ToLower().Substring(0, 5) == "kick " && int.TryParse(Input.Substring(5), out parsedInt) && parsedInt <= 12 && parsedInt >= 1)
                {
                    // Kick player
                    Server.RemovePlayer(parsedInt - 1, true);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Catch errors with message too small to run Input.Substring
            }
        }

        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        /// <returns> The local ip address. </returns>
        public static IPAddress GetLocalIP()
        {
            // Get host entry
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                // Return the local ip
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return null;
        }
    }
}
