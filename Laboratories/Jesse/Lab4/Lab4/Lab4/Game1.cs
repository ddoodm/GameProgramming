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

namespace Lab4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        BasicEffect effect;

        Texture2D[] skyboxTextures;
        Texture2D[] groundTextures;
        Skybox skybox;
        Model skyboxModel;
        Ground ground;
        Texture2D groundTex;

        Cube cube;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            camera = new Camera(this, new Vector3(0f, 10f, 0f), new Vector3(1f, 10f, 0f), Vector3.Up);
            camera.Initialize();

            effect = new BasicEffect(graphics.GraphicsDevice);

            cube = new Cube(new Vector3(1, 1, 1), new Vector3(10, 1, 0));
            cube.Initialize(effect);


            IsMouseVisible = false;

            ground = new Ground(this);
            ground.Initialize(GraphicsDevice, effect);


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

            cube.cubeTexture = Content.Load<Texture2D>("crate");

            skyboxModel = loadModel(@"Skybox/skybox", out skyboxTextures);
            skybox = new Skybox(this, skyboxModel, skyboxTextures);

            groundTex = Content.Load<Texture2D>("groundModel/3SNe");

            // TODO: use this.Content to load your game content here
        }


        private Model loadModel(String assetName, out Texture2D[] Textures)
        {
            Model newModel = Content.Load<Model>(assetName);
            Textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach(ModelMesh mesh in newModel.Meshes)
            {
                foreach(BasicEffect currentEffect in mesh.Effects)
                {
                    Textures[i++] = currentEffect.Texture;
                }
                foreach(ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }

            return newModel;
            
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            camera.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skybox.Draw(GraphicsDevice, camera);
            ground.Draw(GraphicsDevice, camera, groundTex);
            cube.Draw(GraphicsDevice, camera);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}




//www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Skybox.php