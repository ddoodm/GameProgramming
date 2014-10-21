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

namespace _11688025_lab10
{
    /// <summary>
    /// Parameters to pass to an Entity when updating.
    /// </summary>
    public struct UpdateParams
    {
        public GameTime gameTime;
        public Camera camera;
        public KeyboardState keyboardState;
        public MouseState mouseState;
        public Player player;
        public World world;
        public Random random;
    }

    /// <summary>
    /// Parameters to pass to an Entity when drawing.
    /// </summary>
    public struct DrawParams
    {
        public GameTime gameTime;
        public Camera camera;
        public KeyboardState keyboardState;
        public MouseState mouseState;
        public GraphicsDevice graphicsDevice;
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Random randomGenerator;

        public Player player;
        public World world;
        public Camera camera;

        /// <summary>
        /// Topdown camera description
        /// </summary>
        private CameraTuple topdownCamDesc = new CameraTuple()
        {
            position = new Vector3(-100f, 200f, 100f),
            target = Vector3.Zero,
            up = Vector3.Up
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            randomGenerator = new Random();

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create the world
            world = new World(this);

            // Create a player at (-100,0,0)
            player = new Player(this, Matrix.CreateTranslation(new Vector3(-150f, 0, 0)), world);

            // Configure the camera as an FPS camera for the player
            camera = new FlyingCamera(this, topdownCamDesc);

            // Supply the Player to the world
            world.player = player;

            // Load the world
            world.hardcodedWorldPopulation(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load world content
            world.load(Content);

            player.loadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
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
                player = this.player,
                world = this.world,
                random = randomGenerator,
            };

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            // Stop working if the player has died
            if (player.isDead)
            {
                // When 'return' is pressed, restore health
                if (keyboard.IsKeyDown(Keys.Enter))
                    player.health = 1f;
                else
                    return;
            }

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
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Peripheral states
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            GraphicsDevice.Clear(ClearOptions.Target, new Vector4(0.70f, 0.67f, 0.63f, 1.0f), 50f, 0);

            // Prepare drawing parameters for entities, statics and primitives
            DrawParams drawParams = new DrawParams()
            {
                camera = this.camera,
                gameTime = gameTime,
                keyboardState = keyboard,
                mouseState = mouse,
                graphicsDevice = this.GraphicsDevice
            };

            // Draw the world
            world.draw(drawParams);

            // Draw the player
            player.draw(drawParams);

            base.Draw(gameTime);
        }
    }
}
