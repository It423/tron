// Drawing.cs
// <copyright file="Drawing.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tron
{
    /// <summary>
    /// A static class used for drawing.
    /// </summary>
    public static class Drawing
    {
        /// <summary>
        /// Gets or sets the texture for a cell.
        /// </summary>
        public static Texture2D CellTexture { get; set; }

        /// <summary>
        /// Gets or sets the texture for a car.
        /// </summary>
        public static List<Texture2D> CarTexture { get; set; }

        /// <summary>
        /// Gets a colour from a cell value's colour.
        /// </summary>
        /// <param name="colour"> The cell value. </param>
        /// <returns> The colour for this value. </returns>
        public static Color GetColour(CellValues colour)
        {
            switch (colour)
            {
                case CellValues.Red: return new Color(255, 0, 0);
                case CellValues.Green: return new Color(0, 255, 0);
                case CellValues.Blue: return new Color(0, 0, 255);
                case CellValues.Yellow: return new Color(255, 255, 0);
                case CellValues.White: return new Color(255, 255, 255);
                case CellValues.Black: return new Color(0, 0, 0);
                case CellValues.Pink: return new Color(255, 0, 255);
                case CellValues.Orange: return new Color(255, 175, 0);
                case CellValues.Purple: return new Color(190, 50, 255);
                case CellValues.Brown: return new Color(175, 130, 35);
                default: return new Color(10, 60, 60);
            }
        }

        /// <summary>
        /// Draws a cell.
        /// </summary>
        /// <param name="row"> The cell's row number. </param>
        /// <param name="column"> The cell's column number. </param>
        /// <param name="colour"> The cell's colour. </param>
        /// <param name="sp"> The spritebatch drawing tool. </param>
        public static void DrawCell(int row, int column, Color colour, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Get position and draw
            Vector2 pos = new Vector2(row * CellTexture.Width, column * CellTexture.Height);
            spriteBatch.Draw(CellTexture, pos, colour);

            spriteBatch.End();
        }

        /// <summary>
        /// Draws a car.
        /// </summary>
        /// <param name="row"> The car's row number. </param>
        /// <param name="column"> The car's column number. </param>
        /// <param name="colour"> The car's colour. </param>
        /// <param name="sp"> The spritebatch drawing tool. </param>
        public static void DrawCar(int row, int column, Color colour, Direction direction, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Get the positon and draw the car
            Vector2 pos = new Vector2(row * CarTexture[(int)direction].Width, column * CarTexture[(int)direction].Height);
            spriteBatch.Draw(CarTexture[(int)direction], pos, colour);

            spriteBatch.End();
        }
    }
}
