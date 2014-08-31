using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _116880025_lab04
{
    public class StaticModel
    {
        protected Game      game;
        public Model        model { get; protected set; }
        public Matrix       world = Matrix.Identity;
        private AxisHelper  axis;

        public StaticModel(Game game, Model model)
        {
            this.game = game;
            this.model = model;

            axis = new AxisHelper(game, 32f);
        }

        // Copy constructor
        public StaticModel(StaticModel rhs)
        {
            this.game = rhs.game;
            this.model = rhs.model;
        }

        public virtual void update(GameTime gameTime)
        {
            axis.update(gameTime, world);
        }

        public virtual void draw(Camera camera)
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
                    //shader.EnableDefaultLighting();
                    shader.DiffuseColor = Vector3.One;
                    shader.Projection = camera.projection;
                    shader.View = camera.view;
                    shader.World = mesh.ParentBone.Transform * world;
                }

                mesh.Draw();
            }
        }

        public void draw_axis()
        {
            axis.draw();
        }

        public virtual float colSphereSize()
        {
            return -1f;
        }
    }
}
