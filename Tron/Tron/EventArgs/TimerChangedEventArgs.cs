// TimerChangedEventArgs.cs
// <copyright file="TimerChangedEventArgs.cs"> This code is protected under the MIT License. </copyright>
namespace Tron.EventArgs
{
    /// <summary>
    /// The event arguments for when the timer changes.
    /// </summary>
    public class TimerChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerChangedEventArgs" /> class.
        /// </summary>
        /// <param name="timeLeft"> How much time is left on the timer. </param>
        public TimerChangedEventArgs(int timeLeft)
        {
            this.TimeLeft = timeLeft;
        }

        /// <summary>
        /// Gets or sets how much time is left on the timer.
        /// </summary>
        public int TimeLeft { get; set; }
    }
}
