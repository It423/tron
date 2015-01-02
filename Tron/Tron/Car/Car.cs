// Car.cs
// <copyright file="Car.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tron
{
    /// <summary>
    /// The class that represents a car.
    /// </summary>
    public class Car
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Car" /> class.
        /// </summary>
        /// <param name="id"> The new id number for the car. </param>
        /// <param name="x"> The new x position for the car. </param>
        /// <param name="y"> The new y position for the car. </param>
        /// <param name="direction"> The starting direction for the car. </param>
        /// <param name="colour"> The colour value of the car. </param>
        public Car(int id, int x, int y, Direction direction, CellValues colour)
        {
            this.ID = id;
            this.X = x;
            this.Y = y;
            this.Direction = direction;
            this.NewDirection = direction;
            this.IsBoosting = false;
            this.BoostsRemeaning = 3;
            this.BoostTimeRemeaning = 0;
            this.Colour = colour;
            this.Alive = true;
            this.Victories = 0;
        }

        /// <summary>
        /// Gets or sets the id number.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        public int X { get; protected set; }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        public int Y { get; protected set; }

        /// <summary>
        /// Gets or sets the direction the car is moving in.
        /// </summary>
        public Direction Direction { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the car is boosting.
        /// </summary>
        public bool IsBoosting { get; protected set; }

        /// <summary>
        /// Gets or sets how many boosts the car has left.
        /// </summary>
        public int BoostsRemeaning { get; protected set; }

        /// <summary>
        /// Gets or sets the colour value.
        /// </summary>
        /// <remarks> Uses the <see cref="CellValues" /> enumeration as it contains all the colours. </remarks>
        public CellValues Colour { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the car has crashed.
        /// </summary>
        public bool Alive { get; protected set; }
        
        /// <summary>
        /// Gets or sets the amount of victories for this car.
        /// </summary>
        public int Victories { get; protected set; }

        /// <summary>
        /// Gets or sets how much time the car has left in boost.
        /// </summary>
        protected int BoostTimeRemeaning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the next direction the car will move in.
        /// </summary>
        protected Direction NewDirection { get; set; }

        /// <summary>
        /// Moves the car.
        /// </summary>
        /// <param name="grid"> The grid cars move in. </param>
        /// <param name="firstMove"> Whether this is the first time running the method when it is acting recursively. </param>
        public void Move(CellValues[][] grid, bool firstMove = true)
        {
            if (this.Alive)
            {
                // Check if the car has crashed before movement
                if (this.Crashed(grid))
                {
                    this.Alive = false;
                    return;
                }

                // Tell the grid the car in the this position before we move
                grid[this.X][this.Y] = this.Colour;

                // Move the car now we know it hasn't already crashed
                this.Move();                

                // Check collision again
                if (this.Crashed(grid))
                {
                    this.Alive = false;
                }

                // Tell the grid where the car is now
                grid[this.X][this.Y] |= CellValues.Car | this.Colour;

                // Move twice if boost is active
                if (this.Alive && this.IsBoosting && firstMove)
                {
                    this.Move(grid, false);

                    // Check if boost time is up
                    this.BoostTimeRemeaning--;
                    if (this.BoostTimeRemeaning <= 0)
                    {
                        this.IsBoosting = false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the car has crashed.
        /// </summary>
        /// <param name="grid"> The grid cars move in. </param>
        /// <returns> Whether the car has crashed. </returns>
        public bool Crashed(CellValues[][] grid)
        {
            if (grid[this.X][this.Y] != CellValues.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the direction of the car.
        /// </summary>
        /// <param name="direction"> The new direction. </param>
        public void ChangeDirection(Direction direction)
        {
            if (this.Alive)
            {
                if (((int)direction - 2) % 4 == (int)this.Direction)
                {
                    // Don't change the direction if the car is going to go the way its just been
                    this.NewDirection = this.Direction;
                }
                else
                {
                    this.NewDirection = direction;
                }
            }
        }

        /// <summary>
        /// Puts the car into boost mode.
        /// </summary>
        public void Boost()
        {
            if (this.BoostsRemeaning > 0 && this.Alive)
            {
                this.IsBoosting = true;
                this.BoostsRemeaning--;
                this.BoostTimeRemeaning = 15;
            }
        }

        /// <summary>
        /// Move the car.
        /// </summary>
        protected void Move()
        {
            if (this.Direction == Tron.Direction.Up)
            {
                this.Y--;
            }
            else if (this.Direction == Tron.Direction.Right)
            {
                this.X++;
            }
            else if (this.Direction == Tron.Direction.Down)
            {
                this.Y++;
            }
            else if (this.Direction == Tron.Direction.Left)
            {
                this.X--;
            }
        }
    }
}
