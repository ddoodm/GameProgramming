﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
    public class DPlane : Primitive<VertexPositionNormalTexture>
    {
        public Plane plane { get; private set; }
        public Vector3 N { get; private set; }
        public Vector3 T { get; private set; }
        public Vector3 B { get; private set; }

        public DPlane(Game game, float size, Vector3 up)
            : base(game, size)
        {
            plane = new Plane(up, 0);

            texture = game.Content.Load<Texture2D>("Textures\\concreteNew");

            build(size);
        }

        protected virtual float getTexSize(float modelSize)
        {
            return modelSize / 32f;
        }

        protected override void build(float sz)
        {
            // Texture size
            float texSz = getTexSize(sz);

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
            Vector3 szT = T * sz;
            Vector3 szB = B * sz;

            v_data = new VertexPositionNormalTexture[4]
            {
                new VertexPositionNormalTexture( P - szT - szB, N, new Vector2(0,        0) ),
                new VertexPositionNormalTexture( P + szT - szB, N, new Vector2(texSz,    0) ),
                new VertexPositionNormalTexture( P - szT + szB, N, new Vector2(0,        texSz) ),
                new VertexPositionNormalTexture( P + szT + szB, N, new Vector2(texSz,    texSz) ),
            };
        }

        public override void draw(GameTime gameTime)
        {
            base.draw(gameTime);

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

    public class PlasmaPlane : DPlane
    {
        private Effect shader;

        public PlasmaPlane(Game game, float size, Vector3 up)
            : base(game, size, up)
        {
            shader = game.Content.Load<Effect>("Shaders\\plasma");
        }

        protected override float getTexSize(float modelSize)
        {
            return 1f;
        }

        public override void draw(GameTime gameTime)
        {
            shader.Parameters["World"].SetValue(world);
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
                    0, 2);
            }
        }
    }
}
