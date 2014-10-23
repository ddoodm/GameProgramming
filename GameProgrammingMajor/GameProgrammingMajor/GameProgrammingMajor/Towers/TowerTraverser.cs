using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The system that moves an entity across a path.
    /// The class also displays debug 'path spheres'.
    /// 
    /// Unfortunately, as the result of a very busy two weeks,
    /// this class is based upon the article at:
    /// http://xnatd.blogspot.com.au/2011/12/pathfinding-tutorial-part-3.html
    /// ... But has been modified to implement Steering
    /// </summary>
    class TowerTraverser
    {
        public Vector2 position;
        List<Vector2> path;
        StaticModel markerModel;
        Entity mover;
        TowerManager level;

        float levelWidth, levelHeight;

        public bool showDebugPath = true;

        public void addPath(List<Vector2> path)
        {
            this.path = path;
        }

        public void setMarker(StaticModel markerModel)
        {
            this.markerModel = markerModel;
        }

        public void setMover(Entity mover)
        {
            this.mover = mover;
        }

        /// <summary>
        /// Re-create the path as a result of updated search nodes
        /// </summary>
        /// <param name="pathFinder">The path finder that stores the new search nodes.</param>
        public void rePathfind(TowerManager level, TowerPathFinder pathFinder)
        {
            // If the path is null, there is no need to re-calculate
            if (path == null)
                return;

            iVec2 from = level.idOf(mover.kinematic.position);
            iVec2 to = new iVec2(path.Last() / (int)TowerManager.blockSize);

            path = pathFinder.FindPath(from, to);
        }

        public void Update(UpdateParams updateParams, TowerManager level)
        {
            this.level = level;

            levelWidth = levelHeight = TowerManager.NUM_BLOCKS * TowerManager.blockSize;

            // Update if there is a path
            if (path != null && path.Count > 0)
            {
                // Check whether the mover is in the target radius
                bool atNextTarget = mover.npc.inTargetRadius();

                if (atNextTarget)
                {
                    float speed = 50f;
                    position = path[0];

                    TowerType towerType = level.getTowerTypeAt(
                        (int)path[0].X / (int)TowerManager.blockSize,
                        (int)path[0].Y / (int)TowerManager.blockSize);

                    switch (towerType)
                    {
                        case TowerType.PATH: speed = 50f;  // Road
                            break;
                        case TowerType.GRASS: speed = 35f;    // Grass
                            break;
                        case TowerType.TAR: speed = 10f;   // Tar
                            break;
                    }
                    path.RemoveAt(0);

                    mover.npc.steering.maxSpeed = speed;
                }

                // If the steering type is not of type Seek, use Seek
                if (mover.npc.steering.GetType() != typeof(Seek))
                    mover.npc.steering = new Seek(mover.npc.steering);

                // If the path is greater than 1 node, use a target radius the size of one block
                ((Seek)mover.npc.steering).targetRadius = TowerManager.blockSize;
            }
            else
            {
                // If we are at the last node, use the Arrive steering method instaed
                mover.npc.steering = new Arrive(mover.npc.steering);

                // If we are at the end of the path, use a small target radius
                ((Arrive)mover.npc.steering).targetRadius = 1f;
                ((Arrive)mover.npc.steering).slowRadius = TowerManager.blockSize;
            }

            // Update models
            mover.update(updateParams);

            Vector2 target2d = getTileMidpoint(position);
            mover.npc.target = new Kinematic(pathToWorld(target2d));

            mover.npc.update(updateParams);
            markerModel.update(updateParams);
        }

        private Vector2 getTileMidpoint(Vector2 blockId)
        {
            return new Vector2(
                blockId.X * 2 - levelWidth + TowerManager.blockSize,
                blockId.Y * 2 - levelHeight + TowerManager.blockSize);
        }

        /// <summary>
        /// Convert 2D path coordinates into 3D terrain-following world coordinates.
        /// </summary>
        /// <param name="midpoint">The point in the middle of the node to map.</param>
        /// <returns>The point within the 3D world.</returns>
        private Vector3 pathToWorld(Vector2 midpoint)
        {
            return new Vector3(
                        midpoint.X,
                        level.terrain != null ? level.terrain.getYAt(midpoint) : level.midPosition.Y,
                        midpoint.Y);
        }

        public void Draw(DrawParams drawParams)
        {
            mover.draw(drawParams);

            if (showDebugPath)
                DrawPath(drawParams);
        }

        private void DrawPath(DrawParams drawParams)
        {
            // No path: return
            if (path == null)
                return;

            foreach (Vector2 v in path)
            {
                Vector2 midpoint = getTileMidpoint(v);
                markerModel.world = Matrix.CreateScale(2f) * Matrix.CreateTranslation(pathToWorld(midpoint));
                markerModel.draw(drawParams);
            }
        }
    }
}
