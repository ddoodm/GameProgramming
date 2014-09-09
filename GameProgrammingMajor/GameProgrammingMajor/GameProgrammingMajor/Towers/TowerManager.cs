using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Manages the placement and update of towers.
    /// </summary>
    public class TowerManager
    {
        private Game game;

        private const int
            NUM_BLOCKS = 32;        // The number of tower blocks in both axes.
        private float
            gridSizee = 200f;       // The size of the block grid.

        // Position of this collection of towers ("village")
        public Vector3 position;

        // The two-dimensional array of tower blocks to which towers may be allocated.
        public TowerBlock[,] blocks = new TowerBlock[NUM_BLOCKS, NUM_BLOCKS];

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        public TowerManager(Game game, Vector3 position)
        {
            this.game = game;
            this.position = position;

            initializeBlocks();
        }

        /// <summary>
        /// Initialize each TowerBlock and provide its coordinates.
        /// </summary>
        private void initializeBlocks()
        {
            for(int z=0; z<NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    // Place each block in a grid formation:
                    Vector3 blockPosition =
                        position + new Vector3(x * gridSizee, 0, z * gridSizee);

                    // Create the block:
                    blocks[z, x] = new TowerBlock(game, blockPosition, gridSizee);
                }
        }

        public void update(EntityUpdateParams updateParams)
        {
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    blocks[z, x].update(updateParams);
                }
        }

        public void draw(EntityDrawParams drawParams)
        {
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    blocks[z, x].draw(drawParams);
                }
        }
    }
}
