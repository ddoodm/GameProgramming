using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public class QuadtreeNode<T> where T : Entity
    {
        // The area that this node covers
        BoundingBox bounds;
        DrawableBoundingBox drawBounds;

        // The parent tree structure
        Quadtree<T> tree;

        // This node's four children (Upper Left, Upper Right, ...)
        QuadtreeNode<T> UL, UR, LL, LR;

        // This node's parent
        QuadtreeNode<T> parent;

        List<T> entities;

        int currentDepth;

        public QuadtreeNode(Quadtree<T> tree, QuadtreeNode<T> parent, BoundingBox bounds, int currentDepth)
        {
            this.tree = tree;
            this.parent = parent;
            this.bounds = bounds;
            this.currentDepth = currentDepth;

            entities = new List<T>();

            drawBounds = new DrawableBoundingBox(bounds, tree.graphicsDevice, isLeaf()? Color.LightGreen : Color.LightPink);
        }

        public QuadtreeNode(Quadtree<T> tree, QuadtreeNode<T> parent, BoundingBox bounds, int currentDepth, Color color)
            : this(tree, parent, bounds, currentDepth)
        {
            drawBounds = new DrawableBoundingBox(bounds, tree.graphicsDevice, color);
        }

        public bool isLeaf()
        {
            return 
                (currentDepth >= Quadtree<T>.MAX_LEVELS)
                || (entities.Count <= Quadtree<T>.MAX_OBJECTS);
        }

        public bool willBeLeaf()
        {
            return
                (currentDepth >= Quadtree<T>.MAX_LEVELS)
                || ((entities.Count + 1) <= Quadtree<T>.MAX_OBJECTS);
        }

        /// <summary>
        /// Is the supplied entity inside this node?
        /// </summary>
        public bool intersects(T entity)
        {
            return entity.collidesWith(bounds);
        }

        public void insert(T entity)
        {
            if (this.willBeLeaf())
                entities.Add(entity);
            else
                foreach (QuadtreeNode<T> node in branchesFor(entity))
                    node.insert(entity);
        }

        /// <summary>
        /// Find the child branch for the entity
        /// </summary>
        private List<QuadtreeNode<T>> branchesFor(T entity)
        {
            List<QuadtreeNode<T>> branches = new List<QuadtreeNode<T>>();
            Vector3 midpoint = bounds.Min + (bounds.Max - bounds.Min) / 2f;
            midpoint = new Vector3(midpoint.X, 0, midpoint.Z);
            BoundingBox subBox;

            if (entity.collidesWith(subBox = expandY(computeULBox(midpoint))))
            {
                if (UL == null)
                    UL = new QuadtreeNode<T>(tree, this, subBox, currentDepth + 1, Color.Yellow);
                branches.Add(UL);
            }

            if (entity.collidesWith(subBox = expandY(computeURBox(midpoint))))
            {
                if (UR == null)
                    UR = new QuadtreeNode<T>(tree, this, subBox, currentDepth + 1, Color.Lime);
                branches.Add(UR);
            }

            if (entity.collidesWith(subBox = expandY(computeLLBox(midpoint))))
            {
                if (LL == null)
                    LL = new QuadtreeNode<T>(tree, this, subBox, currentDepth + 1, Color.DarkOrchid);
                branches.Add(LL);
            }

            if (entity.collidesWith(subBox = expandY(computeLRBox(midpoint))))
            {
                if (LR == null)
                    LR = new QuadtreeNode<T>(tree, this, subBox, currentDepth + 1, Color.Orange);
                branches.Add(LR);
            }

            return branches;
        }

        private BoundingBox expandY(BoundingBox box)
        {
            return new BoundingBox(
                new Vector3(box.Min.X, tree.size.Min.Y, box.Min.Z),
                new Vector3(box.Max.X, tree.size.Max.Y, box.Max.Z));
        }

        private BoundingBox computeULBox(Vector3 midpoint)
        {
            return new BoundingBox(bounds.Min, midpoint);
        }

        private BoundingBox computeURBox(Vector3 midpoint)
        {
            return new BoundingBox(new Vector3(midpoint.X, 0, bounds.Min.Z), new Vector3(bounds.Max.X, 0, midpoint.Z));
        }

        private BoundingBox computeLLBox(Vector3 midpoint)
        {
            return new BoundingBox(new Vector3(bounds.Min.X, 0, midpoint.Z), new Vector3(midpoint.X, 0, bounds.Max.Z));
        }

        private BoundingBox computeLRBox(Vector3 midpoint)
        {
            return new BoundingBox(midpoint, bounds.Max);
        }

        /// <summary>
        /// Move an entity into another node
        /// </summary>
        public void moveAway(T entity)
        {
            if(isLeaf())
                entities.Remove(entity);

            tree.insert(entity);

            if (empty() && parent != null)
            {
                entities.Clear();
                parent.remove(this);
            }
        }

        public void remove(QuadtreeNode<T> node)
        {
            if (UL != null && UL.Equals(node)) UL = null;
            if (UR != null && UR.Equals(node)) UR = null;
            if (LL != null && LL.Equals(node)) LL = null;
            if (LR != null && LR.Equals(node)) LR = null;

            if (empty() && parent != null)
                parent.remove(this);
        }

        public bool empty()
        {
            return UL == null && UR == null && LL == null && LR == null;
        }

        public int count()
        {
            int count = 1;
            if (UL != null) count += UL.count();
            if (UR != null) count += UR.count();
            if (LL != null) count += LL.count();
            if (LR != null) count += LR.count();
            return count;
        }

        public void update(UpdateParams updateParams)
        {
            if (UL != null) UL.update(updateParams);
            if (UR != null) UR.update(updateParams);
            if (LL != null) LL.update(updateParams);
            if (LR != null) LR.update(updateParams);

            if (this.isLeaf())
            {
                List<T> removeList = new List<T>();
                foreach (T t in entities)
                    if (!this.intersects(t))
                    {
                        moveAway(t);
                        break;
                    }
            }
        }

        public void draw(DrawParams drawParams)
        {
            drawBounds.draw(drawParams);

            if (UL != null) UL.draw(drawParams);
            if (UR != null) UR.draw(drawParams);
            if (LL != null) LL.draw(drawParams);
            if (LR != null) LR.draw(drawParams);
        }
    }
}
