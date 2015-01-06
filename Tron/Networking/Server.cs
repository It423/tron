// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tron;
using Tron.CarData;
using Tron.EventArguments;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        public Server()
        {
            this.Host = new UdpClient(new IPEndPoint(IPAddress.Any, Server.Port));
            this.PlayerIPs = new IPEndPoint[12];
        }

        /// <summary>
        /// Gets or sets the host socket.
        /// </summary>
        public UdpClient Host { get; set; }

        /// <summary>
        /// Gets or sets the array of player ip addresses.
        /// </summary>
        public IPEndPoint[] PlayerIPs { get; set; }

        /// <summary>
        /// Gets or sets the thread for listening to incoming transmissions.
        /// </summary>
        protected Thread ListenThread { get; set; }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void StartServer()
        {
            this.ListenThread = new Thread(() => this.Listen());
            this.ListenThread.Start();
        }

        /// <summary>
        /// Shuts down the server.
        /// </summary>
        public void ShutdownServer()
        {
            // Stop listening
            this.ListenThread.Abort();

            // Kick clients
            for (int i = 0; i < 12; i++)
            {
                if (this.PlayerIPs[i] != null)
                {
                    this.RemovePlayer(i);
                }
            }
        }

        /// <summary>
        /// Listens for an input.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                // Receive incomming transmission
                IPEndPoint transmissionEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = this.Host.Receive(ref transmissionEndPoint);

                // Check if it is a clients message or from a foreign machine
                bool client = this.PlayerIPs.Contains(transmissionEndPoint);

                if (client)
                {
                    this.ParseClientPacket(packet, this.PlayerIPs.ToList().IndexOf(transmissionEndPoint));
                }
                else
                {
                    this.ParseForeignPacket(packet, transmissionEndPoint);
                }
            }
        }

        /// <summary>
        /// Parse a message from a client.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        /// <param name="clientIndex"> The client's index. </param>
        public void ParseClientPacket(byte[] packet, int clientIndex)
        {
            if (packet[0] == 4)
            {
                // Boost player
                TronData.Tron.Cars[clientIndex].Boost();
            }
            else if (packet[0] < 3)
            {
                // Change direction of player
                TronData.Tron.Cars[clientIndex].ChangeDirection((Direction)packet[0]);
            }
            else
            {
                // Disconnect player
                this.RemovePlayer(clientIndex);
                Console.WriteLine("Disconnect client");
            }
        }

        /// <summary>
        /// Parse a message from a foreign machine.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        /// <param name="endPoint"> The end point of the message. </param>
        public void ParseForeignPacket(byte[] packet, IPEndPoint endPoint)
        {
            // Only parse valid packets
            if (packet.Length == 1 && (packet[0] == 0 || packet[0] == 1))
            {
                // Work out if the server is full
                bool full = TronData.Tron.Players > 12;

                // Work out how many spaces are availible and next id
                byte spaces = 0;
                byte nextId = 0;
                if (!full)
                {
                    spaces = (byte)(12 - TronData.Tron.Players);
                    for (byte i = 0; i < 12; i++)
                    {
                        if (this.PlayerIPs[i] != null)
                        {
                            nextId = i;
                        }
                    }
                }

                // Get the message to send
                byte[] returnArr = new byte[1];
                if (full)
                {
                    // Set to 255 if server is full
                    returnArr[0] = 255;
                }
                else if (packet[0] == 0)
                {
                    // Set to how many spaces there are if looking for servers
                    returnArr[0] = spaces;
                }
                else
                {
                    // Set to next player id and rounds to win
                    returnArr[0] = nextId;                    
                }


                // Send the message
                this.Host.Send(returnArr, 1, endPoint);

                // Store the end point if it is trying to connect
                if (!full && packet[0] == 1)
                {
                    // Tell all players that a new car has joined 
                    this.SendToAll(new byte[] { 255, 0 });

                    this.PlayerIPs[nextId] = endPoint;

                    // Tell the new client which players exist
                    List<byte> existingPlayers = new List<byte>();
                    for (int i = 0; i < 12; i++)
                    {
                        if (this.PlayerIPs[i] != null)
                        {
                            existingPlayers.Add((byte)i);
                        }
                    }

                    byte[] playersPacket = existingPlayers.ToArray();
                    Thread t = new Thread(() => this.Host.Send(playersPacket, playersPacket.Length, endPoint));
                    //t.Start();
                    Console.WriteLine("Received client connect");
                }
            }
        }

        /// <summary>
        /// Sends a message to all clients.
        /// </summary>
        /// <param name="packet"> The packet of data to send to all the clients. </param>
        public void SendToAll(byte[] packet)
        { 
            foreach (IPEndPoint ip in this.PlayerIPs)
            {
                // Only send to existing players
                if (ip != null)
                {
                    // Send in a new thread
                    Thread t = new Thread(() => this.Host.Send(packet, packet.Length, ip));
                    t.Start();
                }
            }
        }

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        /// <param name="index"> The index of the player to remove. </param>
        public void RemovePlayer(int index)
        {
            // Tell client they are kicked
            this.Host.Send(new byte[] { 255 }, 1, this.PlayerIPs[index]);

            // Remove their end point and remove from game
            TronData.Tron.RemovePlayer(index);
            this.PlayerIPs[index] = null;

            // Send all clients a message saying this player has left
            this.SendToAll(new byte[] { (byte)index, 255 });
        }

        /// <summary>
        /// Handles the update of a car.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        public void HandleCarUpdate(object sender, MovedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the crashing of a car.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        public void HandleCarCrash(object sender, CrashedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the update of the game timer.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        public void HandleTimerUpdate(object sender, TimerChangedEventArgs e)
        {

        }
    }
}
