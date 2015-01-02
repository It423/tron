// CellValues.cs
// <copyright file="CellValues.cs"> This code is protected under the MIT License. </copyright>
namespace Tron
{
    /// <summary>
    /// An enumeration to represent the possible values for a cell on the grid.
    /// </summary>
    public enum CellValues
    {
        /// <summary>
        /// Nothing is on the cell.
        /// </summary>
        None = 0,

        /// <summary>
        /// A car is on the cell.
        /// </summary>
        Car = 1,
        
        /// <summary>
        /// Red is the colour of the cell.
        /// </summary>
        Red = 2,

        /// <summary>
        /// Green is the colour of the cell.
        /// </summary>
        Green = 4,

        /// <summary>
        /// Blue is the colour of the cell.
        /// </summary>
        Blue = 8,

        /// <summary>
        /// Yellow is the colour of the cell.
        /// </summary>
        Yellow = 16,

        /// <summary>
        /// White is the colour of the cell.
        /// </summary>
        White = 32,

        /// <summary>
        /// Black is the colour of the cell.
        /// </summary>
        Black = 64,

        /// <summary>
        /// Pink is the colour of the cell.
        /// </summary>
        Pink = 128,

        /// <summary>
        /// Orange is the colour of the cell.
        /// </summary>
        Orange = 256,

        /// <summary>
        /// Pink is the colour of the cell.
        /// </summary>
        Purple = 512, 

        /// <summary>
        /// Brown is the colour of the cell.
        /// </summary>
        Brown = 1024
    }
}
