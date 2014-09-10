using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class StaticModel : Entity
    {
        public Model        model { get; protected set; }

        public StaticModel(Game game, Model model)
            : base(game)
        {
            this.model = model;
        }

        public StaticModel(Game game, Model model, Vector3 position)
            : this(game, model)
        {
            kinematic.position = position;

            world *= Matrix.CreateTranslation(position);
        }

        public StaticModel(Game game, Model model, Matrix world)
            : this(game, model)
        {
            kinematic.position = world.Translation;
            this.world = world;
        }

        // Copy constructor
        public StaticModel(StaticModel rhs)
            : base(rhs.game, rhs.world)
        {
            this.model = rhs.model;
        }

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);
        }

        public virtual void draw(DrawParams drawParams)
        {
            // Per-mesh transform matrices
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw each mesh in model
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Provide MVP matrices to each shader of each mesh
                foreach (BasicEffect shader in mesh.Effects)
                {
                    // This method should be overridden; only default lighting is provided. 
                    shader.EnableDefaultLighting();
                    shader.DiffuseColor = Vector3.One;
                    shader.Projection = drawParams.camera.projection;
                    shader.View = drawParams.camera.view;
                    shader.World = mesh.ParentBone.Transform * world;
                }

                mesh.Draw();
            }
        }
    }
}
