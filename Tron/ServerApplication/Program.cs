// Program.cs
// <copyright file="Program.cs"> This code is protected under the MIT License. </copyright>using System;
using Networking;

namespace ServerApplication
{    
    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Any arguments/commands that the program is run/compiled with. </param>
        public static void Main(string[] args)
        {
            Server s = new Server();
            s.Listen();
        }
    }
}
