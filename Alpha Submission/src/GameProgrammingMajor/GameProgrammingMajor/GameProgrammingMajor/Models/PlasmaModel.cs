using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    class PlasmaModel : StaticModel
    {
        private Effect plasma;

        public PlasmaModel(Game game, Model model)
            : base(game, model)
        {
            plasma = game.Content.Load<Effect>("Shaders\\plasma");
        }

        public PlasmaModel(Game game, Model model, Vector3 position)
            : base(game, model, position)
        {
            plasma = game.Content.Load<Effect>("Shaders\\plasma");
        }

        public override void draw(DrawParams drawParams)
        {
            // Per-mesh transform matrices
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw each mesh in model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = plasma;

                    part.Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                    part.Effect.Parameters["View"].SetValue(drawParams.camera.view);
                    part.Effect.Parameters["Projection"].SetValue(drawParams.camera.projection);
                    part.Effect.Parameters["time"].SetValue((float)drawParams.gameTime.TotalGameTime.TotalMilliseconds / 500f);
                }

                mesh.Draw();
            }
        }
    }
}
