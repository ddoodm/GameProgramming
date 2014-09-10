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
        public bool selected = false;

        public TowerBlock(Game game, Vector3 position, float size)
        {
            this.game = game;
            this.position = position;
            this.size = size;

            init_selectionIndicator();
        }

        private void init_selectionIndicator()
        {
            PlanePrimitive selModel = new PlanePrimitive(game, size, Vector3.Up);
            selModel.texture = game.Content.Load<Texture2D>("Textures\\UI\\towerBlockSelector");
            selModel.textureTiling = Vector2.One;

            selectionIndicator = new PlaneEntity(game, selModel, position, 0);
            selectionIndicator.kinematic.position = position + Vector3.Up * 2f; // Offset into +Y
        }

        public void update(EntityUpdateParams updateParams)
        {
            selectionIndicator.update(updateParams);
        }

        public void draw(EntityDrawParams drawParams)
        {
            if (selected)
            {
                // Draw selection indicator as semi-transparent
                game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                selectionIndicator.draw(drawParams);
            }
        }
    }
}
