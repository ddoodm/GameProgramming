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

namespace Lab03_3DDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; protected set; }
        public Floor floor { get; protected set; }
        Cube cube;
        Texture2D tx_dTitle;

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
            /* Place the camera at (0,0,15), looking at (0,0,0),
             * append the Camera to the list of game components. */
            camera = new Camera(this, new Vector3(0, 0, 15), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            // Disable backface culling
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //graphics.GraphicsDevice.RasterizerState = rs;

            // Show mouse
            this.IsMouseVisible = true;

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

            cube = new Cube(this, 4.0f);
            floor = new Floor(this, 42f, new Vector3(0,-10f,0));

            // Load 'DDOODM' sprite
            tx_dTitle = Content.Load<Texture2D>("Textures\\dTitle");
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
                || Keyboard.GetState().IsKeyDown(Keys.Escape) )
                this.Exit();

            cube.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Must draw floor first so that shadow alpha blending can work
            floor.draw();
            cube.draw();

            // Draw title sprite
            spriteBatch.Begin();
            spriteBatch.Draw(tx_dTitle, Vector2.One, Color.White);
            spriteBatch.End();

            // Restore GD states after spriteBatch draw
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap; 

            base.Draw(gameTime);
        }
    }

    public class Primitive
    {
        protected Game1 game;                                 // Handle to game object
        protected Vector3 pos = new Vector3();
        protected Matrix world;                               // World translation matrix
        protected Texture2D texture;                          // Array of textures
        protected VertexPositionNormalTexture[] v_data;       // Vertex data
        protected VertexBuffer vbo;                           // Buffer object
        protected BasicEffect shader;                         // Stock shader
        protected Vector3 lightPos;

        public Primitive(Game1 game, float size)
        {
            this.game = game;

            build(size);
            init();
        }

        public Primitive(Game1 game, float size, Vector3 pos)
        {
            this.game = game;
            this.pos = pos;

            build(size);
            init();
        }

        private void init()
        {
            // Create Vertex Buffer Object
            vbo = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), v_data.Length, BufferUsage.None);
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
            v_data = new VertexPositionNormalTexture[4]
            {
                new VertexPositionNormalTexture(new Vector3(-sz, sz, sz), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(sz, sz, sz), new Vector3(0, 0, 1), new Vector2(.5f, 0)),
                new VertexPositionNormalTexture(new Vector3(-sz, -sz, sz), new Vector3(0, 0, 1), new Vector2(0, .5f)),
                new VertexPositionNormalTexture(new Vector3(sz, -sz, sz), new Vector3(0, 0, 1), new Vector2(.5f, .5f)),
            };
        }

        public virtual void update(GameTime gameTime)
        {
            // Update world transform matrix
            float theta = (float)gameTime.TotalGameTime.TotalMilliseconds * 0.001f;

            world = Matrix.Identity;
            world *= Matrix.CreateTranslation(new Vector3(5f * (float)Math.Sin(theta * 1.2f), 0, 0));
            world *= Matrix.CreateRotationY(theta);
            world *= Matrix.CreateRotationX(theta * 0.75f);
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

    public class Cube : Primitive
    {
        const int 
            NUM_TEXTUES = 4,
            NUM_ROWS = 2,
            NUM_COLS = 2;

        public Cube(Game1 game, float size)
            : base(game, size)
        {
            // Load textures
            texture = game.Content.Load<Texture2D>("Textures\\crateSheet");
        }

        protected override void build(float sz)
        {
            // Create face vertices:
            v_data = new VertexPositionNormalTexture[4 * NUM_TEXTUES];
            v_data[0] = new VertexPositionNormalTexture(new Vector3(-sz, sz, sz), new Vector3(0, 0, 1), new Vector2(0, 0));
            v_data[1] = new VertexPositionNormalTexture(new Vector3(sz, sz, sz), new Vector3(0, 0, 1), new Vector2(.5f, 0));
            v_data[2] = new VertexPositionNormalTexture(new Vector3(-sz, -sz, sz), new Vector3(0, 0, 1), new Vector2(0, .5f));
            v_data[3] = new VertexPositionNormalTexture(new Vector3(sz, -sz, sz), new Vector3(0, 0, 1), new Vector2(.5f, .5f));

            // Remaining three faces use alternative texture coordinates
            for (int i = 1; i < NUM_TEXTUES; i++)
            {
                int vi = i * 4;
                for (int j = 0; j < 4; j++)
                {
                    v_data[vi + j] = v_data[j];
                    v_data[vi + j].TextureCoordinate.X += (float)(i % NUM_ROWS) * 0.5f;
                    v_data[vi + j].TextureCoordinate.Y += (float)(i / NUM_COLS) * 0.5f;
                }
            }
        }

        public override void update(GameTime gameTime)
        {
            // Update world transform matrix
            float theta = (float)gameTime.TotalGameTime.TotalMilliseconds * 0.001f;

            world = Matrix.Identity;
            world *= Matrix.CreateTranslation(new Vector3(5f * (float)Math.Sin(theta*1.2f), 0, 0));
            world *= Matrix.CreateRotationY(theta);
            world *= Matrix.CreateRotationX(theta * 0.75f);

            // Scrollwheel scale
            float scale = 1 + Math.Max(-1, -Mouse.GetState().ScrollWheelValue * 0.00025f);
            world *= Matrix.CreateScale(scale);
        }

        public override void draw()
        {
            base.draw();

            // Draw solid
            // Configure the shader to display the texture
            shader.VertexColorEnabled = false;
            shader.TextureEnabled = true;
            shader.DiffuseColor = Vector3.One;
            draw_faces(Matrix.Identity);

            // Draw shadow projection
            // Configure the shader to display a solid colour
            shader.VertexColorEnabled = false;
            shader.TextureEnabled = false;
            shader.DiffuseColor = Vector3.Zero;
            // Enable alpha blending
            BlendState oldBs = game.GraphicsDevice.BlendState;
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            shader.Alpha = 0.5f;
            draw_faces(Matrix.CreateShadow(lightPos, game.floor.plane));
            game.GraphicsDevice.BlendState = oldBs;
            shader.Alpha = 1.0f;
        }

        private void draw_faces(Matrix shadow)
        {
            // Draw four faces (ring)
            for (int i = 0; i < 4; i++)
            {
                Matrix rot = Matrix.CreateRotationY((float)i / 4f * MathHelper.TwoPi);
                draw_face(rot, shadow, i);
            }

            // Draw caps
            for (int i = 0; i < 2; i++)
            {
                Matrix rot = Matrix.CreateRotationX(MathHelper.PiOver2 + (float)i / 2f * MathHelper.TwoPi);
                draw_face(rot, shadow, i);
            }
        }

        private void draw_face(Matrix rot, Matrix shadow, int texID)
        {
            shader.World = rot * world * shadow; // Move into world position, and the correct orientation

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    4*texID,                        // Vertex offset (x4 for face offset)
                    2);                             // Count
            }
        }
    }

    public class Floor : Primitive
    {
        public Plane plane { get; protected set; }

        public Floor(Game1 game, float size, Vector3 pos)
            : base(game, size, pos)
        {
            plane = new Plane(Vector3.Up, -pos.Y);

            // Load texture
            texture = game.Content.Load<Texture2D>("Textures\\brickFacade");
        }

        protected override void build(float sz)
        {
            // Create face vertices:
            float y = pos.Y;
            v_data = new VertexPositionNormalTexture[4]
            {
                new VertexPositionNormalTexture(new Vector3(-sz, y,-sz ), Vector3.Up, new Vector2(0,0)),
                new VertexPositionNormalTexture(new Vector3( sz, y,-sz ), Vector3.Up, new Vector2(5,0)),
                new VertexPositionNormalTexture(new Vector3(-sz, y, sz ), Vector3.Up, new Vector2(0,5)),
                new VertexPositionNormalTexture(new Vector3( sz, y, sz ), Vector3.Up, new Vector2(5,5)),
            };
        }

        public override void draw()
        {
            base.draw();

            // Disable backface culling, and restore later
            RasterizerState oldRs = game.GraphicsDevice.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            game.GraphicsDevice.RasterizerState = rs;

            shader.VertexColorEnabled = false;
            shader.TextureEnabled = true;

            // Translate up to avoid Z-fighting
            shader.World = Matrix.CreateTranslation(new Vector3(0, -0.01f, 0));

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    0,                              // Array offset
                    2);                             // Count
            }

            // Resture backface-cull state
            game.GraphicsDevice.RasterizerState = oldRs;
        }
    }
}
