// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
                    this.Tron = new TronGame(0, activePlayers[0]);

                    // Initialize the players list without the score limit
                    List<byte> activePlayerList = activePlayers.ToList();
                    activePlayerList.RemoveAt(0);

                    // Initialize the players
                    for (byte i = 0; i < 12; i++)
                    {
                        if (activePlayerList.Contains(i))
                        {
                            this.Tron.Cars.Add(new Car(i, SpawnLists.XPositions[i], SpawnLists.YPositions[i], SpawnLists.Directions[i], (CellValues)i));
                        }
                        else
                        {
                            this.Tron.Cars.Add(null);
                        }
                    }

                    // Finish connection
                    this.HostEP = hostEp;
                    this.Connected = true;
                    return true;
                }
            }
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
