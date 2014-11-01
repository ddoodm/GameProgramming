using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public class QuadtreeNode
    {
        // The area that this node covers
        BoundingBox bounds;
        DrawableBoundingBox drawBounds;

        // The parent tree structure
        Quadtree tree;

        // This node's four children (Upper Left, Upper Right, ...)
        QuadtreeNode UL, UR, LL, LR;

        // This node's parent
        QuadtreeNode parent;

        List<Entity> entities;

        int currentDepth;

        public QuadtreeNode(Quadtree tree, QuadtreeNode parent, BoundingBox bounds, int currentDepth)
        {
            this.tree = tree;
            this.parent = parent;
            this.bounds = bounds;
            this.currentDepth = currentDepth;

            entities = new List<Entity>();

            drawBounds = new DrawableBoundingBox(bounds, tree.graphicsDevice, isLeaf()? Color.LightGreen : Color.LightPink);
        }

        public QuadtreeNode(Quadtree tree, QuadtreeNode parent, BoundingBox bounds, int currentDepth, Color color)
            : this(tree, parent, bounds, currentDepth)
        {
            drawBounds = new DrawableBoundingBox(bounds, tree.graphicsDevice, color);
        }

        public bool isLeaf()
        {
            return (empty()) && (
                (currentDepth >= Quadtree.MAX_LEVELS)
                || (entities.Count <= Quadtree.MAX_OBJECTS));
        }

        public bool willBeLeaf()
        {
            return (empty()) && (
                (currentDepth >= Quadtree.MAX_LEVELS)
                || ((entities.Count + 1) <= Quadtree.MAX_OBJECTS));
        }

        /// <summary>
        /// Is the supplied entity inside this node?
        /// </summary>
        public bool intersects(Entity entity)
        {
            return entity.collidesWith(bounds);
        }

        public bool contains(BoundingSphere sphere)
        {
            return bounds.Contains(sphere) != ContainmentType.Disjoint;
        }

        public Entity collision(BoundingSphere sphere)
        {
            if (isLeaf())
            {
                foreach (Entity t in entities)
                    if (t.collidesWith(sphere))
                        return t;
                return null;
            }
            else
            {
                if (UL != null && UL.contains(sphere)) return UL.collision(sphere);
                if (UR != null && UR.contains(sphere)) return UR.collision(sphere);
                if (LL != null && LL.contains(sphere)) return LL.collision(sphere);
                if (LR != null && LR.contains(sphere)) return LR.collision(sphere);
                return null;
            }
        }

        public void insert(Entity entity)
        {
            if (this.willBeLeaf())
            {
                entities.Add(entity);
                entity.treeNodes.Add(this);
            }
            else
            {
                // Move all entities down
                List<Entity> movingEntities = new List<Entity>(entities);
                movingEntities.Add(entity);
                this.entities.Clear();

                foreach (Entity t in movingEntities)
                {
                    t.treeNodes.Remove(this);
                    foreach (QuadtreeNode node in branchesFor(t))
                        node.insert(t);
                }

                movingEntities.Clear();
            }
        }

        /// <summary>
        /// Find the child branch for the entity
        /// </summary>
        private List<QuadtreeNode> branchesFor(Entity entity)
        {
            List<QuadtreeNode> branches = new List<QuadtreeNode>();
            Vector3 midpoint = bounds.Min + (bounds.Max - bounds.Min) / 2f;
            midpoint = new Vector3(midpoint.X, 0, midpoint.Z);
            BoundingBox subBox;

            if (entity.collidesWith(subBox = expandY(computeULBox(midpoint))))
            {
                if (UL == null)
                    UL = new QuadtreeNode(tree, this, subBox, currentDepth + 1, Color.Yellow);
                branches.Add(UL);
            }

            if (entity.collidesWith(subBox = expandY(computeURBox(midpoint))))
            {
                if (UR == null)
                    UR = new QuadtreeNode(tree, this, subBox, currentDepth + 1, Color.Lime);
                branches.Add(UR);
            }

            if (entity.collidesWith(subBox = expandY(computeLLBox(midpoint))))
            {
                if (LL == null)
                    LL = new QuadtreeNode(tree, this, subBox, currentDepth + 1, Color.DarkOrchid);
                branches.Add(LL);
            }

            if (entity.collidesWith(subBox = expandY(computeLRBox(midpoint))))
            {
                if (LR == null)
                    LR = new QuadtreeNode(tree, this, subBox, currentDepth + 1, Color.Orange);
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
        public void moveAway(Entity entity)
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

        public void remove(QuadtreeNode node)
        {
            /*if (UL != null && UL.Equals(node)) UL = null;
            if (UR != null && UR.Equals(node)) UR = null;
            if (LL != null && LL.Equals(node)) LL = null;
            if (LR != null && LR.Equals(node)) LR = null;

            if (empty() && parent != null)
                parent.remove(this);
             */
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

        public QuadtreeNode getNodeAt(Vector3 position)
        {
            // If there is no overlap, do not return a node
            if (bounds.Contains(position) == ContainmentType.Contains)
            {
                if (isLeaf())
                    return this;

                QuadtreeNode node = null;
                if (UL != null && (node = UL.getNodeAt(position)) != null) return node;
                if (UR != null && (node = UR.getNodeAt(position)) != null) return node;
                if (LL != null && (node = LL.getNodeAt(position)) != null) return node;
                if (LR != null && (node = LR.getNodeAt(position)) != null) return node;
            }

            return null;
        }

        public QuadtreeNode getParentIfExists()
        {
            return parent;
        }

        public List<Entity> getEntities()
        {
            List<Entity> entities = new List<Entity>();
            if (isLeaf())
                entities.AddRange(this.entities);
            else
            {
                if (UL != null) entities.AddRange(UL.getEntities());
                if (UR != null) entities.AddRange(UR.getEntities());
                if (LL != null) entities.AddRange(LL.getEntities());
                if (LR != null) entities.AddRange(LR.getEntities());
            }
            return entities;
        }

        public void remove(Entity entity)
        {
            if (isLeaf())
                entities.Remove(entity);

            if (empty() && parent != null)
            {
                entities.Clear();
                parent.remove(this);
            }
        }

        public void removeIfOutside(Entity entity)
        {
            if (!entity.collidesWith(bounds))
                remove(entity);
        }

        public void update(UpdateParams updateParams)
        {
            entities.Clear();
            if(parent != null) parent.remove(this);

            /*
            if (UL != null) UL.update(updateParams);
            if (UR != null) UR.update(updateParams);
            if (LL != null) LL.update(updateParams);
            if (LR != null) LR.update(updateParams);

            if (this.isLeaf())
            {
                foreach (Entity t in entities)
                    if (!this.intersects(t))
                    {
                        moveAway(t);
                        break;
                    }
            } */
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
