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

        // The two-dimensional index of this block
        protected iVec2 blockId;

        // The transformation of this block
        public Matrix world;

        // The size of the model (should be constant, but here for flexibility)
        public float size; 

        // The actual tower that comsunes this block. Can be null.
        public Tower tower = null;

        // Illuminates when the block is selected
        private PlaneEntity selectionIndicator;
        public bool selected = false;

        public TowerBlock(Game game, iVec2 blockId, Matrix world, float size)
        {
            this.game = game;
            this.blockId = blockId;
            this.world = world;
            this.size = size;

            init_selectionIndicator();
        }

        private void init_selectionIndicator()
        {
            PlanePrimitive selModel = new PlanePrimitive(game, size, Vector3.Up);
            selModel.texture = game.Content.Load<Texture2D>("Textures\\UI\\towerBlockSelector");
            selModel.textureTiling = Vector2.One;

            selectionIndicator = new PlaneEntity(game, selModel, world.Translation, 0);
            selectionIndicator.kinematic.position = world.Translation + Vector3.Up * 2f; // Offset into +Y
        }

        public void setTower(Tower tower)
        {
            this.tower = tower;
        }

        public int getGWeight()
        {
            return tower.getGWeight();
        }

        public void update(UpdateParams updateParams)
        {
            if(tower != null) tower.update(updateParams);
            selectionIndicator.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            if (tower != null) tower.draw(drawParams);

            if (selected)
            {
                // Draw selection indicator as semi-transparent
                game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                selectionIndicator.draw(drawParams);
            }
        }
    }
}
