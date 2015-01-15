// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tron;
using Tron.CarData;
using Tron.EventArgs;

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

            // Initalize game
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
        /// Shuts down the server.
        /// </summary>
        public void Shutdown()
        {
            // Tell all clients they are disconnected
            for (int i = 0; i < 12; i++)
            {
                this.RemovePlayer(i, true);
            }

            // Stop listening
            this.ListenThread.Abort();
            this.Socket.Close();
            
            // Stop the game
            this.Tron = null;
        }

        /// <summary>
        /// Listens for incoming connections.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                // Initialize variables
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = new byte[] { 0, 0 };

                try
                {
                    // Get a packet
                    packet = this.Socket.Receive(ref remoteEP);
                }
                catch (SocketException)
                {
                    // Catch errors and stop receiving if one was found
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Stop receiving if the socket has been closed
                    break;
                }
                
                // Parse packet
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
                if (packet[0] == 255)
                {
                    // Client disconnected
                    this.RemovePlayer(this.ClientEPs.IndexOf(remoteEP), false);
                }
                else if (packet[0] < 4)
                {
                    // Change direction
                    this.Tron.Cars[this.ClientEPs.IndexOf(remoteEP)].ChangeDirection((Direction)packet[0]);
                }
                else if (packet[0] == 4)
                {
                    // Boost
                    this.Tron.Cars[this.ClientEPs.IndexOf(remoteEP)].Boost();
                }
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
                    
                // Send this to the connecting client and send next set if acknowledgement is received
                this.Socket.Send(activePlayers.ToArray(), activePlayers.Count, remoteEP);

                // Get acknowledgement
                if (this.GetAck(remoteEP))
                {
                    // Add the player if the player received the data
                    this.SendToAll(new byte[] { 0, 0 });
                    this.ClientEPs[nextIndex] = remoteEP;
                    this.Tron.AddPlayer();
                }
            }
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        public void SendToAll(byte[] packet)
        {
            // Get a list of connected clients
            List<IPEndPoint> eps = this.ClientEPs.Where(ep => ep != null).ToList();
            
            // Send the message to all clients
            eps.ForEach(ep => new Thread(() => this.Socket.Send(packet, packet.Length, ep)).Start());
        }

        /// <summary>
        /// Removes a player.
        /// </summary>
        /// <param name="id"> The id of the player. </param>
        /// <param name="kicked"> Whether the player was kicked. </param>
        public void RemovePlayer(int id, bool kicked)
        {
            // Make sure the player is real
            if (id < 12 && id >= 0 && this.ClientEPs[id] != null)
            {
                // Tell the player they are kicked
                if (kicked)
                {
                    this.Socket.Send(new byte[] { 255, 255 }, 2, this.ClientEPs[id]);
                }

                // Disconnect player
                this.ClientEPs[id] = null;
                this.Tron.RemovePlayer(id);

                // Tell other players to remove this player
                this.SendToAll(new byte[] { (byte)id, 255 });
            }
        }

        /// <summary>
        /// Sends the time left to each client when the timer changes.
        /// </summary>
        /// <param name="origin"> The origin of the event. </param>
        /// <param name="e"> The event arguments. </param>
        public void SendTimeLeft(object origin, TimerChangedEventArgs e)
        {
            // Apply to move and crash event handlers if time is 0
            if (e.TimeLeft == 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (this.Tron.Cars[i] != null)
                    {
                        // Unsubscribe to events
                        this.Tron.Cars[i].CarMoved -= this.SendCarMove;
                        this.Tron.Cars[i].CarCrashed -= this.SendCarMove;

                        // Resubscribe to events
                        this.Tron.Cars[i].CarMoved += this.SendCarMove;
                        this.Tron.Cars[i].CarCrashed += this.SendCarMove;
                    }
                }
            }

            // Get the time left and points to win
            byte[] packet = new byte[14];
            packet[0] = (byte)e.TimeLeft;
            packet[1] = (byte)this.Tron.PointsToWin;

            // Get the scores
            for (int i = 0; i < 12; i++)
            {
                if (this.Tron.Cars[i] == null)
                {
                    packet[i + 2] = 0;
                }
                else
                {
                    packet[i + 2] = (byte)this.Tron.Cars[i].Victories;
                }
            }

            // Send data
            this.SendToAll(packet);
        }

        /// <summary>
        /// Sends a car's move to each client.
        /// </summary>
        /// <param name="origin"> The origin of the event. </param>
        /// <param name="e"> The event arguments. </param>
        public void SendCarMove(object origin, CarMovedEventArgs e)
        {
            // Get the id and y position
            byte[] packet = new byte[5];
            packet[0] = (byte)e.ID;
            packet[3] = (byte)e.Y;

            // Get the x position
            if (e.X > 255)
            {
                packet[1] = 255;
                packet[2] = (byte)(e.X - 255);
            }
            else
            {
                packet[1] = (byte)e.X;
                packet[2] = 0;
            }

            // Get whether the car is dead or not
            if (this.Tron.Cars[e.ID].Alive)
            {
                packet[4] = (byte)(1 + this.Tron.Cars[e.ID].BoostsRemeaning);
            }
            else
            {
                packet[4] = 0;
            }

            // Send the packet
            this.SendToAll(packet);
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
