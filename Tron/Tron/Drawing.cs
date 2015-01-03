// Drawing.cs
// <copyright file="Drawing.cs"> This code is protected under the MIT License. </copyright>
using System.Collections.Generic;
using System.Linq;
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
        /// Gets or sets the texture for a boost symbol.
        /// </summary>
        public static Texture2D BoostTexture { get; set; }

        /// <summary>
        /// Gets or sets the font used for the win message.
        /// </summary>
        public static SpriteFont WinFont { get; set; }

        /// <summary>
        /// Gets or sets the font used to tell a player they are dead.
        /// </summary>
        public static SpriteFont DeadFont { get; set; }

        /// <summary>
        /// Gets or sets the font used in the HUD.
        /// </summary>
        public static SpriteFont HUDFont { get; set; }

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
                case CellValues.Lime: return new Color(135, 255, 150);
                case CellValues.Grey: return new Color(130, 130, 130);
                default: return new Color(10, 60, 60);
            }
        }

        /// <summary>
        /// Gets the ordinal string of a number.
        /// </summary>
        /// <param name="i"> The number to apply ordinal to. </param>
        /// <returns> The ordinal string. </returns>
        public static string GetOrdinal(int i)
        {
            // Return the number if it has no ordinal
            if (i <= 0)
            {
                return i.ToString();
            }

            // Return 11, 12, 13 with "th"
            switch (i % 100)
            {
                case 11:
                case 12:
                case 13:
                    return i + "th";
            }

            // Return correctly anything else
            switch (i % 10)
            {
                case 1: return i + "st";
                case 2: return i + "nd";
                case 3: return i + "rd";
                default: return i + "th";
            }
        }

        /// <summary>
        /// Draws a cell.
        /// </summary>
        /// <param name="row"> The cell's row number. </param>
        /// <param name="column"> The cell's column number. </param>
        /// <param name="colour"> The cell's colour. </param>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public static void DrawCell(int row, int column, Color colour, SpriteBatch spriteBatch)
        {
            // Get position and draw
            Vector2 pos = new Vector2(row * CellTexture.Width, (column * CellTexture.Height) + 100);
            spriteBatch.Draw(CellTexture, pos, colour);
        }

        /// <summary>
        /// Draws the border between the HUD and the game.
        /// </summary>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public static void DrawBorder(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < TronGame.GridWidth; i++)
            {
                Vector2 pos = new Vector2(i * CellTexture.Width, 98);
                spriteBatch.Draw(CellTexture, pos, GetColour(CellValues.Black));
            }
        }

        /// <summary>
        /// Draws the information related to a car.
        /// </summary>
        /// <param name="xPos"> The x position to place the information. </param>
        /// <param name="c"> The car to display its information. </param>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public static void DrawHUD(int xPos, Car c, SpriteBatch spriteBatch)
        {
            // Get the width of the area
            int BoostWidth = 3 * BoostTexture.Width + 30;

            if (c.Alive)
            {
                // Display how many boosts the player has left
                spriteBatch.DrawString(HUDFont, "Boosts:", new Vector2((BoostWidth - HUDFont.MeasureString("Boosts:").X) / 2 + xPos, 10), GetColour(c.Colour));

                int x = xPos;
                for (int i = 0; i < c.BoostsRemeaning; i++, x += BoostTexture.Width + 15)
                {
                    spriteBatch.Draw(BoostTexture, new Vector2(x, 45), GetColour(c.Colour));
                }
            }
            else
            {
                // Display the player is dead
                spriteBatch.DrawString(DeadFont, "DEAD", new Vector2((BoostWidth - DeadFont.MeasureString("DEAD").X) / 2 + xPos, 0), GetColour(c.Colour));
            }

            // Show how many victories the player has
            spriteBatch.DrawString(HUDFont, string.Format("Victories: {0}", c.Victories), new Vector2((BoostWidth - HUDFont.MeasureString(string.Format("Victories: {0}", c.Victories)).X) / 2 + xPos, 70), GetColour(c.Colour));
        }

        /// <summary>
        /// Draws the leaderboard.
        /// </summary>
        /// <param name="xPos"> The x position to place the information. </param>
        /// <param name="cars"> The list of cars. </param>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public static void DrawLeaderboard(int xPos, List<Car> cars, SpriteBatch spriteBatch)
        {
            // Order the cars
            cars = cars.OrderByDescending(c => c.Victories).ThenByDescending(c => (int)c.Colour).ToList();

            // Draw the leaderbored
            int y = 5;
            int x = xPos;
            int lastPoint = 0;
            int lastPlace = 1;
            for (int i = 0; i < cars.Count; i++, y += 23)
            {
                // Get the correct ordinal value
                int ordinalVal = i + 1;
                if (cars[i].Victories == lastPoint) 
                {
                    ordinalVal = lastPlace;
                }

                spriteBatch.DrawString(HUDFont, string.Format("{0} {1}: {2}", GetOrdinal(ordinalVal), cars[i].Colour, cars[i].Victories), new Vector2(x, y), GetColour(cars[i].Colour));
                
                // Increase the x if the y is too large
                if (y + 30 > 100)
                {
                    y = -18;
                    x += 170;
                }

                // Update the last points and ordinal value set
                lastPoint = cars[i].Victories;
                lastPlace = ordinalVal;
            }
        }

        /// <summary>
        /// Draws a victory message for the car.
        /// </summary>
        /// <param name="winner"> The winning car. </param>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        /// <remarks> If the car inputted is equal to null the procedure will take the result as a tie. </remarks>
        public static void DrawVictoryMessage(Car winner, SpriteBatch spriteBatch)
        {
            // Get the message and colour
            string message;
            Color colour;
            if (winner == null)
            {
                message = "Tie!";
                colour = GetColour(CellValues.White);
            }
            else
            {
                message = string.Format("{0} won the round!", winner.Colour.ToString());
                colour = GetColour(winner.Colour);
            }

            // Get the position of the text
            Vector2 textDimensions = WinFont.MeasureString(message);
            Vector2 pos = new Vector2(((TronGame.GridWidth * CellTexture.Width) - textDimensions.X) / 2, ((TronGame.GridHeight * CellTexture.Height) + 100) / 2);

            // Draw the message
            spriteBatch.DrawString(WinFont, message, pos, colour);
        }
    }
}
