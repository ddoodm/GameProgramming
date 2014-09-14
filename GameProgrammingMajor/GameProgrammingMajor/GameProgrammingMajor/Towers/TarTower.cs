using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    class TarTower : Tower
    {
        public int gWeight = 10;

        public TarTower(Game game, Matrix world, float size)
            : base(game, world, size)
        {
            loadModel();
        }

        /// <summary>
        /// Loads the standard "Wall" tower model
        /// </summary>
        private void loadModel()
        {
            model = new StaticModel(game, game.Content.Load<Model>("Models\\tower_tar"), world);
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
        }
    }
}
