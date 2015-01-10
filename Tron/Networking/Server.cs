using System;
// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tron;

namespace Networking
{
    /// <summary>
    /// A class that acts as a server for a networked game of tron.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The port number for the server.
        /// </summary>
        public static readonly int Port = 27765;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        public Server()
        {
            // Initialize socket
            this.Socket = new UdpClient(new IPEndPoint(IPAddress.Any, Server.Port));

            // Initialize client end points
            this.ClientEPs = new List<IPEndPoint>(12);
            for (int i = 0; i < 12; i++)
            {
                this.ClientEPs.Add(null);
            }

            this.Tron = new TronGame(0, 1);
        }

        /// <summary>
        /// Gets or sets the udp socket.
        /// </summary>
        public UdpClient Socket { get; set; }

        /// <summary>
        /// Gets or sets the list of client end points.
        /// </summary>
        public List<IPEndPoint> ClientEPs { get; set; }

        /// <summary>
        /// Gets or sets the listening thread.
        /// </summary>
        public Thread ListenThread { get; set; }

        /// <summary>
        /// Gets or sets the server's copy of the game.
        /// </summary>
        public TronGame Tron { get; set; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            this.ListenThread = new Thread(() => this.Listen());
            this.ListenThread.Start();
        }

        /// <summary>
        /// Listens for incoming connections.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                // Get a packet
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = this.Socket.Receive(ref remoteEP);

                if (packet.Length == 1)
                {
                    this.ReplyToLanDetect(remoteEP);
                }
            }
        }

        /// <summary>
        /// Replys to a lan auto-detect message.
        /// </summary>
        public void ReplyToLanDetect(IPEndPoint remoteEP)
        {
            // Get how many spaces left
            int spaces = 12 - this.Tron.Players;

            this.Socket.Send(new byte[] { (byte)spaces }, 1, remoteEP);
            Console.WriteLine("Responded to: {0}:{1}", remoteEP.Address.ToString(), remoteEP.Port);
        }
    }
}
