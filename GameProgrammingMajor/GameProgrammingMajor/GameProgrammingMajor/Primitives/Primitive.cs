using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Primitive<Vertex> where Vertex : struct
    {
        public Matrix world;                    // World translation matrix
        protected Vector3 pos = new Vector3();  // Position in Cartesian space

        protected Game game;                    // Handle to game object
        protected float size;
        protected Vertex[] v_data;              // Vertex data
        protected VertexBuffer vbo;             // Buffer object
        protected BasicEffect shader;           // Stock shader

        /// <summary>
        /// The diffuse colour of the primitive.
        /// </summary>
        public Vector3 diffuseColour = Vector3.One;

        /// <summary>
        /// The specular colour of the primitive
        /// </summary>
        public Vector3 specularColour = new Vector3(0.25f);

        /// <summary>
        /// The number of times to wrap the texture.
        /// Use Vector2.One to fill the texture.
        /// Default: 1x1
        /// </summary>
        public Vector2 textureTiling = Vector2.One;

        /// <summary>
        /// Indicates whether the primitive has been built yet.
        /// </summary>
        public bool isBuilt { get; protected set; }

        public Texture2D texture;

        public Primitive(Game game, float size)
        {
            this.game = game;
            this.size = size;

            v_data = new Vertex[1];

            init();
        }

        public Primitive(Game game, float size, Vector3 pos)
            : this(game, size)
        {
            this.pos = pos;
        }

        private void init()
        {
            // Create Vertex Buffer Object
            vbo = new VertexBuffer(game.GraphicsDevice, typeof(Vertex), v_data.Length, BufferUsage.None);
            vbo.SetData(v_data);

            // Initialize the stock shader
            shader = new BasicEffect(game.GraphicsDevice);

            // Configure default lighting
            shader.EnableDefaultLighting();
        }

        public virtual void build()
        {

        }

        public virtual void update(UpdateParams updateParams)
        {
            world = Matrix.Identity;
        }

        public virtual void draw(DrawParams drawParams)
        {
            // Bind the vertex buffer
            game.GraphicsDevice.SetVertexBuffer(vbo);

            shader.World = world;
            shader.View = drawParams.camera.view;             // View transform matrix
            shader.Projection = drawParams.camera.projection; // Perspective projection matrix

            // Bind texture to shader
            shader.Texture = texture;
        }
    }
}
