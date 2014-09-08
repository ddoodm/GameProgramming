using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
    public class StaticModel : Entity
    {
        public Model        model { get; protected set; }
        public float        colSphereSize = -1;

        public StaticModel(Game game, Model model)
            : base(game)
        {
            this.model = model;
        }

        public StaticModel(Game game, Model model, Vector3 position, float colSphereSize)
            : this(game, model)
        {
            kinematic.position = position;
            this.colSphereSize = colSphereSize;

            world *= Matrix.CreateTranslation(position);
        }

        // Copy constructor
        public StaticModel(StaticModel rhs)
            : base(rhs.game, rhs.world)
        {
            this.model = rhs.model;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public virtual void draw(Camera camera)
        {
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

                    shader.DiffuseColor = Vector3.One;
                    shader.Projection = camera.projection;
                    shader.View = camera.view;
                    shader.World = mesh.ParentBone.Transform * world;
                }

                mesh.Draw();
            }
        }

        /*
        public void draw_axis(GameTime gameTime)
        {
            axis.draw(gameTime);
        }
         */

        public virtual float getColSphereSize()
        {
            return colSphereSize;
        }
    }
}
