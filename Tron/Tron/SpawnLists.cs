// SpawnLists.cs
// <copyright file="SpawnLists.cs"> This code is protected under the MIT License. </copyright>
using Tron.CarData;

namespace Tron
{
    /// <summary>
    /// A static class that stores all car spawning data.
    /// </summary>
    public static class SpawnLists
    {
        /// <summary>
        /// The list of x position the car can spawn in.
        /// </summary>
        public static readonly int[] XPositions = { 249, 251, 250, 250, 1, 0, 500, 499, 0, 1, 499, 500 };

        /// <summary>
        /// The list of y position the car can spawn in.
        /// </summary>
        public static readonly int[] YPositions = { 125, 125, 124, 126, 0, 1, 249, 250, 249, 250, 0, 1 };

        /// <summary>
        /// The list of direction the car can spawn in.
        /// </summary>
        public static readonly Direction[] Directions = { Direction.Left, Direction.Right, Direction.Up, Direction.Down, Direction.Right, Direction.Down, Direction.Up, Direction.Left, Direction.Up, Direction.Right, Direction.Left, Direction.Down };
    }
}
