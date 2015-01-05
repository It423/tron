// Game.cs
// <copyright file="Game.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tron;
using Tron.CarData;

namespace Application
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        public Game()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            // Set the screen size
            this.Graphics.PreferredBackBufferWidth = TronGame.GridWidth * 2;
            this.Graphics.PreferredBackBufferHeight = (TronGame.GridHeight * 2) + 100;

            // Set window title
            this.Window.Title = "Tron";

            // Show the mouse
            this.IsMouseVisible = true;

            // Get the current keyboard
            this.CurrentKeyboard = Keyboard.GetState();

            // Set the update to run 10 times a second
            this.TargetElapsedTime = TimeSpan.FromSeconds(1f / 50f);

            // Set the draw method to run as much as possible
            this.Graphics.SynchronizeWithVerticalRetrace = false;
        }

        /// <summary>
        /// Gets or sets the graphics device.
        /// </summary>
        public GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// Gets or sets the sprite batch drawing tool.
        /// </summary>
        public SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        /// Gets or sets the current keyboard state.
        /// </summary>
        public KeyboardState CurrentKeyboard { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the content
            Drawing.CellTexture = this.Content.Load<Texture2D>("Cell");
            Drawing.BoostTexture = this.Content.Load<Texture2D>("Boost-Icon");
            Drawing.DeadFont = this.Content.Load<SpriteFont>("Dead-Font");
            Drawing.HUDFont = this.Content.Load<SpriteFont>("HUD-Data-Font");
            Drawing.WinFont = this.Content.Load<SpriteFont>("Victory-Message-Font");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            // Update game
            if (GameData.LocalMultiPlayer)
            {
                TronData.Tron.Update();
            }
            else
            {
                TronData.Tron.CheckRoundOver();

                // Check if the player has been kicked
                if (GameData.Client.HostIP == new IPEndPoint(IPAddress.Any, 0))
                {
                    this.Exit();
                    Console.WriteLine("You have been disconnected from the server.");
                }
            }

            this.UpdateInputs();

            base.Update(gameTime);
        }

        /// <summary>
        /// Run commands from keyboard inputs.
        /// </summary>
        protected void UpdateInputs()
        {
            // Get the new keyboard state
            KeyboardState newKeyboard = Keyboard.GetState();

            // Player one controls
            this.CheckPlayerOneKeys(newKeyboard);

            // Player two controls
            this.CheckPlayerTwoKeys(newKeyboard);

            // Player 3 controls
            this.CheckPlayerThreeKeys(newKeyboard);
          
            // Store the new keyboard
            this.CurrentKeyboard = newKeyboard;
        }

        /// <summary>
        /// Checks which of keys in player one's control set has been pressed.
        /// </summary>
        /// <param name="newKeyboard"> The new keyboard state. </param>
        protected void CheckPlayerOneKeys(KeyboardState newKeyboard)
        {
            if (this.WasKeyPressed(newKeyboard, Keys.W))
            {
                GameData.ChangeDirection(Direction.Up, 0);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.A))
            {
                GameData.ChangeDirection(Direction.Left, 0);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.S))
            {
                GameData.ChangeDirection(Direction.Down, 0);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.D))
            {
                GameData.ChangeDirection(Direction.Right, 0);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.LeftShift))
            {
                GameData.Boost(0);
            }
        }

        /// <summary>
        /// Checks which of keys in player two's control set has been pressed.
        /// </summary>
        /// <param name="newKeyboard"> The new keyboard state. </param>
        protected void CheckPlayerTwoKeys(KeyboardState newKeyboard)
        {
            if (this.WasKeyPressed(newKeyboard, Keys.Up))
            {
                GameData.ChangeDirection(Direction.Up, 1);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.Left))
            {
                GameData.ChangeDirection(Direction.Left, 1);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.Down))
            {
                GameData.ChangeDirection(Direction.Down, 1);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.Right))
            {
                GameData.ChangeDirection(Direction.Right, 1);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.RightControl))
            {
                GameData.Boost(1);
            }
        }
        
        /// <summary>
        /// Checks which of keys in player three's control set has been pressed.
        /// </summary>
        /// <param name="newKeyboard"> The new keyboard state. </param>
        protected void CheckPlayerThreeKeys(KeyboardState newKeyboard)
        {
            if (this.WasKeyPressed(newKeyboard, Keys.I))
            {
                GameData.ChangeDirection(Direction.Up, 2);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.J))
            {
                GameData.ChangeDirection(Direction.Left, 2);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.K))
            {
                GameData.ChangeDirection(Direction.Down, 2);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.L))
            {
                GameData.ChangeDirection(Direction.Right, 2);
            }

            if (this.WasKeyPressed(newKeyboard, Keys.B))
            {
                GameData.Boost(2);
            }
        }

        /// <summary>
        /// Checks if a key was pressed this frame.
        /// </summary>
        /// <param name="newKeyboard"> The new keyboard state. </param>
        /// <param name="key"> The key to check. </param>
        /// <returns> Whether the key was recently pressed. </returns>
        protected bool WasKeyPressed(KeyboardState newKeyboard, Keys key)
        {
            return !this.CurrentKeyboard.IsKeyDown(key) && newKeyboard.IsKeyDown(key);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Drawing.GetColour(CellValues.None));

            TronData.Tron.Draw(SpriteBatch);
            GameData.DrawPlayerHUD(SpriteBatch);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Runs on the exiting of the game.
        /// </summary>
        /// <param name="sender"> What raised the event. </param>
        /// <param name="args"> The event arguments. </param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            // Disconnect from server
            GameData.Client.Disconnect();

            base.OnExiting(sender, args);
        }
    }
}
