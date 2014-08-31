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

namespace Lab2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        Vector2 angle = new Vector2(0f, 0f);
        Vector2 oldMouse = new Vector2(0f, 0f);

        float aspectRatio;

        BasicEffect cubeEffect;

        Cube cube = new Cube(new Vector3(5, 5, 5), new Vector3(0, 0, 0));

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
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            // TODO: Add your initialization logic here
            //aspectRatio for projection
            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            this.IsMouseVisible = true;

            base.Initialize();
            initWorld();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cube.cubeTexture = Content.Load<Texture2D>("crate");

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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //move the cube with mouse
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X < oldMouse.X - 2)
                {
                    angle.Y -= 2f;
                }
                else if (mouseState.X > oldMouse.X + 2)
                {
                    angle.Y += 2f;
                }

                if (mouseState.Y < oldMouse.Y - 2)
                {
                    angle.X -= 2f;
                }
                else if (mouseState.Y > oldMouse.Y + 2)
                {
                    angle.X += 2f;
                }

            }

            oldMouse.X = mouseState.X;
            oldMouse.Y = mouseState.Y;



            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set the World matrix 
            cubeEffect.World = Matrix.CreateRotationY(MathHelper.ToRadians(angle.Y)) * Matrix.CreateRotationX(MathHelper.ToRadians(angle.X));

            // Set the View matrix which 
            cubeEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);

            // Set the Projection matrix 
            cubeEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),  aspectRatio, 1.0f, 600.0f);

            // Enable textures on the Cube Effect.
            cubeEffect.TextureEnabled = true;
            cubeEffect.Texture = cube.cubeTexture;

            // Lighting
            cubeEffect.EnableDefaultLighting();

            // render the cube
            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                cube.RenderCube(GraphicsDevice);
            }


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


        public void initWorld()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), (float)aspectRatio, 1.0f, 600f);


            cubeEffect = new BasicEffect(graphics.GraphicsDevice);

            cubeEffect.World = worldMatrix;
            cubeEffect.View = viewMatrix;
            cubeEffect.Projection = projectionMatrix;

            cubeEffect.TextureEnabled = true;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
