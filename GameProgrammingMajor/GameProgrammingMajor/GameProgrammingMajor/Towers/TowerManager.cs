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
    class TowerManager
    {
        private Game game;

        private static const int
            NUM_BLOCKS = 32;        // The number of tower blocks in both axes.
        private static float
            blockSize = 10f;        // The size of each block.

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
                    blocks[z, x].position =
                        position + new Vector3(x * blockSize, 0, z * blockSize);

                    // Provide the blockSize:
                    blocks[z, x].size = blockSize;
                }
        }
    }
}
