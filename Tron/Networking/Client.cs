// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Networking.Extensions;
using Tron;
using Tron.CarData;
using System.Timers;

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
        /// Connects to a host at a specified ip address.
        /// </summary>
        /// <param name="hostIP"> The host's ip address. </param>
        /// <returns> Whether the connection was completed. </returns>
        /// <remarks> Returns true for connected, false for server full and null for host not found. </remarks>
        public bool? Connect(IPAddress hostIP)
        {
            // Set the timeout to 5 seconds
            this.Connector.Client.ReceiveTimeout = 5000;

            // Send a message to the host
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

            // Check if the server is full
            if (packet[0] == 255)
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
                Thread t = new Thread(() => this.ListenForMessages());

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
            if (packet.Length == 2)
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
            this.SendDisconnect();
            this.HostIP = new IPEndPoint(IPAddress.Any, 0);
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
