// NetCarExtensions.cs
// <copyright file="NetCarExtensions.cs"> This code is protected under the MIT License. </copyright>
using Tron.CarData;

namespace Networking.Extensions
{
    /// <summary>
    /// An extension class for the car containing useful tools for a networked game of tron.
    /// </summary>
    public static class NetCarExtensions
    {
        /// <summary>
        /// Turns the car's important information into a byte array.
        /// </summary>
        /// <param name="c"> The car. </param>
        /// <returns> The byte array of information. </returns>
        public static byte[] PosToByteArray(this Car c)
        {
            // There will be 4 bytes in the car's byte data
            byte[] bArr = new byte[4];

            // Add the id, x position and y position
            bArr[0] = (byte)c.ID;
            bArr[3] = (byte)c.Y;

            // Add the x position (takes up 2 bytes)
            int[] xVal = new int[2];
            xVal[0] = c.X;
            xVal[1] = 0;
            if (xVal[0] > 255)
            {
                xVal[0] = 255;
                xVal[1] = c.X - 255;
            }

            bArr[1] = (byte)xVal[0];
            bArr[2] = (byte)xVal[1];

            return bArr;
        }

        /// <summary>
        /// Sets the car's position from a byte array.
        /// </summary>
        /// <param name="c"> The car. </param>
        /// <param name="byteArray"> The byte array. </param>
        public static void PosFromByteArray(this Car c, byte[] byteArray)
        {
            // Take the x and y position
            c.X = byteArray[1] + byteArray[2];
            c.Y = byteArray[3];
        }

        /// <summary>
        /// Turns the car's score information into a byte array.
        /// </summary>
        /// <param name="c"> The car. </param>
        /// <returns> The byte array of information. </returns>
        public static byte[] ScoreToByteArray(this Car c)
        {
            // There will be =2 bytes in the car's byte data
            byte[] bArr = new byte[2];

            // Add the id and score
            bArr[0] = (byte)c.ID;
            bArr[1] = (byte)c.Victories;

            return bArr;
        }

        /// <summary>
        /// Sets the car's score from a byte array.
        /// </summary>
        /// <param name="c"> The car. </param>
        /// <param name="byteArray"> The byte array. </param>
        public static void ScoreFromByteArray(this Car c, byte[] byteArray)
        {
            // Take the score
            c.Victories = byteArray[1];
        }
    }
}
