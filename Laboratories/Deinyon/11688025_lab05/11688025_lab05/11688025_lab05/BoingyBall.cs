using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab05
{
    class BoingyBall : StaticModel
    {
        private const float SIZE = 16.0f;

        public BoingyBall(Game game, Model model)
            : base(game, model)
        {
        }

        public override void update(GameTime gameTime)
        {
            world = Matrix.Identity;
            world *= Matrix.CreateScale(SIZE);

            double theta = gameTime.TotalGameTime.TotalMilliseconds * 0.001;

            // Rotate
            world *= Matrix.CreateRotationY((float)theta) * Matrix.CreateRotationX((float)theta);

            // Bounce
            float y = SIZE + 60.0f * (float)Math.Abs(Math.Cos(3.0 * theta));
            // if (y-SIZE < Math.PI) ((Game1)game).camera.shake(5);

            // Rotate around the floor
            float x = 100.0f * (float)Math.Cos(theta);
            float z = 100.0f * (float)Math.Sin(theta);

            Vector3 position = new Vector3(x, y, z);

            DPlane plane = ((Game1)game).plane;
            position = Vector3.Transform(position, Matrix.CreateWorld(plane.T, plane.B, plane.N));
            world *= Matrix.CreateTranslation(position);

            base.update(gameTime);
        }

        public override void draw(Camera camera)
        {
            // Enable alpha blending
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Disable texture interpolation
            game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            // Per-mesh transform matrices
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Light sun = ((Game1)game).sun;

            // Draw each mesh in model
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Provide MVP matrices to each shader of each mesh
                foreach (BasicEffect shader in mesh.Effects)
                {
                    shader.LightingEnabled = true;
                    shader.DirectionalLight0.Direction = sun.direction;
                    shader.DirectionalLight0.DiffuseColor = sun.diffuse;
                    shader.DirectionalLight0.SpecularColor = sun.specular;
                    shader.SpecularPower = 128.0f;
                    shader.AmbientLightColor = sun.ambient;

                    shader.Alpha = 1.0f;
                    shader.DiffuseColor = Vector3.One;
                    shader.Projection = camera.projection;
                    shader.View = camera.view;
                    shader.World = world /* * mesh.ParentBone.Transform*/;
                }

                mesh.Draw();
            }

            // Revert to default sampler state
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            draw_shadow(camera, transforms, sun);
        }

        private void draw_shadow(Camera camera, Matrix[] transforms, Light sun)
        {
            // Compute shadow matrix
            Matrix shadow = Matrix.CreateShadow(sun.position, ((Game1)game).plane.plane);
            shadow *= Matrix.CreateTranslation(new Vector3(0, 1, 0));

            // Draw each mesh in model
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Provide MVP matrices to each shader of each mesh
                foreach (BasicEffect shader in mesh.Effects)
                {
                    //shader.EnableDefaultLighting();
                    shader.Alpha = 0.4f;
                    shader.DiffuseColor = Vector3.Zero;
                    shader.Projection = camera.projection;
                    shader.View = camera.view;
                    shader.World = world /* * mesh.ParentBone.Transform*/ * shadow;
                }

                mesh.Draw();
            }
        }

        public override float getColSphereSize()
        {
            return SIZE;
        }
    }
}
