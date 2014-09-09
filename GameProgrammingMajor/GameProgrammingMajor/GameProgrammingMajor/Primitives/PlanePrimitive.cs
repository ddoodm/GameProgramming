using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class PlanePrimitive : Primitive<VertexPositionNormalTexture>
    {
        public Plane plane { get; private set; }    // The XNA plane that is an analytic representation of the plane.
        public Vector3 N { get; private set; }      // The normal to the plane
        public Vector3 T { get; private set; }      // The tangent to the plane, which is arbitrary for bitangent computation
        public Vector3 B { get; private set; }      // The bitangent to the plane, which is used to define the coordinate system

        /// <summary>
        /// The number of times to wrap the texture.
        /// Use Vector2.One to fill the texture.
        /// Default: 8x8
        /// </summary>
        public Vector2 tiling = new Vector2(8, 8);

        public PlanePrimitive(Game game, float size, Vector3 up)
            : base(game, size)
        {
            plane = new Plane(up, 0);
        }

        public override void build()
        {
            /* In order to find the coordinate system approximated by N, 
             * I cross an arbitrary vector with the plane's normal to
             * find the tangent, and cross the tangent with the normal
             * to find the bitangent. */
            Vector3 X = new Vector3(1, 0, 0);
            N = plane.Normal;
            T = Vector3.Cross(X, N);
            B = Vector3.Cross(T, N);

            /* Then, I build an arbitrary quad on the coordinate system's
             * 'X' and 'Z' axes.*/
            Vector3 P = -N * plane.D;
            Vector3 szT = T * size;
            Vector3 szB = B * size;

            v_data = new VertexPositionNormalTexture[4]
            {
                new VertexPositionNormalTexture( P - szT - szB, N, new Vector2(0,        0) ),
                new VertexPositionNormalTexture( P + szT - szB, N, new Vector2(tiling.X, 0) ),
                new VertexPositionNormalTexture( P - szT + szB, N, new Vector2(0,        tiling.Y) ),
                new VertexPositionNormalTexture( P + szT + szB, N, new Vector2(tiling.X, tiling.Y) ),
            };

            // Update the "Built" flag to true.
            isBuilt = true;
        }

        public override void draw(EntityDrawParams drawParams)
        {
            if (!isBuilt)
                throw new Exception("You must first call build() before drawing this plane.");

            base.draw(drawParams);

            shader.VertexColorEnabled = false;
            shader.LightingEnabled = true;
            shader.TextureEnabled = true;

            shader.DiffuseColor = Vector3.One;
            shader.SpecularColor = new Vector3(0.25f);

            shader.World = world;

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    0, 2);
            }
        }
    }

    public class PlasmaPlane : PlanePrimitive
    {
        private Effect shader;

        public PlasmaPlane(Game game, float size, Vector3 up)
            : base(game, size, up)
        {
            shader = game.Content.Load<Effect>("Shaders\\plasma");
        }

        public override void draw(EntityDrawParams drawParams)
        {
            shader.Parameters["World"].SetValue(world);
            shader.Parameters["View"].SetValue(drawParams.camera.view);
            shader.Parameters["Projection"].SetValue(drawParams.camera.projection);
            shader.Parameters["time"].SetValue((float)drawParams.gameTime.TotalGameTime.TotalMilliseconds / 500f);

            // Draw VBO for multiple shader passes
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleStrip,    // Primitive type
                    v_data,                         // Array of vertices
                    0, 2);
            }
        }
    }
}
