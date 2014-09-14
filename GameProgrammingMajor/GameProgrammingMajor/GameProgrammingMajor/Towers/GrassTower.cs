using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProgrammingMajor
{
    class GrassTower : Tower
    {
        public int gWeight = 3;

        public GrassTower(Game game, Matrix world, float size)
            : base(game, world, size)
        {
            loadModel();
        }

        /// <summary>
        /// Loads the standard "Wall" tower model
        /// </summary>
        private void loadModel()
        {
            // Grass tower does not use a model
            model = null;
        }

        public override int getGWeight()
        {
            return gWeight;
        }

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);
        }

        public override void draw(DrawParams drawParams)
        {
            base.draw(drawParams);
        }
    }
}
