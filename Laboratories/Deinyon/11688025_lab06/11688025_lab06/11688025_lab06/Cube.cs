using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
    public class Cube : Primitive<VertexPositionNormalTexture>
    {
        protected Vector3 position;

        public BoundingBox boundingBox;

        const int
            NUM_TEXTUES = 4,
            NUM_ROWS = 2,
            NUM_COLS = 2;

        public Cube(Game game, Vector3 position, float size)
            : base(game, size)
        {
            this.position = position;

            // Load textures
            texture = game.Content.Load<Texture2D>("Textures\\checker");

            // Configure bounding box
            Vector3 max = position + Vector3.One * size;
            Vector3 min = position - Vector3.One * size;
            boundingBox = new BoundingBox(min, max);
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
            base.update(gameTime);

            world *= Matrix.CreateTranslation(position);
        }

        public override void draw(GameTime gameTime)
        {
            base.draw(gameTime);

            // Draw solid
            // Configure the shader to display the texture
            shader.VertexColorEnabled = false;
            shader.TextureEnabled = true;
            shader.DiffuseColor = Vector3.One;
            draw_faces();
        }

        private void draw_faces()
        {
            // Draw four faces (ring)
            for (int i = 0; i < 4; i++)
            {
                Matrix rot = Matrix.CreateRotationY((float)i / 4f * MathHelper.TwoPi);
                draw_face(rot, i);
            }

            // Draw caps
            for (int i = 0; i < 2; i++)
            {
                Matrix rot = Matrix.CreateRotationX(MathHelper.PiOver2 + (float)i / 2f * MathHelper.TwoPi);
                draw_face(rot, i);
            }
        }

        protected virtual void draw_face(Matrix rot, int texID)
        {
            shader.World = rot * world; // Move into world position, and the correct orientation

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    4 * texID,                      // Vertex offset (x4 for face offset)
                    2);                             // Count
            }
        }
    }

    public class PlasmaCube : Cube
    {
        private Effect shader;
        private GameTime gameTime;

        public PlasmaCube(Game game, Vector3 position, float size)
            : base(game, position, size)
        {
            shader = game.Content.Load<Effect>("Shaders\\plasma");
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            this.gameTime = gameTime;
        }

        protected override void draw_face(Matrix rot, int texID)
        {
            shader.Parameters["World"].SetValue(rot * world);
            shader.Parameters["View"].SetValue(game.camera.view);
            shader.Parameters["Projection"].SetValue(game.camera.projection);
            shader.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds / 500f);

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    4 * texID,
                    2);
            }
        }
    }
}
