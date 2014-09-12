using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The main Game structure
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Standard XNA graphics interfaces
        /// </summary>
        private GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        /// <summary>
        /// High-level game objects
        /// </summary>
        public Player player;
        public World world;
        public Camera camera;
        public HUD hud;

        /// <summary>
        /// Topdown camera description
        /// </summary>
        private CameraTuple topdownCamDesc = new CameraTuple()
        {
            position = new Vector3(-100f, 200f, 100f),
            target = Vector3.Zero,
            up = Vector3.Up
        };

        /// <summary>
        /// First-person camera description
        /// </summary>
        private CameraTuple fpCamDesc = new CameraTuple()
        {
            position = new Vector3(-100f, 20f, 100f),
            target = Vector3.Zero,
            up = Vector3.Up
        };

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            // Create the world
            world = new World(this);

            // Create a player at (0,0,0)
            player = new Player(this, Matrix.Identity, world);

            // Configure the camera as an FPS camera for the player
            camera = new FPCamera(this, player);

            // Supply the Player to the world
            world.player = player;

            // Create a HUD that displays information about the Player
            hud = new HUD(this, player);

            // Load the world
            world.hardcodedWorldPopulation(this);

            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.loadContent(Content);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// 
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Peripheral states
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Prepare update parameters for entities, statics and primitives
            UpdateParams updateParams = new UpdateParams()
            {
                camera = this.camera,
                gameTime = gameTime,
                keyboardState = keyboard,
                mouseState = mouse,
                world = this.world
            };

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            // Select the type of camera:
            changeCamera(keyboard);

            // Update primary camera
            camera.Update(gameTime);

            // Update the player
            player.update(updateParams);

            // Update the world (entities and static models)
            world.update(updateParams);

            base.Update(gameTime);
        }

        private void changeCamera(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.D1))
                camera = new FPCamera(this, player);
            else if (keyboard.IsKeyDown(Keys.D2))
                camera = new TopdownCamera(this, topdownCamDesc);
            else if (keyboard.IsKeyDown(Keys.D3))
                camera = new FlyingCamera(this, topdownCamDesc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Peripheral states
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Prepare drawing parameters for entities, statics and primitives
            DrawParams drawParams = new DrawParams()
            {
                camera = this.camera,
                gameTime = gameTime,
                keyboardState = keyboard,
                mouseState = mouse
            };

            // Draw the world
            world.draw(drawParams);

            // Draw the player
            player.draw(drawParams);

            // Draw a UI
            hud.draw(drawParams);

            base.Draw(gameTime);
        }
    }
}
