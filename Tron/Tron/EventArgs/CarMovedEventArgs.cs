// CarMovedEventArgs.cs
// <copyright file="CarMovedEventArgs.cs"> This code is protected under the MIT License. </copyright>
namespace Tron.EventArgs
{
    /// <summary>
    /// The event arguments for when the timer changes.
    /// </summary>
    public class CarMovedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarMovedEventArgs" /> class.
        /// </summary>
        /// <param name="id"> The car's id number. </param>
        /// <param name="x"> The car's x position. </param>
        /// <param name="y"> The car's y position. </param>
        public CarMovedEventArgs(int id, int x, int y)
        {
            this.ID = id;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the car's id number.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the car's new x position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the car's new y position.
        /// </summary>
        public int Y { get; set; }
    }
}
