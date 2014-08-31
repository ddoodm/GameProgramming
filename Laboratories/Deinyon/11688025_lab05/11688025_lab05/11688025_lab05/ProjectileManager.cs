using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _11688025_lab05
{
    public class ProjectileManager
    {
        private const float bulletLife = 420f;

        private Game game;
        private List<Projectile> projectiles;
        private List<StaticModel> targets;
        private StaticModel model;
        private float cooldown = 0;

        private SoundEffect sf_bullet;
        private SoundEffect sf_impact;

        public ProjectileManager(Game game)
        {
            this.game = game;
            projectiles = new List<Projectile>();
            targets = ((Game1)game).modelManager.models;
        }

        public void loadContent(Game game)
        {
            model = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));

            sf_bullet = game.Content.Load<SoundEffect>("Sound\\smg1_fire1");
            sf_impact = game.Content.Load<SoundEffect>("Sound\\metal_barrel_impact_hard2");
        }

        public void shoot(Vector3 origin, Vector3 direction)
        {
            if (cooldown <= 0)
            {
                Projectile p = new Projectile(this, new StaticModel(model), origin, direction);
                projectiles.Add(p);

                sf_bullet.Play();

                cooldown = p.cooldown;
            }
            else
                cooldown -= 1f;
        }

        public void shoot(Camera camera)
        {
            shoot(camera.eye, camera.direction);
        }

        public void update(GameTime gameTime)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].update(gameTime);

                if (projectiles[i].screenTime >= bulletLife)
                {
                    projectiles.Remove(projectiles[i]);
                    continue;
                }

                if (projectiles[i].collision_test(targets))
                {
                    ((Game1)game).camera.shake(25);
                    projectiles.Remove(projectiles[i]);
                    continue;
                }
            }
        }

        public void draw(Camera camera)
        {
            foreach (Projectile p in projectiles)
                p.draw(camera);
        }

        public class Projectile
        {
            private const float SPEED = 0.8f;

            private ProjectileManager manager;
            private StaticModel model;
            private Vector3 start;
            private Vector3 direction;

            public float cooldown = 6f;
            public float screenTime = 0;
            public float creationTime = 0;

            public Projectile(ProjectileManager manager, StaticModel model, Vector3 start, Vector3 direction)
            {
                this.manager = manager;
                this.model = model;
                this.start = start;
                this.direction = direction;
            }

            public void update(GameTime gameTime)
            {
                float bulletTime = (float)gameTime.TotalGameTime.TotalMilliseconds * SPEED;

                if (creationTime == 0)
                    creationTime = bulletTime;
                screenTime = bulletTime - creationTime;

                model.world = Matrix.Identity;
                model.world *= Matrix.CreateTranslation(start + screenTime * direction);
            }

            public bool collision_test(List<StaticModel> targets)
            {
                foreach (StaticModel target in targets)
                {
                    if (target.getColSphereSize() <= 0)
                        continue;

                    // Collision with current target is possible
                    // Get distance from projectile to target
                    // by computing delta magnitude
                    Vector3 toTarget = target.world.Translation - model.world.Translation;
                    float distance = toTarget.Length();

                    if (Math.Abs(distance) <= target.getColSphereSize())
                    {
                        // Play collision sound effect
                        manager.sf_impact.Play();

                        //target.collisionResponse();
                        return true;
                    }
                }

                return false;
            }

            public void draw(Camera camera)
            {
                model.draw(camera);
            }
        }
    }
}
