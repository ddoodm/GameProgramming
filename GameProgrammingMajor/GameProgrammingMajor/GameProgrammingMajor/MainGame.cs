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
        public SoundManager soundManager;
        public HUD hud;
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
    /// The main Game structure
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Standard XNA graphics interfaces
        /// </summary>
        private GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        private SoundManager soundManager;
        private Random randomGenerator;

        /// <summary>
        /// High-level game objects
        /// </summary>
        public Player player;
        public World world;
        public Camera camera;
        public HUD hud;

        private const int MAX_LEVELS = 2;
        private int levelNumber = 0;

        /// <summary>
        /// Topdown camera description
        /// </summary>
        private CameraTuple topdownCamDesc = new CameraTuple()
        {
            position = new Vector3(-100f, 200f, 100f),
            target = Vector3.Zero,
            up = Vector3.Up
        };

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.IsFullScreen = true;

            randomGenerator = new Random();

            IsMouseVisible = true;
        }

        public void nextLevel()
        {
            levelNumber++;
            if (levelNumber >= MAX_LEVELS)
                levelNumber = 0;

            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            // Initialize sounds
            soundManager = new SoundManager(this);

            // Create the world
            world = new World(this);

            // Configure the camera as an FPS camera for the player
            camera = new TopdownCamera(this, topdownCamDesc);

            // Load the world
            world.hardcodedWorldPopulation(this, levelNumber);

            // Create a HUD that displays information about the game, and debug
            hud = new HUD(this);

            // Create a player at (-100,0,0)
            player = new Player(this, hud, Matrix.CreateTranslation(new Vector3(-150f, 0, 0)), world, world.towerManager);

            // Supply the Player to the world
            world.player = player;

            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load sounds
            soundManager.load(Content);

            // Load world content
            world.load(Content);

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
                player = this.player,
                world = this.world,
                random = randomGenerator,
                soundManager = this.soundManager,
                hud = this.hud
            };

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            // Toggle full-screen
            if (keyboard.IsKeyDown(Keys.F12))
                graphics.ToggleFullScreen();

            // Play the ambient noise
            if (soundManager.ambientInstance == null)
                soundManager.playAmbience(SoundManager.SoundNames.AMBIENCE_TOWN);

            // Select the type of camera:
            changeCamera(keyboard);

            // Update primary camera
            camera.Update(gameTime);

            // Update the player
            player.update(updateParams);

            // Update the world (entities and static models)
            world.update(updateParams);

            // Update UI message
            hud.update(updateParams);

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
                mouseState = mouse,
                graphicsDevice = this.GraphicsDevice
            };

            // Draw the world
            world.draw(drawParams);

            // Draw the player
            player.draw(drawParams);

            // Draw a UI
            hud.draw(drawParams, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
