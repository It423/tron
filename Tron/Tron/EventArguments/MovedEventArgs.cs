// MovedEventArgs.cs
// <copyright file="MovedEventArgs.cs"> This code is protected under the MIT License. </copyright>
using System;

namespace Tron.EventArguments
{
    /// <summary>
    /// The event arguments for when a car changes direction.
    /// </summary>
    public class MovedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovedEventArgs" /> class.
        /// </summary>
        /// <param name="x"> The car's x value. </param>
        /// <param name="y"> The car's y value. </param>
        /// <param name="carID"> The id of the car. </param>
        public MovedEventArgs(int x, int y, int carID)
        {
            this.X = x;
            this.Y = y;
            this.CarID = carID;
        }

        /// <summary>
        /// Gets or sets the new x value of the car.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the new y value of the car.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the id number of the car that changed direction.
        /// </summary>
        public int CarID { get; set; }
    }
}
