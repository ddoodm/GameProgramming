using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class TeapotTower : Tower
    {
        private int gWeight = 0;

        // Teapot takes 85 hits before the game is over
        private const float DAMAGE = 1f / 85f;

        public float health = 1f;

       public TeapotTower(Game game, Matrix world, float size, iVec2 id)
            : base(game, world, size, id)
        {
            loadModel();
        }

        /// <summary>
        /// Loads a PlasmaModel Teapot
        /// </summary>
        private void loadModel()
        {
            model = new PlasmaModel(game, game.Content.Load<Model>("Models\\teapot"), world.Translation);

            // Create a Bounding Box from the model using our utility class
            boundingBox = BoundingBoxUtilities.createBoundingBox(model.model, world);

            // Create a drawable Bounding Box from the Bounding Box created above
            drawableBoundingBox = new DrawableBoundingBox(boundingBox, game.GraphicsDevice, Color.White);
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

        public override bool collidesWith(BoundingSphere sphere)
        {
            if (base.collidesWith(sphere))
            {
                this.takeDamage();
                return true;
            }
            return false;
        }

        public void takeDamage()
        {
            health -= DAMAGE;

            if (health <= 0)
                this.kill();
        }

        public void kill()
        {
            destroy();
        }

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);

            model.update(updateParams);

            updateParams.hud.teapotHealth = health;
            updateParams.hud.teapotDead = dead;
        }

        public override void draw(DrawParams drawParams)
        {
            base.draw(drawParams);

            drawShadow(drawParams);
            model.draw(drawParams);
            drawableBoundingBox.draw(drawParams);
        }
    }
}
