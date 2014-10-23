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

        public const int
            NUM_BLOCKS = 20;        // The number of tower blocks in both axes.
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

        // Mark the path with marker spheres
        private bool showDebugPath = true;

        // The terrain on which the towers are placed
        public Terrain terrain;

        // A pre-designed map
        private int[,] predesignedMap = new int[NUM_BLOCKS, NUM_BLOCKS]
        {
            {2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {2,3,3,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {2,3,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,3,2,2,2,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,2,2,0,0,0,0,1,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,2,2,2,0,1,1,1,2,2,2,2,2,2,3,3,3,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,2,3,3,3,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,2,3,3,3,2,2,2,2},
            {2,2,2,2,2,1,1,1,1,2,2,2,2,3,3,3,2,2,2,2},
            {1,1,1,1,1,1,2,2,1,2,2,1,1,1,3,2,2,2,2,2},
            {2,2,2,2,2,1,2,2,1,2,2,1,2,3,3,2,2,2,2,2},
            {2,3,3,2,2,1,2,2,1,1,1,1,2,3,2,2,2,2,2,2},
            {2,3,2,2,2,1,2,2,2,2,2,2,2,3,2,0,2,2,2,2},
            {3,3,2,2,0,1,1,1,2,2,2,2,2,3,2,0,0,0,0,2},
            {3,2,2,0,0,0,0,1,2,2,2,2,3,3,2,0,2,2,2,2},
            {3,2,2,2,2,1,1,1,2,2,2,2,3,3,2,0,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,3,2,2,0,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,3,2,2,0,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,2,3,2,2,2,2,2,2},
            {1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
        };

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        public TowerManager(Game game, Matrix world)
            : this(game, world, null)
        {

        }

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        /// <param name="terrain">A pre-initialized terrain height map.</param>
        public TowerManager(Game game, Matrix world, Terrain terrain)
        {
            this.game = game;
            this.world = world;
            this.terrain = terrain;

            midPosition = world.Translation - new Vector3(
                blockSize * NUM_BLOCKS,
                0f,
                blockSize * NUM_BLOCKS);

            initializeBlocks();

            selectionManager = new TowerSelectionManager(this);

            boundary = new PlaneEntity(game, new WireframePlanePrimitive(
                game, blockSize * NUM_BLOCKS, Vector3.Up), world.Translation + Vector3.Up * 15f, 0);
            boundary.primitive.diffuseColour = new Vector3(1f, 0f, 0f);

            // Set up path finding objects
            pathFinder = new TowerPathFinder(this);
            mover = new TowerTraverser();
        }

        public void load(ContentManager content)
        {
            // Create the entity that will traverse the terrain
            Tank tank = new Tank(game, ((MainGame)game).world, this); // TODO: Very bad hack. Do not do this.
            tank.load(content);
            tank.npc = new NPC(game, tank);
            tank.npc.steering = new Seek();
            ((Seek)tank.npc.steering).targetRadius = blockSize;
            tank.npc.steering.maxSpeed = 50;
            tank.npc.steering.maxAcceleration = 200;
            mover.setMover(tank);

            mover.setMarker(new StaticModel(game, content.Load<Model>("Models\\DSphere")));
        }

        /// <summary>
        /// Initialize each TowerBlock and provide its coordinates.
        /// </summary>
        private void initializeBlocks()
        {
            for (int z = 0; z < NUM_BLOCKS; z++)
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
            Vector2 coords2d = this.coordinatesOf2D(blockId);

            float Y = 0;
            if (terrain != null)
                Y = terrain.getYAt(coords2d);

            return new Vector3(coords2d.X, Y, coords2d.Y);
        }

        /// <summary>
        /// Get the world coordinates of a block ID.
        /// </summary>
        public Vector2 coordinatesOf2D(iVec2 blockId)
        {
            return
                new Vector2(
                    midPosition.X + (float)blockId.y * blockSize * 2 + blockSize,
                    midPosition.Z + (float)blockId.x * blockSize * 2 + blockSize);
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

        /// <summary>
        /// Returns the block ID of the block at the 'position'.
        /// </summary>
        public iVec2 idOf(Vector2 position)
        {
            return idOf(new Vector3(position.X, 0, position.Y));
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
                    {
                        // Place the tower
                        blocks[z, x].tower = new WallTower(game, blocks[z, x].world, blocks[z, x].size);

                        // Update search nodes
                        pathFinder.updateSearchNodes(this);

                        // Re-pathfind
                        mover.rePathfind(this, pathFinder);
                    }

                    // Perform an update on this block
                    blocks[z, x].update(updateParams);
                }

            boundary.update(updateParams);

            updatePathFinder(updateParams, selectedBlock);
        }

        private void updatePathFinder(UpdateParams updateParams, iVec2 selectedBlock)
        {
            // Show / hide debug spheres
            if (updateParams.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
                showDebugPath = !showDebugPath;
            mover.showDebugPath = showDebugPath;

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

        public void setPathFinderStartNode(iVec2 startBlock)
        {
            mover.position = new Vector2(startBlock.x, startBlock.y) * blockSize;
        }

        /// <summary>
        /// Collision detect the sphere against all Towers in this Tower Manager.
        /// </summary>
        /// <param name="sphere">The sphere to collide with</param>
        /// <returns>True if an intersection was detected</returns>
        public bool towersCollideWith(BoundingSphere sphere)
        {
            foreach (TowerBlock block in blocks)
                if (block.tower != null && block.tower.collidesWith(sphere))
                    return true;
            return false;
        }

        /// <summary>
        /// Collision detect the box against all Towers in this Tower Manager.
        /// </summary>
        /// <param name="box">The box to collide with</param>
        /// <returns>True if an intersection was detected</returns>
        public bool towersCollideWith(BoundingBox box)
        {
            foreach (TowerBlock block in blocks)
                if (block.tower != null && block.tower.collidesWith(box))
                    return true;
            return false;
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
