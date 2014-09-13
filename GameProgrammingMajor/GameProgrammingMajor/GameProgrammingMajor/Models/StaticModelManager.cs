using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Stores and manages a list of static geometry.
    /// </summary>
    public class StaticModelManager
    {
        /// <summary>
        /// The list of static models to draw and update. 
        /// </summary>
        public List<StaticModel> models;

        /// <summary>
        /// Add a model to the Static Model Manager
        /// </summary>
        /// <param name="model">The static model to add.</param>
        /// <returns>A handle to the static model that was added.</returns>
        public StaticModel add(StaticModel model)
        {
            models.Add(model);
            return model;
        }

        public StaticModelManager()
        {
            models = new List<StaticModel>();
        }

        /// <summary>
        /// Finds the nearest bounding sphere to the vector "position"
        /// </summary>
        /// <param name="position">The position of the collider</param>
        /// <param name="ahead">The vector that points in front of the collider</param>
        /// <returns>The bounding sphere that is nearest to the vector.</returns>
        public BoundingSphere? findNearestCollisionSphere(Vector3 position, Vector3 ahead, float aheadDistance)
        {
            BoundingSphere? nearestSphere = null;

            // Convert the "Ahead" vector to a Ray
            Ray feeler = new Ray(position, Vector3.Normalize(ahead));

            foreach (StaticModel model in this.models)
            {
                // Ignore non-collidable models
                if (model.noCollision)
                    continue;

                foreach (BoundingSphere sphere in model.boundingSpheres)
                {
                    // Get the distance that the ray intersected into the sphere
                    float? intersection = sphere.Intersects(feeler);

                    // Was there a collision?
                    bool collision = intersection.HasValue && intersection <= aheadDistance;

                    if (!collision)
                        continue;

                    // There was a collision, and there are no other spheres yet
                    if (!nearestSphere.HasValue)
                        nearestSphere = sphere;

                    // There was a collision, but we must determine that this sphere is closer than other spheres
                    else if (Vector3.Distance(position, sphere.Center) < Vector3.Distance(position, ((BoundingSphere)nearestSphere).Center))
                        nearestSphere = sphere;
                }
            }

            return nearestSphere;
        }

        public void update(UpdateParams updateParams)
        {
            foreach (StaticModel model in models)
                model.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            foreach (StaticModel model in models)
                model.draw(drawParams);
        }
    }
}
