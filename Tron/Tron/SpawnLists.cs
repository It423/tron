// SpawnLists.cs
// <copyright file="SpawnLists.cs"> This code is protected under the MIT License. </copyright>
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
        public static readonly int[] XPositions = { 149, 151, 150, 150, 1, 0, 300, 299, 0, 1, 299, 300 };

        /// <summary>
        /// The list of y position the car can spawn in.
        /// </summary>
        public static readonly int[] YPositions = { 100, 100, 99, 101, 0, 1, 199, 200, 199, 200, 0, 1 };

        /// <summary>
        /// The list of direction the car can spawn in.
        /// </summary>
        public static readonly Direction[] Directions = { Direction.Left, Direction.Right, Direction.Up, Direction.Down, Direction.Right, Direction.Down, Direction.Up, Direction.Left, Direction.Up, Direction.Right, Direction.Left, Direction.Down };
    }
}
