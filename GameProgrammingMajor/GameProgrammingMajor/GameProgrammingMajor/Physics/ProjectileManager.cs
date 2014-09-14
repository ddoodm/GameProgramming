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
        private List<Entity> entityTargets;
        private StaticModel projectileModel;
        private float cooldown = 0;

        public float cooldownWait = 20f;
        public float projectileSpeed = 0.8f;
        public string projectileFireSound = SoundManager.SoundNames.PROJECTILE_FIRE;

        public ProjectileManager(Game game, World world)
        {
            this.game = game;
            projectiles = new List<Projectile>();

            entityTargets = world.entityManager.entities;
            staticTargets = world.staticManager.models;
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

                if (projectiles[i].collision_test(staticTargets)
                    || projectiles[i].collision_test(entityTargets)
                    || projectiles[i].collision_test(updateParams, updateParams.player))
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
            private ProjectileManager manager;
            private StaticModel projectileModel;
            private BoundingSphere boundingSphere;
            private Vector3 start;
            private Vector3 direction;

            public float speed;
            public float screenTime = 0;
            public float creationTime = 0;
            public float damage = 0.09f;

            public Projectile(ProjectileManager manager, StaticModel projectileModel, Vector3 start, Vector3 direction, float speed)
            {
                this.manager = manager;
                this.projectileModel = projectileModel;
                this.start = start;
                this.direction = direction;
                this.speed = speed;
            }

            public void update(UpdateParams updateParams)
            {
                float bulletTime = (float)updateParams.gameTime.TotalGameTime.TotalMilliseconds * speed;

                if (creationTime == 0)
                    creationTime = bulletTime;
                screenTime = bulletTime - creationTime;

                projectileModel.world = Matrix.Identity;
                projectileModel.world *= Matrix.CreateTranslation(start + screenTime * direction);

                // Update projectile's bounding sphere position
                foreach (ModelMesh mesh in projectileModel.model.Meshes)
                    boundingSphere = new BoundingSphere(start + screenTime * direction, mesh.BoundingSphere.Radius);
            }

            public bool collision_test(List<StaticModel> targets)
            {
                foreach (StaticModel target in targets)
                    if (target.collidesWith(boundingSphere))
                        return true;
                return false;
            }

            public bool collision_test(List<Entity> targets)
            {
                foreach (Entity target in targets)
                    if (target.collidesWith(boundingSphere))
                        return true;
                return false;
            }

            public bool collision_test(UpdateParams updateParams, Player player)
            {
                if(player.collidesWith(boundingSphere))
                {
                    player.takeDamage(updateParams, damage);

                    // Shake the camera
                    updateParams.camera.shake(25);

                    return true;
                }
                return false;
            }

            public void draw(DrawParams drawParams)
            {
                projectileModel.draw(drawParams);
            }
        }
    }
}
