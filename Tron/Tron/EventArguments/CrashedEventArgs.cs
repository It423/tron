// CrashedEventArgs.cs
// <copyright file="CrashedEventArgs.cs"> This code is protected under the MIT License. </copyright>
using System;

namespace Tron.EventArguments
{
    /// <summary>
    /// The event arguments for when a car changes direction.
    /// </summary>
    public class CrashedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrashedEventArgs" /> class.
        /// </summary>
        /// <param name="carID"> The id of the car. </param>
        public CrashedEventArgs(int carID)
        {
            this.CarID = carID;
        }

        /// <summary>
        /// Gets or sets the id number of the car that changed direction.
        /// </summary>
        public int CarID { get; set; }
    }
}
