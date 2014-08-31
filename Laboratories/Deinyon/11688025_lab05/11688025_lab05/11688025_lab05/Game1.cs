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

namespace _11688025_lab05
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont uiFont;

        // ModelManager for static model handling
        public ModelManager modelManager;
        public ProjectileManager projectileManager;

        public Light sun;
        public Camera camera;
        public DPlane plane;
        public Tank tank;

        private WaypointManager waypointMan;
        //private BoingyBall ball;
        private Skybox skybox;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            modelManager = new ModelManager(this);

            waypointMan = new WaypointManager(this);

            // Camera's up vector is positive Z, because we have a "bird's eye" angle
            camera = new Camera(this, new Vector3(0, 275, 0), Vector3.Zero, new Vector3(0, 0, 1));
            Components.Add(camera);

            plane = new DPlane(this, 400, Vector3.Up);

            sun = new Light(new Vector3(100, 100, 100));

            tank = new Tank(this);

            projectileManager = new ProjectileManager(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            modelManager.LoadContent();

            projectileManager.loadContent(this);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            skybox = new Skybox(this, Content.Load<Model>("Models\\DSkyboxMesh"));

            uiFont = Content.Load<SpriteFont>("Font\\UIFont");

            tank.load(Content);
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
            // Get peripheral states
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            keyboardUpdate(keyboard);

            // Manual update is required for runtime Camera type changes
            camera.Update(gameTime);

            // Update waypoint selector
            waypointMan.update(gameTime);

            projectileManager.update(gameTime);

            // Update models
            modelManager.Update(gameTime);
            plane.update(gameTime);
            tank.update(gameTime, waypointMan, mouse);
            skybox.update(gameTime);

            base.Update(gameTime);
        }

        private void keyboardUpdate(KeyboardState ks)
        {
            // Static bird's-eye camera
            if (ks.IsKeyDown(Keys.D1))
                camera = new Camera(this, new Vector3(0, 275, 0), Vector3.Zero, new Vector3(0, 0, 1));

            // Flying camera
            else if (ks.IsKeyDown(Keys.D2))
                camera = new FlyingCamera(this, new Vector3(0, 25, 200), new Vector3(0, 25, 0), Vector3.Up);

            // FPS-style camera
            else if (ks.IsKeyDown(Keys.D3))
                camera = new FPCamera(this, new Vector3(0, 25, 50), new Vector3(0, 25, 0), Vector3.Up);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            waypointMan.draw(camera);
            projectileManager.draw(camera);

            plane.draw();
            tank.draw(camera, sun, plane.plane);
            skybox.draw(camera);

            modelManager.Draw(gameTime);

            drawUI();

            base.Draw(gameTime);
        }

        private void drawUI()
        {
            Type camType = camera.GetType();
            string message = string.Format("{0}(1) Top-Down | {1}(2) Flying-Cam | {2}(3) FPS-Cam\n(LMB) Set Waypoint | (RMB) Fire",
                camType.Equals(typeof(Camera)) ? '*' : ' ',
                camType.Equals(typeof(FlyingCamera)) ? '*' : ' ',
                camType.Equals(typeof(FPCamera)) ? '*' : ' ');

            spriteBatch.Begin();
            for (int i = 0; i < 2; i++ )
            {
                spriteBatch.DrawString(
                    uiFont,                                 // Font object
                    message,                                // String
                    new Vector2(20 - i * 2, 20 - i * 2),    // Position
                    i == 0 ? Color.Black : Color.White);
            }
            spriteBatch.End();

            // Restore GD states after spriteBatch draw
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap; 
        }
    }
}
