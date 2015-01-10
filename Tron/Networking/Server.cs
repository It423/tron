// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Linq;
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
        /// Gets or sets the server's copy of the game.
        /// </summary>
        public TronGame Tron { get; set; }

        /// <summary>
        /// Gets or sets the listening thread.
        /// </summary>
        protected Thread ListenThread { get; set; }

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

                this.RespondToPacket(packet, remoteEP);
            }
        }

        /// <summary>
        /// Responds to an incoming packet.
        /// </summary>
        /// <param name="packet"> The packet. </param>
        /// <param name="remoteEP"> The remote end point. </param>
        public void RespondToPacket(byte[] packet, IPEndPoint remoteEP)
        {
            if (!this.ClientEPs.Contains(remoteEP))
            {
                // Forign packets
                if (packet.Length == 1)
                {
                    this.ReplyToConnectionAttempt(remoteEP);
                }
                else
                {
                    this.ReplyToLanDetect(remoteEP);
                }
            }
            else
            {
                // Packet from client
            }
        }

        /// <summary>
        /// Replies to a lan auto-detect message.
        /// </summary>
        /// <param name="remoteEP"> The remote end point. </param>
        public void ReplyToLanDetect(IPEndPoint remoteEP)
        {
            // Get how many spaces left
            int spaces = 12 - this.Tron.Players;

            this.Socket.Send(new byte[] { (byte)spaces }, 1, remoteEP);
        }

        /// <summary>
        /// Replies to a connection attempt.
        /// </summary>
        /// <param name="remoteEP"> The remote end point. </param>
        public void ReplyToConnectionAttempt(IPEndPoint remoteEP)
        {
            // Get new player index
            int nextIndex = this.ClientEPs.IndexOf(null);
            if (nextIndex == -1)
            {
                nextIndex = 12;
            }
            
            // Send the client the index
            this.Socket.Send(new byte[] { (byte)nextIndex }, 1, remoteEP);

            // Only send rest of data if there is space in the game and the client responded
            if (nextIndex < 12 && this.GetAck(remoteEP))
            {
                // Get a list of active player indexes with the rounds to win in front
                List<byte> activePlayers = this.ClientEPs
                    .Where(ep => ep != null)                            // Get players that arn't null
                    .Select(ep => (byte)this.ClientEPs.IndexOf(ep))     // Turn them into their indexes
                    .ToList();                                          // Turn to list
                activePlayers.Insert(0, (byte)this.Tron.PointsToWin);   // Add the points to win
                    
                // Send this to the connecting client and send next set if acknowledgement is received
                this.Socket.Send(activePlayers.ToArray(), activePlayers.Count, remoteEP);

                // Get acknowledgement
                if (this.GetAck(remoteEP))
                {
                    // Add the player if the player received the data
                    this.SendToAll(new byte[] { 0, 0 });
                    this.ClientEPs[nextIndex] = remoteEP;
                }
            }
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        public void SendToAll(byte[] packet)
        {
            // Get a list of connected clients
            List<IPEndPoint> eps = this.ClientEPs.Where(ep => ep != null).ToList();
            
            // Send the message to all clients
            eps.ForEach(ep => new Thread(() => this.Socket.Send(packet, packet.Length, ep)).Start());
        }

        /// <summary>
        /// Gets an acknowledgement from an end point.
        /// </summary>
        /// <param name="remoteEP"> The remote end point. </param>
        /// <returns> Whether the end point acknowledged. </returns>
        private bool GetAck(IPEndPoint remoteEP)
        {
            // Set recieve timeout
            this.Socket.Client.ReceiveTimeout = 5000;

            // Get messages until acknowledgement is received or method timed out
            while (true)
            {
                try
                {
                    // Get incomming message
                    IPEndPoint incommingEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] packet = this.Socket.Receive(ref incommingEP);

                    // Make sure its from the client, otherwise parse the packet
                    if (incommingEP.Equals(remoteEP))
                    {
                        this.Socket.Client.ReceiveTimeout = -1;
                        return true;
                    }
                    else
                    {
                        // Parse packet if it was from another player
                        this.RespondToPacket(packet, incommingEP);
                    }
                }
                catch (SocketException)
                {
                    // Timeout error, say the client did not respond and break the loop
                    this.Socket.Client.ReceiveTimeout = -1;
                    return false;
                }
            }
        }
    }
}
