// TooManyPlayersException.cs
// <copyright file="TooManyPlayersException.cs"> This code is protected under the MIT License. </copyright>
using System;

namespace Tron.Exceptions
{
    /// <summary>
    /// An exception thrown when too many players are in the game.
    /// </summary>
    public class TooManyPlayersException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyPlayersException" /> class.
        /// </summary>
        public TooManyPlayersException()
        { 
        }
    }
}
