using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public enum TowerType
    {
        WALL = 0,
        PATH,
        GRASS,
        TAR,
        NONE
    }

    /// <summary>
    /// Manages the placement and update of towers.
    /// </summary>
    public class TowerManager
    {
        private Game game;

        private const int
            NUM_BLOCKS = 10;        // The number of tower blocks in both axes.
        public const float
            blockSize = 20f;       // The size each block in the grid.

        // Transformation of this collection of towers ("village")
        public Matrix world;

        // The position from the centre of the manager
        public Vector3 midPosition;

        // The two-dimensional array of tower blocks to which towers may be allocated.
        public TowerBlock[,] blocks = new TowerBlock[NUM_BLOCKS, NUM_BLOCKS];

        // Determines which block is selected.
        public TowerSelectionManager selectionManager;

        // Illustrates the boundaries of the Tower Manager region
        private PlaneEntity boundary;

        // The path finding algorithm
        private TowerPathFinder pathFinder;

        // The AI that traverses the terrain
        private TowerTraverser mover;

        // The path for the AI to follow
        private List<Vector2> path;

        // A pre-designed map
        private int[,] predesignedMap = new int[NUM_BLOCKS, NUM_BLOCKS]
        {
            {2,2,2,2,2,1,2,2,2,2},
            {2,3,3,2,2,1,2,2,2,2},
            {2,3,2,2,2,1,2,2,2,2},
            {3,3,2,2,2,1,1,1,2,2},
            {3,2,2,0,0,0,0,1,2,2},
            {3,2,2,2,2,1,1,1,2,2},
            {2,2,2,2,2,1,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2},
            {1,1,1,1,1,1,2,2,2,2},
        };

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        public TowerManager(Game game, Matrix world, StaticModelManager staticManager)
        {
            this.game = game;
            this.world = world;

            midPosition = world.Translation - new Vector3(
                blockSize * NUM_BLOCKS,
                0f,
                blockSize * NUM_BLOCKS);

            initializeBlocks();

            selectionManager = new TowerSelectionManager(this);

            boundary = new PlaneEntity(game, new WireframePlanePrimitive(
                game, blockSize * NUM_BLOCKS, Vector3.Up), world.Translation + Vector3.Up, 0);
            boundary.primitive.diffuseColour = new Vector3(1f, 0f, 0f);

            // Add the "Base" model entity
            Matrix baseTransform = Matrix.CreateScale(blockSize * NUM_BLOCKS) * world;
            staticManager.add(new StaticModel(game, game.Content.Load<Model>("Models\\towerManBase"), baseTransform));

            // Set up path finding objects
            pathFinder = new TowerPathFinder(this);
            mover = new TowerTraverser();
            path = pathFinder.FindPath(new Point(0, 0), new Point(8, 4));
            mover.addPath(path);
        }

        public void load(ContentManager content)
        {
            mover.addMoverModel(new StaticModel(game, content.Load<Model>("Models\\mover_model")));
            mover.addModel(new StaticModel(game, content.Load<Model>("Models\\DSphere")));
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
                    Vector3 blockPosition = coordinatesOf(new iVec2(z, x));

                    // Compute the block's world transform matrix
                    Matrix blockWorld = Matrix.CreateTranslation(blockPosition);

                    // Create the block:
                    blocks[z, x] = new TowerBlock(game, new iVec2(z, x), blockWorld, blockSize);

                    // Set the block based on the pre-designed map
                    blocks[z, x].setTower(createTowerFromInt(predesignedMap[z, x], blockWorld, blockSize));
                }
        }

        public TowerType getTowerTypeOf(Type type)
        {
            if (type == typeof(GrassTower))
                return TowerType.GRASS;
            if (type == typeof(PathTower))
                return TowerType.PATH;
            if (type == typeof(WallTower))
                return TowerType.WALL;
            if (type == typeof(TarTower))
                return TowerType.TAR;

            return TowerType.NONE;
        }

        private Tower createTowerFromInt(int towerTypeID, Matrix world, float size)
        {
            TowerType towerType = (TowerType)towerTypeID;

            switch (towerType)
            {
                case TowerType.GRASS:
                    return new GrassTower(game, world, size);
                case TowerType.PATH:
                    return new PathTower(game, world, size);
                case TowerType.WALL:
                    return new WallTower(game, world, size);
                case TowerType.TAR:
                    return new TarTower(game, world, size);
            }

            throw new Exception("towerTypeID has no corresponding tower type.");
        }

        /// <summary>
        /// Get the world coordinates of a block ID.
        /// </summary>
        public Vector3 coordinatesOf(iVec2 blockId)
        {
            return midPosition +
                new Vector3((float)blockId.y * blockSize * 2 + blockSize,
                    0,
                    (float)blockId.x * blockSize * 2 + blockSize);
        }

        /// <summary>
        /// Returns the block ID of the block at the 'position'.
        /// </summary>
        public iVec2 idOf(Vector3 position)
        {
            return new iVec2(
                (int)((position.X - midPosition.X + world.Translation.X) / blockSize / 2),
                (int)((position.Z - midPosition.Z + world.Translation.Z) / blockSize / 2));
        }

        // Get the Tower Type at the specified index
        public TowerType getTowerTypeAt(int x, int y)
        {
            return getTowerTypeOf(blocks[y, x].tower.GetType());
        }

        public int getGWeight(int x, int y)
        {
            return blocks[y, x].getGWeight();
        }

        public void update(UpdateParams updateParams)
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

                    // A test of creating new towers on click
                    if (updateParams.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                        && blocks[z, x].selected
                        && blocks[z, x].tower.GetType() == typeof(GrassTower))
                        blocks[z, x].tower = new WallTower(game, blocks[z, x].world, blocks[z, x].size);

                    // Perform an update on this block
                    blocks[z, x].update(updateParams);
                }

            boundary.update(updateParams);

            updatePathFinder(updateParams, selectedBlock);
        }

        private void updatePathFinder(UpdateParams updateParams, iVec2 selectedBlock)
        {
            mover.Update(updateParams, this);

            // Place a new target when the right mouse button is pressed
            if (updateParams.mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                // If within bounds and not a wall
                if (selectedBlock.x >= 0 && selectedBlock.x < NUM_BLOCKS && selectedBlock.y >= 0 && selectedBlock.y < NUM_BLOCKS
                    && getTowerTypeAt(selectedBlock.x, selectedBlock.y) != TowerType.WALL)
                {
                    Point startPoint = new Point(
                        (int)(mover.position.X) / (int)blockSize,
                        (int)(mover.position.Y) / (int)blockSize);
                    Point endPoint = new Point(selectedBlock.x, selectedBlock.y);
                    path = pathFinder.FindPath(startPoint, endPoint);
                    mover.addPath(path);
                }
            }
        }

        public void draw(DrawParams drawParams)
        {
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    blocks[z, x].draw(drawParams);
                }

            boundary.draw(drawParams);

            // Draw path finder
            mover.Draw(drawParams);
        }
    }
}
