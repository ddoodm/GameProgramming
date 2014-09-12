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
        public Model                model { get; protected set; }
        public BoundingSphere[]     boundingSpheres;
        public bool                 noCollision = false;

        public StaticModel(Game game, Model model)
            : base(game)
        {
            this.model = model;

            boundingSpheres = new BoundingSphere[model.Meshes.Count];
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

        /// <summary>
        /// Determines whether a model is colliding with this model
        /// </summary>
        /// <param name="other">The model to check collisions with</param>
        /// <returns>Whether there is a collision</returns>
        public bool collidesWith(StaticModel other)
        {
            if (this.noCollision)
                return false;

            foreach (ModelMesh otherMesh in other.model.Meshes)
                if (this.collidesWith(otherMesh.BoundingSphere))
                    return true;
            return false;
        }

        /// <summary>
        /// Determines whether this mesh collides with a bounding sphere
        /// </summary>
        public bool collidesWith(BoundingSphere otherSphere)
        {
            if (this.noCollision)
                return false;

            foreach (BoundingSphere thisSphere in boundingSpheres)
                if (thisSphere.Intersects(otherSphere))
                    return true;
            return false;
        }

        public override void update(UpdateParams updateParams)
        {
            // Obtain model bounding spheres and transform them
            for(int i=0; i<model.Meshes.Count; i++)
                boundingSpheres[i] = model.Meshes[i].BoundingSphere.Transform(world);

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
