// NetGameExtensions.cs
// <copyright file="NetGameExtensions.cs"> This code is protected under the MIT License. </copyright>
using Tron;

namespace Networking.Extensions
{
    /// <summary>
    /// An extension class for the game containing useful tools for a networked game of tron.
    /// </summary>
    public static class NetGameExtensions
    {
        /// <summary>
        /// Adjusts a car in the game accordingly to a byte array.
        /// </summary>
        /// <param name="g"> The game. </param>
        /// <param name="byteArray"> The byte array. </param>
        public static void SetCarFromByteArray(this TronGame g, byte[] byteArray)
        {
            // Get the index of the car
            int index = byteArray[0];

            // Adjust the car 
            g.Cars[index].FromByteArray(byteArray);
        }
    }
}
