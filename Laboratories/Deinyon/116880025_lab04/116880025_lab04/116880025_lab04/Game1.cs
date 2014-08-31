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

namespace _116880025_lab04
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public ModelManager modelManager;

        public Camera camera { get; protected set; }
        public Light sun { get; protected set; }

        private ProjectileManager projectileMan;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // 16:9 aspect ratio
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 30, 50), new Vector3(0, 25, 0), Vector3.Up);
            Components.Add(camera);

            sun = new Light(new Vector3(100, 100, 100));

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            projectileMan = new ProjectileManager(this);

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

            projectileMan.loadContent(this);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                projectileMan.shoot(camera);

            projectileMan.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            projectileMan.draw(camera);

            base.Draw(gameTime);
        }
    }

    public class ProjectileManager
    {
        private Game game;
        private List<Projectile> projectiles;
        private List<StaticModel> targets;
        private StaticModel model;
        private float cooldown = 0;

        public ProjectileManager(Game game)
        {
            this.game = game;
            projectiles = new List<Projectile>();
            targets = ((Game1)game).modelManager.models;
        }

        public void loadContent(Game game)
        {
            model = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));
        }

        public void shoot(Camera camera)
        {
            if (cooldown <= 0)
            {
                Projectile p = new Projectile(new StaticModel(model), camera.eye, camera.direction);
                projectiles.Add(p);
                cooldown = p.cooldown;
            }
            else
                cooldown -= 1f;
        }

        public void update(GameTime gameTime)
        {
            for(int i=0;i<projectiles.Count;i++)
            {
                projectiles[i].update(gameTime);

                if (projectiles[i].screenTime >= 250f)
                {
                    projectiles.Remove(projectiles[i]);
                    continue;
                }

                if (projectiles[i].collision_test(targets))
                {
                    ((Game1)game).camera.shake(25);
                    projectiles.Remove(projectiles[i]);
                    continue;
                }
            }
        }

        public void draw(Camera camera)
        {
            foreach (Projectile p in projectiles)
                p.draw(camera);
        }

        public class Projectile
        {
            private const float SPEED = 0.2f;

            private StaticModel model;
            private Vector3 start;
            private Vector3 direction;

            public float cooldown = 5f;
            public float screenTime = 0;
            public float creationTime = 0;

            public Projectile(StaticModel model, Vector3 start, Vector3 direction)
            {
                this.model = model;
                this.start = start;
                this.direction = direction;
            }

            public void update(GameTime gameTime)
            {
                float bulletTime = (float)gameTime.TotalGameTime.TotalMilliseconds * SPEED;

                if (creationTime == 0)
                    creationTime = bulletTime;
                screenTime = bulletTime - creationTime;

                model.world = Matrix.Identity;
                model.world *= Matrix.CreateTranslation(start + screenTime * direction);
            }

            public bool collision_test(List<StaticModel> targets)
            {
                foreach (StaticModel target in targets)
                {
                    if (target.colSphereSize() <= 0)
                        continue;

                    // Collision with current target is possible
                    // Get distance from projectile to target
                    // by computing delta magnitude
                    Vector3 toTarget = target.world.Translation - model.world.Translation;
                    float distance = toTarget.Length();

                    if (Math.Abs(distance) <= target.colSphereSize())
                    {
                        //target.collisionResponse();
                        return true;
                    }
                }

                return false;
            }

            public void draw(Camera camera)
            {
                model.draw(camera);
            }
        }
    }

    public class Light
    {
        public Vector3 position {get; set;}
        public Vector3 ambient = new Vector3(0.5f, 0.45f, 0.35f);
        public Vector3 diffuse = new Vector3(0.95f, 0.56f, 0.40f);
        public Vector3 specular = new Vector3(.98f, .90f, .89f);

        public Vector3 direction
        {
            get { return Vector3.Normalize(-position); }
        }

        public Light(Vector3 position)
        {
            this.position = position;
        }
    }

    public class Primitive <T> where T : struct
    {
        protected Game1 game;                                 // Handle to game object
        protected Vector3 pos = new Vector3();
        protected Matrix world;                               // World translation matrix
        protected Texture2D texture;                          // Array of textures
        protected T[] v_data;       // Vertex data
        protected VertexBuffer vbo;                           // Buffer object
        protected BasicEffect shader;                         // Stock shader
        protected Vector3 lightPos;

        public Primitive(Game game, float size)
        {
            this.game = (Game1)game;

            build(size);
            init();
        }

        public Primitive(Game game, float size, Vector3 pos)
        {
            this.game = (Game1)game;
            this.pos = pos;

            build(size);
            init();
        }

        private void init()
        {
            // Create Vertex Buffer Object
            vbo = new VertexBuffer(game.GraphicsDevice, typeof(T), v_data.Length, BufferUsage.None);
            vbo.SetData(v_data);

            // Initialize the stock shader
            shader = new BasicEffect(game.GraphicsDevice);

            // Configure lighting
            lightPos = new Vector3(5f, 10f, 5f);
            shader.SpecularPower = 128f;
            shader.PreferPerPixelLighting = true;   // Use Phong shading
            shader.LightingEnabled = true; // turn on the lighting subsystem.
            shader.DirectionalLight0.Direction = Vector3.Normalize(-lightPos); // Reflect
            shader.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            shader.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
            shader.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
        }

        protected virtual void build(float sz)
        {

        }

        public virtual void update(GameTime gameTime)
        {
            world = Matrix.Identity;
        }

        public virtual void draw()
        {
            // Bind the vertex buffer
            game.GraphicsDevice.SetVertexBuffer(vbo);

            shader.View = game.camera.view;             // View transform matrix
            shader.Projection = game.camera.projection; // Perspective projection matrix

            // Bind texture to shader
            shader.Texture = texture;
        }
    }

    public class AxisHelper : Primitive<VertexPositionColor>
    {
        public AxisHelper(Game game, float size)
            : base(game, size)
        {

        }

        protected override void build(float sz)
        {
            v_data = new VertexPositionColor[6]
            {
                new VertexPositionColor(Vector3.Zero, Color.Red),
                new VertexPositionColor(new Vector3(sz, 0, 0), Color.Red),

                new VertexPositionColor(Vector3.Zero, Color.LightGreen),
                new VertexPositionColor(new Vector3(0, sz, 0), Color.Green),

                new VertexPositionColor(Vector3.Zero, Color.Blue),
                new VertexPositionColor(new Vector3(0, 0, sz), Color.Blue),
            };
        }

        public void update(GameTime gameTime, Matrix subject)
        {
            base.update(gameTime);

            // Do not carry scale or rotation
            world *= Matrix.CreateTranslation(subject.Translation);
        }

        public override void draw()
        {
            base.draw();

            shader.LightingEnabled = false;
            shader.VertexColorEnabled = true;
            shader.DiffuseColor = Vector3.One;
            shader.World = world;

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,    // Primitive type
                    v_data,                    // Array of vertices
                    0, 3);
            }
        }
    }
}
