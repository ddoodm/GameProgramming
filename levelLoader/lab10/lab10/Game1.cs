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

namespace lab10
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font1;
        bool infodisplay = false;
        KeyboardState preKeyboardState;

        //BasicModel model;
        //Cube cube;
        Level level;
        public Camera camera;
        Ground ground;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            camera = new Fpscam(this, new Vector3(60, 60, 60), new Vector3(40, 0, 0), Vector3.Up);
            Components.Add(camera);
            ground = new Ground(this, camera);
            Components.Add(ground);
            //cube = new Cube(this, 10 ,Vector3.Zero);
            int levelNum = 1;
            level = new Level(this, "./Levels/Level" + Convert.ToString(levelNum) + ".xml");
            preKeyboardState = Keyboard.GetState();

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
            font1 = Content.Load<SpriteFont>(@"Spritefont1");

            //model = new BasicModel(Content.Load<Model>(@"Models\spaceship"), new Vector3(0, 10, 0), 1);
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
            // Allows the game to exit
            KeyboardState keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyboard.IsKeyDown(Keys.I) && !preKeyboardState.IsKeyDown(Keys.I)) infodisplay = !infodisplay;

            level.Update(gameTime);
            /*MouseState currMouseState = Mouse.GetState();
            if (currMouseState.LeftButton == ButtonState.Pressed)// && preMouseState.LeftButton != ButtonState.Pressed)
            {
                Vector3? m = MouseHelper.mousepickingPlane(this, new Plane(Vector3.Up, -5));
                if (m != null)
                {
                    cube = new Cube(this, 5, (Vector3)m, "Textures/crate(2)");
                }
            }
            */
            preKeyboardState = keyboard;
            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw();
            //model.Draw(camera);
            //if(cube != null)cube.Draw();
            base.Draw(gameTime);
            if (infodisplay)
            {
                string output = " left click to place waypoint \n right click remove or add a wall \n c to toggle flight \n i to toggle information display \n wasd to move the camera \n space to go up";
                spriteBatch.Begin();
                spriteBatch.DrawString(font1, output, Vector2.Zero, Color.Black);
                spriteBatch.End();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }
    }
}
