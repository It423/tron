// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Networking
{
    /// <summary>
    /// A class that acts as a server for a networked game of tron.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The port used by the server.
        /// </summary>
        public static readonly int Port = 22528;

        public Server()
        {
            this.Host = new UdpClient(new IPEndPoint(IPAddress.Any, Server.Port));
            this.PlayerIPs = new IPEndPoint[12];
        }

        /// <summary>
        /// The host socket.
        /// </summary>
        public UdpClient Host { get; set; }

        /// <summary>
        /// The array of player ip addresses.
        /// </summary>
        public IPEndPoint[] PlayerIPs { get; set; }

        /// <summary>
        /// Listens for an input.
        /// </summary>
        public void Listen
        {

        }

        /// <summary>
        /// Sends a message to all clients.
        /// </summary>
        public void SendToAll()
        { 

        }

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        public void RemovePlayer
        {
            
        }
    }
}
