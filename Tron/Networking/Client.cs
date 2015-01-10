// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Tron;

namespace Networking
{
    /// <summary>
    /// A class that acts as a client for a networked game of tron.
    /// </summary>
    public class Client
    {
        public Client()
        {
            this.Socket = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            this.HostEP = new IPEndPoint(IPAddress.Any, 0);
            this.Tron = new TronGame(0, 1);
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
        /// Searches the local area network for servers.
        /// </summary>
        /// <returns> Returns the list of local servers. </returns>
        public IPAddress[] SearchForLanServers()
        {
            // Send a message to the local area network
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), Server.Port);
            this.Socket.Send(new byte[] { 0 }, 1, broadcast);

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
    }
}
