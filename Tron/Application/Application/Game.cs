// Game.cs
// <copyright file="Game.cs"> This code is protected under the MIT License. </copyright>
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tron;

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
            this.Graphics.PreferredBackBufferWidth = TronGame.GridWidth * 3;
            this.Graphics.PreferredBackBufferHeight = TronGame.GridHeight * 3;

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
        /// Gets or sets the spritebatch drawing tool.
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

            // Load the cell texture
            Drawing.CellTexture = this.Content.Load<Texture2D>("Cell");
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

            GameData.Tron.Update();

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

            if (this.WasKeyPressed(newKeyboard, Keys.Enter))
            {
                GameData.Boost(0);
            }
          
            // Store the new keyboard
            this.CurrentKeyboard = newKeyboard;
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
            GraphicsDevice.Clear(Color.Black);

            GameData.Tron.Draw(SpriteBatch);

            base.Draw(gameTime);
        }
    }
}
