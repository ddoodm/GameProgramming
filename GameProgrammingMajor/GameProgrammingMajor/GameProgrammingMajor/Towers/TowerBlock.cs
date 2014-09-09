using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The discrete space in which a tower resides.
    /// </summary>
    public class TowerBlock
    {
        private Game game;

        public Vector3 position;
        public float size;

        // Illuminates when the block is selected
        private PlaneEntity selectionIndicator;

        public TowerBlock(Game game, Vector3 position, float size)
        {
            this.game = game;

            selectionIndicator = new PlaneEntity(game, position, Vector3.Up, size, 0);
            selectionIndicator.primitive.texture = game.Content.Load<Texture2D>("Textures\\UI\\towerBlockSelector");
            selectionIndicator.kinematic.position = position;
        }

        public void update(EntityUpdateParams updateParams)
        {
            selectionIndicator.update(updateParams);
        }

        public void draw(EntityDrawParams drawParams)
        {
            // Draw selection indicator as semi-transparent
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            selectionIndicator.draw(drawParams);
        }
    }
}
