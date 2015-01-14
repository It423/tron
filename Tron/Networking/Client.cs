// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tron;
using Tron.CarData;

namespace Networking
{
    /// <summary>
    /// A class that acts as a client for a networked game of tron.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        public Client()
        {
            this.Socket = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            this.HostEP = new IPEndPoint(IPAddress.Any, 0);
            this.Tron = new TronGame(0, 1);
            this.Tron.Timer.Stop();
            this.OnlineID = 0;
            this.Connected = false;
        }

        /// <summary>
        /// Gets or sets the udp socket.
        /// </summary>
        public UdpClient Socket { get; set; }

        /// <summary>
        /// Gets or sets the host end point.
        /// </summary>
        public IPEndPoint HostEP { get; set; }

        /// <summary>
        /// Gets or sets the thread for listening to incomming transmissions.
        /// </summary>
        public Thread ListenThread { get; set; }

        /// <summary>
        /// Gets or sets the client game of tron.
        /// </summary>
        public TronGame Tron { get; set; }

        /// <summary>
        /// Gets or sets the client's online id.
        /// </summary>
        public int OnlineID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is connected to a server.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Searches the local area network for servers.
        /// </summary>
        /// <returns> Returns the list of local servers. </returns>
        public IPAddress[] SearchForLanServers()
        {
            // Send a message to the local area network
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), Server.Port);
            this.Socket.Send(new byte[] { 0, 0 }, 2, broadcast);

            // Set timeout and list of ip addresses
            this.Socket.Client.ReceiveTimeout = 5000;
            List<IPAddress> ips = new List<IPAddress>();

            while (true)
            {
                try
                {
                    // Receive message
                    IPEndPoint newIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] packet = this.Socket.Receive(ref newIP);

                    // Only add the ip if the server has space
                    if (packet.Length == 1 && packet[0] >= 1 && packet[0] <= 12)
                    {
                        ips.Add(newIP.Address);
                    }
                }
                catch (SocketException)
                {
                    // No more messages to receive, break loop
                    break;
                }
            }

            // Reset timeout and return ips
            this.Socket.Client.ReceiveTimeout = -1;
            return ips.ToArray();
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        /// <param name="hostIp"> The servers ip address. </param>
        /// <returns> Whether the connection worked. </returns>
        /// <remarks> If null was returned the host was not found. False means the server is full. </remarks>
        public bool? Connect(IPAddress hostIp)
        {
            // Initialize the socket (incase it has been previously disposed
            this.Socket = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            // Tell the server we are trying to connect
            IPEndPoint hostEp = new IPEndPoint(hostIp, Server.Port);
            this.Socket.Send(new byte[] { 0 }, 1, hostEp);

            // Get the new id number of the player
            byte[] newId = new byte[] { 12 };
            newId = this.GetConnectionMessageFromServer(hostEp);

            // Check if the server said it is full
            if (newId == null)
            {
                return null;
            }
            else if (newId[0] >= 12)
            {
                return false;
            }
            else
            {
                // Store id
                this.OnlineID = newId[0];

                // Tell the server we got the message
                this.Socket.Send(new byte[] { 1 }, 1, hostEp);

                // Get the list of active players
                byte[] activePlayers = this.GetConnectionMessageFromServer(hostEp);

                if (activePlayers == null)
                {
                    // Exit with null if the receive failed
                    return null;
                }
                else
                {
                    // Return the acknowledgement
                    this.Socket.Send(new byte[] { 1 }, 1, hostEp);

                    // Initialize the game
                    this.Tron = new TronGame(0, 1);
                    this.Tron.Timer.Stop();
                    this.Tron.Action = string.Empty;

                    // Initialize the players
                    this.Tron.Players = activePlayers.Length + 1;
                    for (byte i = 0; i < 12; i++)
                    {
                        if (activePlayers.Contains(i) || i == newId[0])
                        {
                            this.Tron.Cars[i] = new Car(i, SpawnLists.XPositions[i], SpawnLists.YPositions[i], SpawnLists.Directions[i], (CellValues)i + 1);
                        }
                        else
                        {
                            this.Tron.Cars[i] = null;
                        }
                    }

                    // Finish connection
                    this.HostEP = hostEp;
                    this.Connected = true;
                    this.ListenThread = new Thread(() => this.Listen());
                    this.ListenThread.Start();
                    return true;
                }
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <param name="kicked"> Whether the client was kicked or not. </param>
        public void Disconnect(bool kicked)
        {
            // Tell the server we are disconnecting
            if (!kicked)
            {
                this.Socket.Send(new byte[] { 255 }, 1, this.HostEP);
            }

            // Disconnect
            this.HostEP = new IPEndPoint(IPAddress.Any, 0);
            this.Socket.Close();
            this.Tron = new TronGame(0, 1);
            this.OnlineID = 0;
            this.Connected = false;
        }

        /// <summary>
        /// Listens for incomming transmissions.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                // Initalize variables
                IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = new byte[] { 255, 255 };

                try
                {
                    // Get message
                    packet = this.Socket.Receive(ref remoteEp);
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
                this.ParsePacket(packet, remoteEp);
            }
        }

        /// <summary>
        /// Parses a packet sent to the client.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        /// <param name="remoteEP"> The sender's ip end point. </param>
        public void ParsePacket(byte[] packet, IPEndPoint remoteEp)
        {
            // Make sure its from the server
            if (remoteEp.Equals(this.HostEP))
            {
                // Check if its a kick, add or remove message
                if (packet.Length == 2)
                {
                    if (packet[0] == 255 && packet[1] == 255)
                    {
                        // Kicked
                        this.Disconnect(true);
                    }
                    else if (packet[0] == 0 && packet[1] == 0)
                    {
                        // Add car
                        this.Tron.AddPlayer();
                    }
                    else if (packet[0] < 12 && packet[1] == 255)
                    {
                        // Remove car
                        this.Tron.RemovePlayer(packet[0]);
                    }
                }
                else if (packet.Length == 14)
                {
                    // New timer and scores message
                    // Parse scores
                    for (int i = 0; i < 12; i++)
                    {
                        if (this.Tron.Cars[i] != null)
                        {
                            this.Tron.Cars[i].Victories = packet[i + 2];
                        }
                    }

                    // Parse time and rounds to win
                    this.Tron.DecTimer();
                    this.Tron.Timer.Stop();
                    this.Tron.TimeTillAction = packet[0];
                    this.Tron.ResetGame(this.Tron.GameWon);
                    this.Tron.PointsToWin = packet[1];
                }
            }
        }

        /// <summary>
        /// Sends a direction change input to the server.
        /// </summary>
        /// <param name="d"> The new direction. </param>
        public void SendDirection(Direction d)
        {
            this.Socket.Send(new byte[] { (byte)d }, 1, this.HostEP);
        }

        /// <summary>
        /// Sends a boost input to the server.
        /// </summary>
        public void SendBoost()
        {
            this.Socket.Send(new byte[] { 4 }, 1, this.HostEP);
        }

        /// <summary>
        /// Gets a message from the server during the connection sequence.
        /// </summary>
        /// <param name="hostEp"> The server's end point. </param>
        /// <returns> The message from the server. </returns>
        /// <remarks> If it returns null the host could not be reached. </remarks>
        private byte[] GetConnectionMessageFromServer(IPEndPoint hostEp)
        {
            // Initialise variables
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
            byte[] packet = null;

            // Set timeout
            this.Socket.Client.ReceiveTimeout = 5000;

            try
            {
                // Get responses until its from the server
                while (!remoteEp.Equals(hostEp))
                {
                    // Get a response
                    packet = this.Socket.Receive(ref remoteEp);
                }
            }
            catch (SocketException)
            {
                // Received timed out
                packet = null;
            }

            // Reset the timeout
            this.Socket.Client.ReceiveTimeout = -1;
            return packet;
        }
    }
}
