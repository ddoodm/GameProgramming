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

       public TeapotTower(Game game, Matrix world, float size)
            : base(game, world, size)
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

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);

            model.update(updateParams);
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
