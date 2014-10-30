using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class TurretTower : WallTower
    {
        private int gWeight = 0;
        private Quadtree quadtree;
        private ProjectileManager projectileMan;
        private TowerManager level;

        private ModelBone axisBone, turretBone;
        private Matrix bakedAxisTransform, bakedTurretTransform;

        private Vector3 turretPosition;

        public TurretTower(Game game, Matrix world, TowerManager level, float size, Quadtree quadtree)
            : base(game, world, size)
        {
            this.quadtree = quadtree;
            this.level = level;

            projectileMan = new ProjectileManager(game, ((MainGame)game).world, level, quadtree);

            loadModel();
        }

        /// <summary>
        /// Loads the standard "Wall" tower model
        /// </summary>
        private void loadModel()
        {
            model = new StaticModel(game, game.Content.Load<Model>("Models\\tower_turret"), world);

            // Create a Bounding Box from the model using our utility class
            boundingBox = BoundingBoxUtilities.createBoundingBox(model.model, world);

            // Create a drawable Bounding Box from the Bounding Box created above
            drawableBoundingBox = new DrawableBoundingBox(boundingBox, game.GraphicsDevice, Color.White);

            projectileMan.loadContent(game.Content);
            projectileMan.projectileSpeed = .25f;
            projectileMan.cooldownWait = 70f;

            // Load bones
            axisBone = model.model.Bones["turret_swiv"];
            bakedAxisTransform = axisBone.Transform;
            turretBone = model.model.Bones["turret_gun"];
            bakedTurretTransform = turretBone.Transform;

            turretPosition = world.Translation + new Vector3(0, boundingBox.Max.Y, 0);
        }

        public override int getGWeight()
        {
            return gWeight;
        }

        /// <summary>
        /// Is it impossible to pathfind through this tower?
        /// </summary>
        public override bool isSolid()
        {
            return true;
        }

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);

            model.update(updateParams);

            // Search for targets
            Entity target = findTarget();

            projectileMan.update(updateParams);

            // Shoot at targets
            if (target != null)
            {
                Vector3 origin = turretPosition;
                Vector3 direction = (target.kinematic.position + target.kinematic.velocity) - origin;
                Vector3 unitDirection = Vector3.Normalize(direction);
                updateTurretFacing(direction);
                projectileMan.shoot(updateParams, origin, unitDirection);
            }
        }

        private Entity findTarget()
        {
            QuadtreeNode node = quadtree.getNodeAt(world.Translation);

            if (node == null)
                return null;

            List<Entity> entities = node.getEntities();

            if (entities.Count == 0)
                return null;
            else
            {
                // Check whether a ray can see any of the tanks
                foreach (Entity entity in entities)
                {
                    Ray ray = new Ray(turretPosition, Vector3.Normalize(
                        (entity.kinematic.position + entity.kinematic.velocity) - turretPosition));
                    if (!level.intersects(ray))
                        return entity;
                }
                return null;
            }
        }

        private void updateTurretFacing(Vector3 direction)
        {
            float rotY = (float)Math.Atan2(direction.X, direction.Z) - MathHelper.ToRadians(90f);
            Matrix rotMatrix = Matrix.CreateRotationY(rotY);
            axisBone.Transform = bakedAxisTransform * rotMatrix;
            turretBone.Transform = bakedTurretTransform * rotMatrix;
        }

        public override void draw(DrawParams drawParams)
        {
            base.draw(drawParams);

            drawShadow(drawParams);
            model.draw(drawParams);
            drawableBoundingBox.draw(drawParams);

            projectileMan.draw(drawParams);
        }
    }
}
