using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
    public class Primitive<Vertex> where Vertex : struct
    {
        protected Game1 game;                                 // Handle to game object
        protected float size;
        protected Vector3 pos = new Vector3();
        protected Matrix world;                               // World translation matrix
        protected Texture2D texture;                          // Array of textures
        protected Vertex[] v_data;                            // Vertex data
        protected VertexBuffer vbo;                           // Buffer object
        protected BasicEffect shader;                         // Stock shader
        protected Vector3 lightPos;

        public Primitive(Game game, float size)
        {
            this.game = (Game1)game;
            this.size = size;

            build(size);
            init();
        }

        public Primitive(Game game, float size, Vector3 pos)
        {
            this.game = (Game1)game;
            this.pos = pos;
            this.size = size;

            init();
        }

        private void init()
        {
            // Create Vertex Buffer Object
            vbo = new VertexBuffer(game.GraphicsDevice, typeof(Vertex), v_data.Length, BufferUsage.None);
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

        public virtual void draw(GameTime gameTime)
        {
            // Bind the vertex buffer
            game.GraphicsDevice.SetVertexBuffer(vbo);

            shader.View = game.camera.view;             // View transform matrix
            shader.Projection = game.camera.projection; // Perspective projection matrix

            // Bind texture to shader
            shader.Texture = texture;
        }
    }
}
