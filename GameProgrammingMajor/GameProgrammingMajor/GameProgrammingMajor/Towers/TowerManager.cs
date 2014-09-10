﻿using System;
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
            NUM_BLOCKS = 10;        // The number of tower blocks in both axes.
        private float
            blockSize = 20f;       // The size each block in the grid.

        // Transformation of this collection of towers ("village")
        public Matrix world;

        // The position from the centre of the manager
        public Vector3 midPosition;

        // The two-dimensional array of tower blocks to which towers may be allocated.
        public TowerBlock[,] blocks = new TowerBlock[NUM_BLOCKS, NUM_BLOCKS];

        // Determines which block is selected.
        public TowerSelectionManager selectionManager;

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        public TowerManager(Game game, Matrix world)
        {
            this.game = game;
            this.world = world;

            midPosition = world.Translation - new Vector3(1f, 0f, 1f) * (blockSize * NUM_BLOCKS - blockSize);

            initializeBlocks();

            selectionManager = new TowerSelectionManager(
                world, midPosition, NUM_BLOCKS, blockSize);
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
                        midPosition + new Vector3((float)x * blockSize*2, 0, (float)z * blockSize*2);

                    // Create the block:
                    blocks[z, x] = new TowerBlock(game, blockPosition, blockSize);
                }
        }

        public void update(EntityUpdateParams updateParams)
        {
            // Obtain the ID of the currently selected block
            iVec2 selectedBlock = selectionManager.getSelectedBlock(
                game.GraphicsDevice.Viewport, updateParams.camera, updateParams.mouseState);

            // For each block in the 2D array:
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    // If this block is the selected block, let it know
                    blocks[z, x].selected = selectedBlock == new iVec2(x, z);

                    // Perform an update on this block
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
