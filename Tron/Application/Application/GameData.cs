// GameData.cs
// <copyright file="GameData.cs"> This code is protected under the MIT License. </copyright>
using System;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using Tron;
using Tron.CarData;

namespace Application
{
    /// <summary>
    /// A class containing all game data and method for input handling.
    /// </summary>
    public static class GameData
    {
        /// <summary>
        /// The list of x positions for the game HUD.
        /// </summary>
        public static readonly int[] LocalHUDXPos = { 100, (TronGame.GridWidth * 2) - 280, TronGame.GridWidth - 90 };

        /// <summary>
        /// Gets or sets a value indicating whether the current game is a local multiplayer one.
        /// </summary>
        public static bool LocalMultiPlayer { get; set; }

        /// <summary>
        /// Gets or sets how many local players are playing.
        /// </summary>
        public static int LocalPlayers { get; set; }

        /// <summary>
        /// Gets or sets the local tron game.
        /// </summary>
        public static TronGame Tron { get; set; }

        /// <summary>
        /// Gets or sets the client for a networked game.
        /// </summary>
        public static Client Client { get; set; }

        /// <summary>
        /// Sends the move command to a player.
        /// </summary>
        /// <param name="direction"> The direction to move. </param>
        /// <param name="player"> The player inputting the move. </param>
        public static void ChangeDirection(Direction direction, int player)
        {
            // Only run command if the game is active
            if (!Tron.RoundFinished)
            {
                // Send redirection to client if not local multiplayer
                if (!LocalMultiPlayer)
                {
                    // If the player key set is not that of player one don't do anything
                    if (player != 0)
                    {
                        return;
                    }

//                    Client.SendDirection(direction);
                }
                else
                {
                    // Don't do anything if the command is for a non existant player
                    if (player + 1 > LocalPlayers)
                    {
                        return;
                    }

                    Tron.Cars[player].ChangeDirection(direction);
                }
            }
        }

        /// <summary>
        /// Boosts a player.
        /// </summary>
        /// <param name="player"> The player inputting the boost. </param>
        public static void Boost(int player)
        {
            // Only run command if the game is active
            if (!Tron.RoundFinished)
            {
                // Send boost to client if not local multiplayer
                if (!LocalMultiPlayer)
                {
                    // If the player key set is not that of player one don't do anything
                    if (player != 0)
                    {
                        return;
                    }

//                    Client.SendBoost();
                }
                else
                {
                    // Don't do anything if the command is for a non existant player
                    if (player + 1 > LocalPlayers)
                    {
                        return;
                    }

                    Tron.Cars[player].Boost();
                }
            }
        }

        /// <summary>
        /// Draws the player's HUD.
        /// </summary>
        /// <param name="spriteBatch"> The sprite batch drawing tool. </param>
        public static void DrawPlayerHUD(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (LocalMultiPlayer)
            {
                // Draw player one and player two's HUD
                for (int i = 0; i < LocalPlayers; i++)
               {
                   Drawing.DrawHUD(LocalHUDXPos[i], Tron.Cars[i], spriteBatch);
               }
            }
            else
            {
                // Draw the player's HUD and the leaderboard
                Drawing.DrawHUD(LocalHUDXPos[0], Client.Tron.Cars[Client.OnlineID], spriteBatch);
                Drawing.DrawLeaderboard((int)(840 - (170 * Math.Truncate((decimal)(Client.Tron.Cars.Count - 1) / 4))), Client.Tron.Cars, spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
