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

namespace lab4._1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;//, fpsCamera, topdownCamera;
        ModelManager modelManager;
        Skybox skybox;
        Ground ground;
        MouseState preMouseState;
        KeyboardState preKeyboardState;
        SpriteFont font1;
        Cube marker;
        bool infodisplay = true;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            camera = new Fpscam(this, new Vector3(0, 10, 50), new Vector3(0, 0, 0), Vector3.Up);
            //topdownCamera = new Camera(this, new Vector3(0, 100, 0), new Vector3(0, 0, 0), Vector3.Forward);
            //camera = fpsCamera;
            Components.Add(camera);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            ground = new Ground(this, camera);
            Components.Add(ground);
            preMouseState = Mouse.GetState();
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

            skybox = new Skybox(this, @"Models\Skybox\skybox2");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyboard.IsKeyDown(Keys.I)&&!preKeyboardState.IsKeyDown(Keys.I)) infodisplay = !infodisplay;
            preKeyboardState = keyboard;
            
            MouseState currMouseState = Mouse.GetState();
            if (currMouseState.LeftButton == ButtonState.Pressed && preMouseState.LeftButton != ButtonState.Pressed)
            {
                Vector3? m = mousepicking();
                if (m != null)
                {
                    modelManager.addWayPoint((Vector3)m);
                    marker = new Cube(this, 1, (Vector3)m);
                }
            }
            preMouseState = currMouseState;
            base.Update(gameTime);
        }
        protected Vector3? mousepicking()
        {
            MouseState mouseState = Mouse.GetState();
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;
            Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
            Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource,
                camera.projection, camera.view, world);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource,
                camera.projection, camera.view, world);
            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);
            //ground plane can replace with boundings of colidable objects
            Plane plane = new Plane(Vector3.Up, 0);
            float? dist = pickRay.Intersects(plane);
            if (dist == null) return null;
            return nearPoint + direction * dist;

        } 
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            skybox.DrawSkybox();
            if (marker != null) marker.Draw();
            base.Draw(gameTime);
            if(infodisplay)
            {
                string output = " left click to place waypoint \n right click to ativate stalker tank \n c to toggle flight \n i to toggle information display";
                spriteBatch.Begin();
                spriteBatch.DrawString(font1, output, Vector2.Zero, Color.Red);
                spriteBatch.End();
            }
        }
    }
}
