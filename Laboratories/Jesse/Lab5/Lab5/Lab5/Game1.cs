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

namespace Lab5
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
        Skybox skybox;
        Model skyboxModel;
        Ground ground;
        Texture2D groundTex;

        Model tankModel;
        Tank tank;

        Cube cube;
        MouseState mouseStateCurrent, mouseStateOld;
        Vector3 selectedCoords;
        Vector3 targetDirection;
        float tankDistance;

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

            camera = new Camera(this, new Vector3(0f, 250f, 0f), new Vector3(1f, 1f, 0f), Vector3.Up);
            camera.Initialize();

            effect = new BasicEffect(graphics.GraphicsDevice);

            cube = new Cube(new Vector3(1, 1, 1), new Vector3(0, 1, 0));
            cube.Initialize(effect);
            selectedCoords = new Vector3(0f, 0f, 0f);

            IsMouseVisible = true;

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

            groundTex = Content.Load<Texture2D>("grass");

            tankModel = Content.Load<Model>("Tank/tank");
            tank = new Tank(this, tankModel);


            // TODO: use this.Content to load your game content here
        }


        private Model loadModel(String assetName, out Texture2D[] Textures)
        {
            Model newModel = Content.Load<Model>(assetName);
            Textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Textures[i++] = currentEffect.Texture;
                }
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
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

            camera.Update(GraphicsDevice.Viewport, cube, effect);

            mouseStateCurrent = Mouse.GetState();
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStateOld.LeftButton == ButtonState.Released)
            {
                Vector2 mousePos = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                Ray ray = camera.MouseRay(mousePos, GraphicsDevice.Viewport);


                Vector3 planeNormal = new Vector3(0f, 1f, 0f);
                Plane plane = new Plane(planeNormal, 0f);

                float? distance = ray.Intersects(plane);

                selectedCoords = ray.Position + ray.Direction * distance.Value;
                cube.cubePosition = selectedCoords;

                targetDirection = selectedCoords - tank.tankPosition;
                targetDirection.Normalize();


                Vector2 facingVec = new Vector2(tank.tankPosition.Z, tank.tankPosition.X) - new Vector2(selectedCoords.Z, selectedCoords.X);
                tank.tankRotation = (float)Math.Atan2(facingVec.Y, facingVec.X) + 1.575f + MathHelper.PiOver2;


               /* Vector3 D = (targetDirection - tank.tankPosition);
                Vector3 Right = Vector3.Cross(new Vector3(0f, 1f, 0f), D);
                Vector3.Normalize(ref Right, out Right);
                Vector3 Backwards = Vector3.Cross(Right, new Vector3(0f, 1f, 0f));
                Vector3.Normalize(ref Backwards, out Backwards);
                Vector3 Up = Vector3.Cross(Backwards, Right);
                tankDirection = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1);*/
                

            }
            mouseStateOld = Mouse.GetState();

            tankDistance = (selectedCoords - tank.tankPosition).Length();

            tank.Update(gameTime, targetDirection, tankDistance);

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
            tank.Draw(GraphicsDevice, camera);

            //if(selectedCoords != )



            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}




//www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Skybox.php