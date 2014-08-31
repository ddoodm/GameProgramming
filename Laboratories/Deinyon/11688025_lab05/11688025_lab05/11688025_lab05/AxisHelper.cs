using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab05
{
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
