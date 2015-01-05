// TimerChangedEventArgs.cs
// <copyright file="TimerChangedEventArgs.cs"> This code is protected under the MIT License. </copyright>
using System;

namespace Tron.EventArguments
{
    /// <summary>
    /// The event arguments for when a car changes direction.
    /// </summary>
    public class TimerChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrashedEventArgs" /> class.
        /// </summary>
        /// <param name="carID"> The id of the car. </param>
        public TimerChangedEventArgs(int timeLeft)
        {
            this.TimeLeft = timeLeft;
        }

        /// <summary>
        /// Gets or sets the id number of the car that changed direction.
        /// </summary>
        public int TimeLeft { get; set; }
    }
}
