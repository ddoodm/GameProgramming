﻿using System;
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
        private const float bulletLife = 420f;

        private Game game;
        private List<Projectile> projectiles;
        private List<StaticModel> staticTargets;
        private List<Entity> entityTargets;
        private StaticModel projectileModel;
        private float cooldown = 0;

        private SoundEffect sf_bullet;
        private SoundEffect sf_impact;

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

            sf_bullet = game.Content.Load<SoundEffect>("Sound\\smg1_fire1");
            sf_impact = game.Content.Load<SoundEffect>("Sound\\metal_barrel_impact_hard2");
        }

        public void shoot(Vector3 origin, Vector3 direction)
        {
            if (cooldown <= 0)
            {
                Projectile p = new Projectile(this, new StaticModel(projectileModel), origin, direction);
                projectiles.Add(p);

                sf_bullet.Play();

                cooldown = p.cooldown;
            }
            else
                cooldown -= 1f;
        }

        public void shoot(Camera camera)
        {
            shoot(camera.position, camera.direction);
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
                    || projectiles[i].collision_test(entityTargets))
                {
                    // Play collision sound effect
                    sf_impact.Play();

                    // Shake the camera
                    updateParams.camera.shake(25);

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
            private const float SPEED = 0.8f;

            private ProjectileManager manager;
            private StaticModel projectileModel;
            private BoundingSphere boundingSphere;
            private Vector3 start;
            private Vector3 direction;

            public float cooldown = 20f;
            public float screenTime = 0;
            public float creationTime = 0;

            public Projectile(ProjectileManager manager, StaticModel projectileModel, Vector3 start, Vector3 direction)
            {
                this.manager = manager;
                this.projectileModel = projectileModel;
                this.start = start;
                this.direction = direction;
            }

            public void update(UpdateParams updateParams)
            {
                float bulletTime = (float)updateParams.gameTime.TotalGameTime.TotalMilliseconds * SPEED;

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

            public void draw(DrawParams drawParams)
            {
                projectileModel.draw(drawParams);
            }
        }
    }
}