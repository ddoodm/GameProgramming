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

namespace _11697822_lab06
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;

        BasicEffect effect;
        Skybox skybox;
        Ground ground;
        Tank tank;
        Tank pursuer;
        Plane plane;

        MouseState mouseState;

        Vector3? target, pursuitTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(this, new Vector3(13 * 10, 70f * 10, -8 * 10),
                new Vector3(12 * 10, 0 * 10, -8 * 10), Vector3.Up);
            Components.Add(camera);            

            effect = new BasicEffect(GraphicsDevice);

            mouseState = Mouse.GetState();

            plane = new Plane(Vector3.Up, 0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D[] skyboxTextures;
            Model skyboxModel = loadModel(@"Skybox/skybox", out skyboxTextures);

            Texture2D[] groundTextures;
            Model groundModel = loadModel(@"Ground/ground", out groundTextures);

            skybox = new Skybox(this, skyboxModel, skyboxTextures);
            ground = new Ground(this, groundModel, groundTextures);

            //Components.Add(ground);

            tank = new Tank();
            tank.Load(Content);
            tank.rotation = Matrix.CreateRotationY(0f);

            pursuer = new Tank();
            pursuer.Load(Content);
            pursuer.world *= Matrix.CreateTranslation(100f, 0, 100f);
            pursuer.rotation = Matrix.CreateRotationY(0f);
            pursuer.speed = 50;
        }

        private Model loadModel (string assetName, out Texture2D[] textures)
        {
            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                foreach(BasicEffect currentEffect in mesh.Effects)
                {
                    textures[i++] = currentEffect.Texture;
                }
                
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
            return newModel;

        }

        //Vector2 mousePicking()

        /*public Vector3 unproject(Camera camera)
        {
            
        }*/

        public Vector3? mousePicking()
        {
            Vector3 nearSource = new Vector3(Mouse.GetState().X, Mouse.GetState().Y, 1f);
            Vector3 farSource = new Vector3(Mouse.GetState().X, Mouse.GetState().Y, 1000.0f);

            //Matrix world = Matrix.CreateTranslation(0, 1, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
                camera.projection, camera.view, Matrix.Identity);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
                camera.projection, camera.view, Matrix.Identity);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            Ray r = new Ray(nearPoint, direction);

            float? distance = r.Intersects(plane);
            float denom = Vector3.Dot(plane.Normal, r.Direction);
            float num = Vector3.Dot(plane.Normal, r.Position) + plane.D;
            float t = -(num / denom);

            Vector3 picked = nearPoint + direction * t;


            //Console.WriteLine(distance.ToString());
            //distance.ToString();

            //if (r.Intersects(p) != null)
            //    tank.moveToTarget();

            //return nearPoint + direction * distance;
            return picked;
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                target = mousePicking();
                tank.target = (Vector3)target;
                //tank.switchState(Tank.AIState.SEEK, gameTime);
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                pursuer.switchState(Tank.AIState.PURSUE, gameTime);
                pursuitTarget = tank.calcPursuit(tank.position, tank.velocity, gameTime); //+tank.velocity * tank.timeToTarget;
                pursuer.target = (Vector3)pursuitTarget;
                pursuer.moveToTarget(pursuer.target, gameTime);
                pursuer.rotateToTarget(); //= tank.position + tank.velocity * pursuer.timeToTarget;
                
                //pursuer.pursue(tank.position, tank.velocity, gameTime);
            }

            if (tank.collided(tank, pursuer))
            {
                //tank.moveToTarget(tank.position, gameTime);
                pursuer.moveToTarget(pursuer.position, gameTime);
            }

            //target = mousePicking();

            if (target.HasValue)
            {
                tank.moveToTarget((Vector3)target, gameTime);
                tank.rotateToTarget();
                pursuer.moveToTarget(pursuer.target, gameTime);
                pursuer.rotateToTarget();
                //pursuer.pursue(tank.position, tank.velocity, gameTime);
            }

            //pursuer.moveToTarget(pursuer.target, gameTime);
            pursuer.target = tank.position + tank.velocity * tank.timeToTarget;
            pursuer.rotateToTarget();

            //Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

            //pursuer.pursue(tank.position, tank.velocity, gameTime);

            skybox.Update(gameTime);
            ground.Update(gameTime);
            
            //int mouseX = mouseState.X;
            //int mouseY = mouseState.Y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skybox.Draw(GraphicsDevice, camera);
            ground.Draw(gameTime);
            tank.Draw(tank.world, camera.view, camera.projection);
            pursuer.Draw(pursuer.world, camera.view, camera.projection);

            tank.switchState(Tank.AIState.IDLE, gameTime);
            pursuer.switchState(Tank.AIState.IDLE, gameTime);

            this.IsMouseVisible = true;

            base.Draw(gameTime);
        }
    }
}