using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Quadtree
    {
        public const int MAX_OBJECTS = 0;
        public const int MAX_LEVELS = 4;

        public BoundingBox size;

        // Make the graphics device available to nodes (for debug)
        public GraphicsDevice graphicsDevice;

        // The root (head) node
        QuadtreeNode root;

        public Quadtree(BoundingBox size, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            this.size = size;

            // Create a node at the max level
            root = new QuadtreeNode(this, null, size, 0);
        }

        /// <summary>
        /// Insert an entity into the quadtree.
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        public void insert(Entity entity)
        {
            if (root.intersects(entity))
                root.insert(entity);
        }

        public QuadtreeNode getNodeAt(Vector3 position)
        {
            return root.getNodeAt(position);
        }

        /** Returns any element that collides with a sphere */
        public Entity collision(BoundingSphere sphere)
        {
            if(!root.contains(sphere))
                return null;

            return root.collision(sphere);
        }

        public List<Entity> getEntities()
        {
            return root.getEntities();
        }

        public void update(UpdateParams updateParams)
        {
            root.update(updateParams);

            updateParams.hud.quadtreeNodeCount = root.count();
        }

        public void draw(DrawParams drawParams)
        {
            root.draw(drawParams);
        }
    }
}
