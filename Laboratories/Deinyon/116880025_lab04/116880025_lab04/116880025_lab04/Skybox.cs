using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _116880025_lab04
{
    class Skybox : StaticModel
    {
        public Skybox(Game game, Model model)
            : base(game, model)
        {

        }

        public override void draw(Camera camera)
        {
            // Use linear clamp to remove borders
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            // Per-mesh transform matrices
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw each mesh in model
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Provide MVP matrices to each shader of each mesh
                foreach (BasicEffect shader in mesh.Effects)
                {
                    // 100% liminosity
                    shader.DiffuseColor = Vector3.One;

                    shader.Projection = camera.projection;

                    // Translate back to camera center for infinite-depth illusion
                    shader.View = camera.view;
                    shader.World = mesh.ParentBone.Transform * Matrix.CreateTranslation(camera.eye);
                }

                mesh.Draw();
            }

            // Reset sampling mode to default
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
