using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public class ProjectileManager
    {
        private const float bulletLife = 700f;

        private Game game;
        private List<Projectile> projectiles;
        private List<StaticModel> staticTargets;
        //private List<Entity> entityTargets;
        private TowerManager towerManager;
        private StaticModel projectileModel;
        private Quadtree quadtree;
        private float cooldown = 0;

        public float cooldownWait = 20f;
        public float projectileSpeed = 0.8f;
        public string projectileFireSound = SoundManager.SoundNames.PROJECTILE_FIRE;

        public ProjectileManager(Game game, World world, TowerManager towerManager, Quadtree quadtree)
        {
            this.game = game;
            projectiles = new List<Projectile>();

            /*entityTargets = world.entityManager.entities;*/
            staticTargets = world.staticManager.models;
            this.towerManager = towerManager;

            this.quadtree = quadtree;
        }

        public void loadContent(ContentManager content)
        {
            projectileModel = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));
        }

        public void shoot(UpdateParams updateParams, Vector3 origin, Vector3 direction)
        {
            if (cooldown <= 0)
            {
                Projectile p = new Projectile(this, 
                    new StaticModel(projectileModel), origin, direction, projectileSpeed);
                projectiles.Add(p);

                updateParams.soundManager.play(projectileFireSound);

                cooldown = cooldownWait;
            }
            else
                cooldown -= 1f;
        }

        public void shoot(UpdateParams updateParams, Camera camera)
        {
            shoot(updateParams, camera.position, camera.direction);
        }

        public void shootSeeking(UpdateParams updateParams, Vector3 origin, Kinematic target, Tower excludedTower)
        {
            if (cooldown <= 0)
            {
                Vector3 direction = Vector3.Normalize((target.position + target.velocity) - origin);
                SeekProjectile p = new SeekProjectile(this,
                    new StaticModel(projectileModel), origin, direction, projectileSpeed, target);
                p.excludedTower = excludedTower;
                projectiles.Add(p);

                updateParams.soundManager.play(projectileFireSound);

                cooldown = cooldownWait;
            }
            else
                cooldown -= 1f;
        }

        public void update(UpdateParams updateParams)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].update(updateParams);

                if (projectiles[i].screenTime >= bulletLife)
                {
                    projectiles.Remove(projectiles[i]);
                    continue;
                }

                if (
                  /*|| projectiles[i].collision_test(entityTargets)*/
                    projectiles[i].collision_test(towerManager)
                    || projectiles[i].collision_test(updateParams, quadtree))
                {
                    // Play collision sound effect
                    updateParams.soundManager.play(SoundManager.SoundNames.IMPACT_METAL);

                    // Remove the projectile
                    projectiles.Remove(projectiles[i]);
                    continue;
                }
            }
        }

        public void draw(DrawParams drawParams)
        {
            foreach (Projectile p in projectiles)
                p.draw(drawParams);
        }

        public class Projectile
        {
            protected ProjectileManager manager;
            protected StaticModel projectileModel;
            protected BoundingSphere boundingSphere;
            protected Vector3 start;
            protected Vector3 direction;

            public Tower excludedTower;

            public float speed;
            public float screenTime = 0;
            public float creationTime = 0;
            public float damage = 1f / 5f;
            public float radius = 15f;

            public Projectile(ProjectileManager manager, StaticModel projectileModel, Vector3 start, Vector3 direction, float speed)
            {
                this.manager = manager;
                this.projectileModel = projectileModel;
                this.start = start;
                this.direction = direction;
                this.speed = speed;

                boundingSphere = new BoundingSphere(start + screenTime * direction, radius);
            }

            public virtual void update(UpdateParams updateParams)
            {
                float bulletTime = (float)updateParams.gameTime.TotalGameTime.TotalMilliseconds * speed;

                if (creationTime == 0)
                    creationTime = bulletTime;
                screenTime = bulletTime - creationTime;

                projectileModel.world = Matrix.Identity;
                projectileModel.world *= Matrix.CreateTranslation(start + screenTime * direction);

                // Update projectile's bounding sphere position
                boundingSphere = new BoundingSphere(start + screenTime * direction, radius);
            }

            public bool collision_test(List<StaticModel> targets)
            {
                foreach (StaticModel target in targets)
                    if (target.collidesWith(boundingSphere))
                        return true;
                return false;
            }

            /*
            public bool collision_test(List<Entity> targets)
            {
                foreach (Entity target in targets)
                    if (target.collidesWith(boundingSphere))
                        return true;
                return false;
            }*/

            /// <summary>
            /// Check for collisions with all Towers in a TowerManager
            /// </summary>
            public bool collision_test(TowerManager towerManager)
            {
                return towerManager.towersCollideWith(boundingSphere, excludedTower);
            }

            public bool collision_test(UpdateParams updateParams, Quadtree quadtree)
            {
                Entity subject = quadtree.collision(boundingSphere);

                if (subject != null && subject.npc != null)
                    subject.npc.takeDamage(updateParams, damage);

                return subject != null;
            }

            public void draw(DrawParams drawParams)
            {
                projectileModel.draw(drawParams);
            }
        }

        public class SeekProjectile : Projectile
        {
            private Kinematic target;

            public SeekProjectile(ProjectileManager manager, StaticModel projectileModel, Vector3 start, Vector3 direction, float speed, Kinematic target)
                : base(manager, projectileModel, start, direction, speed)
            {
                this.target = target;
            }

            public override void update(UpdateParams updateParams)
            {
                recomputeDirection();
                base.update(updateParams);
            }

            private void recomputeDirection()
            {
                if (boundingSphere != null)
                    this.direction = Vector3.Normalize( (target.position + Vector3.Up * 5f) - start );
            }
        }
    }
}
