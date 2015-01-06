// Program.cs
// <copyright file="Program.cs"> This code is protected under the MIT License. </copyright>using System;
using Networking;
using Tron;

namespace ServerApplication
{    
    /// <summary>
    /// The main program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Any arguments/commands that the program is run/compiled with. </param>
        static void Main(string[] args)
        {
            TronData.Tron = new TronGame(0, 5);
            Server s = new Server();
            s.Listen();
        }
    }
}
