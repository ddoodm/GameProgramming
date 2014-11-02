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
        private static float cost = 200f;

        private int gWeight = 0;
        private Quadtree quadtree;
        private ProjectileManager projectileMan;
        private TowerManager level;

        private ModelBone axisBone, turretBone;
        private Matrix bakedAxisTransform, bakedTurretTransform;

        private Vector3 turretPosition;

        private BoundingSphere turretRange;

        private Entity target;

        private Player player;

        public TurretTower(Game game, Matrix world, TowerManager level, float size, Quadtree quadtree, iVec2 id)
            : base(game, world, size, id)
        {
            this.quadtree = quadtree;
            this.level = level;

            projectileMan = new ProjectileManager(game, ((MainGame)game).world, level, quadtree);

            turretRange = new BoundingSphere(this.boundingBox.Max, 300f);

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
            projectileMan.cooldownWait = 125f;

            // Load bones
            axisBone = model.model.Bones["turret_swiv"];
            bakedAxisTransform = axisBone.Transform;
            turretBone = model.model.Bones["turret_gun"];
            bakedTurretTransform = turretBone.Transform;

            turretPosition = world.Translation + bakedTurretTransform.Translation;
        }

        public override float getCost()
        {
            return cost;
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

            if (player == null)
                player = updateParams.player;

            model.update(updateParams);

            // Search for targets if we need a new one
            if(target == null || target.dead || !targetInRange(target))
                target = findTarget();

            projectileMan.update(updateParams);

            // Shoot at targets
            if (target != null)
            {
                Vector3 origin = turretPosition;
                updateTurretFacing(origin, target.kinematic.position);
                projectileMan.shootSeeking(updateParams, origin, target.kinematic, this, projectileCollisionCallback);
            }
        }

        public void projectileCollisionCallback(Entity collidedEntity)
        {
            collidedEntity.setAttacker(this);

            if (collidedEntity.dead)
                player.notifyEntityKilled(collidedEntity);
        }

        private bool targetInRange(Entity target)
        {
            return turretRange.Contains(target.kinematic.position) != ContainmentType.Disjoint;
        }

        private Entity findTarget()
        {
            List<Entity> entities = level.waveManager.getEntities();

            if (entities.Count == 0)
                return null;
            else
            {
                // Check whether a ray can see any of the tanks
                // Search backwards to find the oldest tank
                for(int i=entities.Count-1; i>=0; i--)
                {
                    // Check that the entity is in range
                    if (!targetInRange(entities[i]))
                        continue;

                    // Used to to ray casting here,
                    // but that was far too much of an overhead.

                    return entities[i];
                }
                return null;
            }
        }

        private void updateTurretFacing(Vector3 origin, Vector3 target)
        {
            Vector3 direction = target - origin;
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
