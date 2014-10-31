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
        TEAPOT,
        TURRET,
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

        // Mark the path with marker spheres
        private bool showDebugPath = true;

        // The terrain on which the towers are placed
        public Terrain terrain;

        // The tree that stores the tanks
        public Quadtree tankTree;

        // The wave manager
        private WaveManager waveManager;

        // A pre-designed map
        private int[,] predesignedMap = new int[NUM_BLOCKS, NUM_BLOCKS]
        {
            {2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {2,3,3,2,5,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {2,3,2,2,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,3,2,2,2,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,2,2,0,0,0,0,1,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,2,2,2,0,1,1,1,2,2,2,2,2,2,3,3,3,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,2,2,2,3,3,3,2,2,2,2},
            {2,2,2,2,2,1,2,2,2,2,5,2,2,3,3,3,2,2,2,2},
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
            {1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,4},
        };

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        public TowerManager(Game game, Matrix world)
            : this(game, world, null, null)
        {

        }

        /// <summary>
        /// Create a new Tower Manager.
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="position">The position of this collection of towers.</param>
        /// <param name="terrain">A pre-initialized terrain height map.</param>
        public TowerManager(Game game, Matrix world, Terrain terrain, Quadtree tankTree)
        {
            this.game = game;
            this.world = world;
            this.terrain = terrain;
            this.tankTree = tankTree;

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

            waveManager = new WaveManager(
                this, new iVec2(0,0), new iVec2(18,19), (MainGame)game, tankTree);
        }

        public void load(ContentManager content)
        {

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
            if (type == typeof(TurretTower))
                return TowerType.TURRET;

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
                case TowerType.TEAPOT:
                    return new TeapotTower(game, world, size);
                case TowerType.TURRET:
                    return new TurretTower(game, world, this, size, tankTree);
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

        public bool insideArea(Vector2 position)
        {
            iVec2 blockId = idOf(position);
            return blockId.x >= 0 && blockId.y >= 0
                && blockId.x < NUM_BLOCKS && blockId.y < NUM_BLOCKS;
        }

        public bool insideArea(Vector3 position)
        {
            return insideArea(new Vector2(position.X, position.Z));
        }

        // Get the Tower Type at the specified index
        public TowerType getTowerTypeAt(int x, int y)
        {
            return getTowerTypeOf(blocks[y, x].tower.GetType());
        }

        public Tower getTowerAt(int x, int y)
        {
            return blocks[y, x].tower;
        }

        public int getGWeight(int x, int y)
        {
            return blocks[y, x].getGWeight();
        }

        public bool intersects(Ray ray)
        {
            // For each block in the 2D array:
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    if(blocks[z,x].tower.collidesWith(ray))
                        return true;
                }
            return false;
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
                    // Create an iVec2 for easy comparison
                    iVec2 cID = new iVec2(x, z);

                    // If this block is the selected block, let it know
                    blocks[z, x].selected = selectedBlock == cID;

                    // Place a tower when the mouse button is down
                    if (updateParams.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                        && blocks[z, x].selected)
                    {
                        // Unfortunately, we must place the tower first, and remove it if it is illegal
                        TowerType oldTower = getTowerTypeAt(x, z);
                        Tower newTower = new WallTower(game, blocks[z, x].world, blocks[z, x].size);
                        placeTower(cID, newTower);

                        if (waveManager.placementAllowedAt(this, cID))
                        {
                            // Update search nodes
                            pathFinder.updateSearchNodes(this);

                            // Update the wave manager
                            waveManager.updatePathfinder();
                            waveManager.calculatePaths();
                        }
                        else
                        {
                            // Placement is not allowed, so we must undo
                            placeTower(cID, createTowerFromInt((int)oldTower, blocks[z, x].world, blocks[z, x].size));
                        }
                    }

                    // Perform an update on this block
                    blocks[z, x].update(updateParams);
                }

            boundary.update(updateParams);

            updatePathFinder(updateParams, selectedBlock);

            waveManager.Update(updateParams);

            // Death map test
            if(updateParams.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.M))
                updateParams.hud.setDeathMap(generateDeathMap());
        }

        public void placeTower(iVec2 cID, Tower tower)
        {
            blocks[cID.y, cID.x].tower = tower;
        }

        private void updatePathFinder(UpdateParams updateParams, iVec2 selectedBlock)
        {
            // Show / hide debug spheres
            if (updateParams.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
                showDebugPath = !showDebugPath;
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

        public void addDeathAt(Vector3 position)
        {
            iVec2 blockID = idOf(position);
            TowerBlock block = blocks[blockID.y, blockID.x];
            block.addDeath();
        }

        public Texture2D generateDeathMap()
        {
            Texture2D bmp = new Texture2D(game.GraphicsDevice, NUM_BLOCKS, NUM_BLOCKS);
            int[,] deathCounts = new int[NUM_BLOCKS,NUM_BLOCKS];
            int highestDeathCount = blocks[0,0].deathCount;

            for (int y = 0; y < NUM_BLOCKS; y++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    deathCounts[y, x] = blocks[y, x].deathCount;

                    if (deathCounts[y, x] > highestDeathCount)
                        highestDeathCount = deathCounts[y, x];
                }

            Color[] normedDeathCounts = new Color[NUM_BLOCKS * NUM_BLOCKS];
            for (int y = 0; y < NUM_BLOCKS; y++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    float luma = (float)deathCounts[y, x] / (float)highestDeathCount;
                    normedDeathCounts[NUM_BLOCKS * y + x] =
                        new Color(luma, luma, luma, .75f);
                }

            bmp.SetData<Color>(normedDeathCounts);
            return bmp;
        }

        public void draw(DrawParams drawParams)
        {
            for (int z = 0; z < NUM_BLOCKS; z++)
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    blocks[z, x].draw(drawParams);
                }

            boundary.draw(drawParams);

            waveManager.Draw(drawParams);
        }
    }
}
