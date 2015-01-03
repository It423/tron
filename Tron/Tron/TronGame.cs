// TronGame.cs
// <copyright file="TronGame.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework.Graphics;
using Tron.Exceptions;

namespace Tron
{
    /// <summary>
    /// A class that holds all the game data.
    /// </summary>
    public class TronGame
    {
        /// <summary>
        /// The width of the grid.
        /// </summary>
        public static readonly int GridWidth = 501;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public static readonly int GridHeight = 251;

        /// <summary>
        /// Initializes a new instance of the <see cref="TronGame" /> class.
        /// </summary>
        /// <param name="players"> How many players will be in the game. </param>
        public TronGame(int players)
        {
            // Initalize the grid
            this.InitializeGrid();

            // Add the players
            this.Players = 0;
            for (int i = 0; i < players; i++)
            {
                this.AddPlayer();
            }
        }

        /// <summary>
        /// Gets or sets the car list. 
        /// </summary>
        public List<Car> Cars { get; set; }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        public CellValues[][] Grid { get; set; }

        /// <summary>
        /// Gets or sets how many players are going to play.
        /// </summary>
        public int Players { get; set; }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void InitializeGame()
        {
            this.InitializeGrid();
            this.InitializePlayers();
        }

        /// <summary>
        /// Initializes the grid.
        /// </summary>
        public void InitializeGrid()
        {
            // Set x size
            this.Grid = new CellValues[TronGame.GridWidth][];
            
            // Set y size in each row
            for (int i = 0; i < TronGame.GridWidth; i++)
            {
                this.Grid[i] = new CellValues[TronGame.GridHeight];
            }
        }

        /// <summary>
        /// Initializes the players.
        /// </summary>
        public void InitializePlayers()
        {
            // Initialize the car list
            this.Cars = new List<Car>(12);

            // Initialize each car
            for (int i = 0; i < this.Players; i++)
            {
                this.Cars.Add(new Car(i, SpawnLists.XPositions[i], SpawnLists.YPositions[i], SpawnLists.Directions[i], (CellValues)i + 1));
            }
        }

        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        public void AddPlayer()
        {
            if (this.Players < 12)
            {
                this.Players++;
            }
            else
            {
                throw new TooManyPlayersException();
            }
        }

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        public void RemovePlayer(int playerIndex)
        {
            if (this.Players > 0)
            {
                this.Players--;
                this.Cars[playerIndex] = null;
            }
        }

        /// <summary>
        /// Updates the game.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < this.Cars.Count; i++)
            {
                // Update the cars]
                if (this.Cars[i] != null)
                {
                    CellValues[][] gridCopy = this.Grid;
                    this.Cars[i].Move(ref gridCopy);
                    this.Grid = gridCopy;
                }
            }
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw each cell
            for (int r = 0; r < this.Grid.Length; r++)
            {
                for (int c = 0; c < this.Grid[r].Length; c++)
                {
                    Drawing.DrawCell(r, c, Drawing.GetColour(this.Grid[r][c]), spriteBatch);
                }
            }

            // Draw each car
            foreach (Car c in this.Cars)
            {
                if (c != null)
                {
                    Drawing.DrawCell(c.X, c.Y, Drawing.GetColour(c.Colour), spriteBatch);
                }
            }

            // Draw the border
            Drawing.DrawBorder(spriteBatch);

            spriteBatch.End();
        }
    }
}
