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
        private TowerManager level;

        public GrassTower(Game game, Matrix world, float size, TowerManager level, iVec2 id)
            : base(game, world, size, id)
        {
            this.level = level;
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
            // Add the death count to this block's GWeight
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
