using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Quadtree <T> where T : Entity
    {
        public const int MAX_OBJECTS = 0;
        public const int MAX_LEVELS = 5;

        public BoundingBox size;

        // Make the graphics device available to nodes
        public GraphicsDevice graphicsDevice;

        // The root (head) node
        QuadtreeNode<T> root;

        public Quadtree(BoundingBox size, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            this.size = size;

            // Create a node at the max level
            root = new QuadtreeNode<T>(this, null, size, 0);
        }

        /// <summary>
        /// Insert an entity into the quadtree.
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        public void insert(T entity)
        {
            if (root.intersects(entity))
                root.insert(entity);
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
