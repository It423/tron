// TronGame.cs
// <copyright file="TronGame.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework.Graphics;
using Tron.CarData;
using Tron.Exceptions;
using Tron.EventArguments;

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
        /// <param name="pointsToWin"> How many points are required to win the game. </param>
        public TronGame(int players, int pointsToWin)
        {
            // Initalize the grid
            this.InitializeGrid();

            // Add the players
            this.Players = 0;
            for (int i = 0; i < players; i++)
            {
                this.AddPlayer();
            }

            // Initalize other properties
            this.RoundFinished = true;
            this.GameWon = true;
            this.PointsToWin = pointsToWin;
            this.TimerActive = true;
            this.TimeTillAction = 5;
            this.Action = "round begins";

            // Active the timer
            this.Timer = new Timer(1000);
            this.Timer.AutoReset = true;
            this.Timer.Elapsed += this.HandleTimer;
            this.Timer.Enabled = true;
            this.Timer.Start();
        }

        /// <summary>
        /// Fires when the timer changes.
        /// </summary>
        public event EventHandler<TimerChangedEventArgs> TimerChanged;

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
        /// Gets or sets a value indicating whether the round has finished.
        /// </summary>
        public bool RoundFinished { get; set; }

        /// <summary>
        /// Gets or sets how many wins are required to win the game.
        /// </summary>
        public int PointsToWin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game has been won.
        /// </summary>
        public bool GameWon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is active.
        /// </summary>
        public bool TimerActive { get; set; }

        /// <summary>
        /// Gets or sets how many seconds till an action takes place.
        /// </summary>
        public int TimeTillAction { get; set; }

        /// <summary>
        /// Gets or sets a string saying what the action is.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the timer.
        /// </summary>
        public Timer Timer { get; set; }

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
        /// Resets the game.
        /// </summary>
        /// <param name="resetScore"> Whether the score needs to be reset. </param>
        public void ResetGame(bool resetScore)
        {
            // Reset the grid then players
            this.InitializeGrid();
            this.ResetPlayers(resetScore);
        }

        /// <summary>
        /// Resets the players.
        /// </summary>
        /// <param name="resetScore"> Whether the score needs to be reset. </param>
        public void ResetPlayers(bool resetScore)
        {
            // Reset players in the game
            List<int> nullPlayerIndexes = new List<int>();
            int spawnedPlayers = 0;
            for (int i = 0; i < this.Cars.Count; i++)
            {
                if (this.Cars[i] == null)
                {
                    nullPlayerIndexes.Add(i);
                }
                else
                {
                    this.Cars[i].Reset(SpawnLists.XPositions[i], SpawnLists.YPositions[i], SpawnLists.Directions[i]);
                    spawnedPlayers++;

                    // Reset the score if its is required
                    if (resetScore)
                    {
                        this.Cars[i].Victories = 0;
                    }
                }
            }

            // Add new players into the game if need be using null player slots
            for (int i = 0; this.Players - spawnedPlayers != 0 && i < nullPlayerIndexes.Count; i++, spawnedPlayers++)
            {
                this.Cars[nullPlayerIndexes[i]] = new Car(nullPlayerIndexes[i], SpawnLists.XPositions[nullPlayerIndexes[i]], SpawnLists.YPositions[nullPlayerIndexes[i]], SpawnLists.Directions[nullPlayerIndexes[i]], (CellValues)nullPlayerIndexes[i] + 1);
            }

            // Add new players into the game using unused player slots
            for (int i = this.Cars.Count; this.Players - spawnedPlayers != 0; i++, spawnedPlayers++)
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
        /// <param name="playerIndex"> The index of the player to remove. </param>
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
            if (!this.RoundFinished)
            {
                for (int i = 0; i < this.Cars.Count; i++)
                {
                    // Update the cars
                    if (this.Cars[i] != null)
                    {
                        CellValues[][] gridCopy = this.Grid;
                        this.Cars[i].Move(ref gridCopy);
                        this.Grid = gridCopy;
                    }
                }

                // Check if the round has finished
                this.CheckRoundOver();
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
                try
                {
                    for (int c = 0; c < this.Grid[r].Length; c++)
                    {
                        Drawing.DrawCell(r, c, Drawing.GetColour(this.Grid[r][c]), spriteBatch);
                    }
                }
                catch (NullReferenceException)
                {
                    // Avoid issues with grid being reset while game is being drawn
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

            if (this.TimerActive)
            {
                // Draw the timer if the timer is active
                Drawing.DrawTimer(this.TimeTillAction, this.Action, spriteBatch);
            }
            else if (this.RoundFinished)
            {
                // Draw the victory message if the round has been won
                // Get the winning car
                Car winner = null;
                try
                {
                    winner = this.Cars.Where(c => c.Alive).ToList()[0];
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Catch the exception but leave the winner as null if it was a tie
                }

                // Draw the victory message
                Drawing.DrawVictoryMessage(winner, this.PointsToWin, spriteBatch);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Checks if the round has finished and runs code accordingly.
        /// </summary>
        public void CheckRoundOver()
        {
            // Check if the round has finished
            List<Car> aliveCars = this.Cars.Where(c => c.Alive).ToList();
            if (aliveCars.Count <= 1)
            {
                this.RoundFinished = true;

                // Set the timer to display the victory message for 5 seconds
                this.Timer.Interval = 5000;
                this.Timer.Start();

                // Add one to the car's score if it won
                if (aliveCars.Count == 1)
                {
                    this.Cars[this.Cars.IndexOf(aliveCars[0])].Victories++;

                    // See if the car has won the game
                    if (this.Cars[this.Cars.IndexOf(aliveCars[0])].Victories >= this.PointsToWin)
                    {
                        this.GameWon = true;
                    }
                }
            }
        }

        /// <summary>
        /// Decreases the timer.
        /// </summary>
        public void DecTimer()
        {
            if (this.Action == string.Empty)
            {
                // Restart the timer if begin told to
                this.TimerActive = true;
                this.TimeTillAction = 5;
                this.Timer.Interval = 1000;
                this.Timer.Start();

                // Set the action
                this.Action = "round begins";
                if (this.GameWon)
                {
                    this.Action = "game restarts";
                    this.TimeTillAction = 15;
                }

                // Reset the game 
                this.ResetGame(this.GameWon);
            }
            else
            {
                // Star the game if the timer reaches 0
                if (this.TimeTillAction == 1)
                {
                    // Deactivate the timer
                    this.Action = string.Empty;
                    this.TimerActive = false;
                    this.Timer.Stop();

                    // Say game has begin
                    this.GameWon = false;
                    this.RoundFinished = false;
                }

                // Decrease the timer
                this.TimeTillAction--;
            }


        }

        /// <summary>
        /// Handles the timer elapsing.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        protected void HandleTimer(object sender, ElapsedEventArgs e)
        {
            this.DecTimer();
        }

        /// <summary>
        /// Runs the event methods attached to the Crashed event.
        /// </summary>
        /// <param name="origin"> The origin of the event. </param>
        /// <param name="e"> The event arguments. </param>
        protected virtual void OnTimerChange(object origin, TimerChangedEventArgs e)
        {
            EventHandler<TimerChangedEventArgs> handler = this.TimerChanged;

            if (handler != null)
            {
                handler(origin, e);
            }
        }
    }
}
