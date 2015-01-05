// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Networking.Extensions;
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
            this.Connector = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            this.HostIP = new IPEndPoint(IPAddress.Any, 0);
            this.ListenThread = new Thread(() => this.ListenForMessages());
        }

        /// <summary>
        /// Gets or sets the connector socket.
        /// </summary>
        public UdpClient Connector { get; set; }

        /// <summary>
        /// Gets or sets the ip address of the host.
        /// </summary>
        public IPEndPoint HostIP { get; set; }

        /// <summary>
        /// Gets or sets the id of the player represented by this client.
        /// </summary>
        public int OnlinePlayerId { get; set; }

        /// <summary>
        /// Gets or sets the thread for listening for messages.
        /// </summary>
        protected Thread ListenThread { get; set; }

        /// <summary>
        /// Searches the local area network for available servers.
        /// </summary>
        /// <returns> The ip addresses of available servers. </returns>
        public IPAddress[] SearchLANForServers()
        {
            // Set 5 second timeout
            this.Connector.Client.ReceiveTimeout = 5000;

            // Send a message saying we are searching
            byte[] packet = new byte[] { 0 };
            this.Connector.Send(packet, 1, new IPEndPoint(IPAddress.Parse("255.255.255.255"), Server.Port));

            // Wait for responses
            List<IPAddress> ips = new List<IPAddress>();
            while (true)
            {
                try
                {
                    IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedPacket = this.Connector.Receive(ref senderIP);

                    // Check the packet is valid and the server says it has space
                    if (receivedPacket.Length == 0 && receivedPacket[0] <= 12)
                    {
                        ips.Add(senderIP.Address);
                    }
                }
                catch
                {
                    // Receive timed out, no more ips received
                    break;
                }
            }

            return ips.ToArray();
        }

        /// <summary>
        /// Connects to a host at a specified ip address.
        /// </summary>
        /// <param name="hostIP"> The host's ip address. </param>
        /// <returns> Whether the connection was completed. </returns>
        /// <remarks> Returns true for connected, false for server full and null for host not found. </remarks>
        public bool? Connect(IPAddress hostIP)
        {
            // Set the timeout to 5 seconds
            this.Connector.Client.ReceiveTimeout = 5000;

            // Send a message to the host saying we are trying to connect
            byte[] packet = new byte[] { 1 };
            this.Connector.Send(packet, 1, new IPEndPoint(hostIP, Server.Port));

            // Wait for response
            try
            {
                IPEndPoint reply = this.HostIP;
                packet = this.Connector.Receive(ref reply);

                // Make sure its the same host
                if (reply != new IPEndPoint(hostIP, Server.Port) || packet.Length != 1)
                {
                    return null;
                }
            }
            catch (SocketException)
            {
                // Not connected if receive timed out
                return null;
            }

            // Check if the server is full or a bad number
            if (packet[0] == 255 || packet[0] > 11)
            {
                return false;
            }
            else
            {
                // Server is not full, connect to server
                this.HostIP = new IPEndPoint(hostIP, Server.Port);
                this.OnlinePlayerId = packet[0];
                this.Connector.Client.ReceiveTimeout = -1;

                // Start listening for messages
                this.ListenThread = new Thread(() => this.ListenForMessages());
                this.ListenThread.Start();

                return true;
            }
        }

        /// <summary>
        /// Listens for messages from the server.
        /// </summary>
        public void ListenForMessages()
        {
            while (true)
            {
                // Get data
                IPEndPoint receiveFrom = new IPEndPoint(IPAddress.Any, 0);
                byte[] packet = this.Connector.Receive(ref receiveFrom);

                // Check its from server
                if (receiveFrom != this.HostIP)
                {
                    continue;
                }

                // Parse the information
                this.ParsePacket(packet);
            }
        }

        /// <summary>
        /// Parses a packet sent to the client.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        public void ParsePacket(byte[] packet)
        {
            // Work out if it is car data or a server message
            if (packet[0] == 255)
            {
                this.ParseServerPacket(packet);
            }
            else
            {
                this.ParseCarPacket(packet);
            }
        }

        /// <summary>
        /// Parses a packet about the game information sent to the client.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        public void ParseServerPacket(byte[] packet)
        {
            if (packet.Length == 2 && packet[1] == 0)
            {
                // Add a car
                TronData.Tron.AddPlayer();
            }
            if (packet.Length == 2 && packet[1] == 255)
            {
                // Disconnected
                this.HostIP = new IPEndPoint(IPAddress.Any, 0); // Only remove the host ip ad this will stop incomming transmissions and the game will detect this and run the disconnect method
            }
            else if (packet.Length == 3 && packet[1] == 255)
            {
                // Set the timer
                TronData.Tron.TimeTillAction = packet[1] + 1;

                // Run the timer decreaser
                TronData.Tron.DecTimer();
            }
            else
            {
                // Set scores
                for (int i = 1; i < packet.Length; i++)
                {
                    TronData.Tron.SetCarScoreFromByteArray(new byte[] { packet[i], packet[i + 1] });
                }
            }
        }

        /// <summary>
        /// Parses a packet about a car sent to the client.
        /// </summary>
        /// <param name="packet"> The packet of data. </param>
        public void ParseCarPacket(byte[] packet)
        {
            if (packet.Length == 2)
            {
                if (packet[1] == 255)
                {
                    // Remove car if told to
                    TronData.Tron.RemovePlayer(packet[0]);
                }
                else
                {
                    // Crash car if told to
                    TronData.Tron.Cars[packet[0]].Alive = false;
                }
            }
            else
            {
                // Set the position
                TronData.Tron.SetCarPosFromByteArray(packet);
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                this.SendDisconnect();
            }
            catch (SocketException)
            { 
                // Catch socket exceptions from host ip being any ip address and port
            }
            this.HostIP = new IPEndPoint(IPAddress.Any, 0);
            this.ListenThread.Abort();
        }

        /// <summary>
        /// Sends the server a message saying the client is trying to change direction.
        /// </summary>
        /// <param name="direction"> The direction the client is trying to move. </param>
        public void SendDirection(Direction direction)
        {
            this.Connector.Send(new byte[] { (byte)(int)direction }, 1, this.HostIP);
        }

        /// <summary>
        /// Sends the server a message saying the client is trying to boost.
        /// </summary>
        public void SendBoost()
        {
            this.Connector.Send(new byte[] { 4 }, 1, this.HostIP);
        }

        /// <summary>
        /// Sends the server a message saying the client is trying to disconnect.
        /// </summary>
        public void SendDisconnect()
        {
            this.Connector.Send(new byte[] { 255 }, 1, this.HostIP);
        }
    }
}
