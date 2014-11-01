using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Dan's enemy wave controller code
    /// </summary>
    public class WaveManager
    {
        List<TowerTraverser> monsters;
        public TowerPathFinder pathfinder;
        iVec2 startPoint, endPoint;
        MainGame game;
        //GameTime time;
        float delay, defaultDelay = 4f;//could make this different for each wave
        float waveDelay = 14f, defaultWaveDelay = 15f;//could make this differnt for each wave
        int[] numberToSpawn = { 2, 5, 5, 15 };
        public int wavenum;
        List<Vector2> path;
        TowerManager level;
        Quadtree quadtree;
        int idCounter = 0;

        public WaveManager(TowerManager level, iVec2 startPoint, iVec2 endPoint, MainGame game, Quadtree quadtree, LevelDescription levelDescription)
        {
            monsters = new List<TowerTraverser>();
            pathfinder = new TowerPathFinder(level);//mabe just have reference to level and use its path finder
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.game = game;
            //load wave Manager details from xml
            this.level = level;
            this.quadtree = quadtree;
            this.numberToSpawn = levelDescription.numberToSpawn;
            path = getPath(startPoint, endPoint);
        }

        private List<Vector2> getPath(iVec2 startPoint, iVec2 endPoint)
        {
            return pathfinder.FindPath(startPoint, endPoint);
        }

        private void addMonster(ContentManager content, TowerTraverser traverser)
        {
            // Create the entity that will traverse the terrain
            Tank tank = new Tank(game, ((MainGame)game).world, level, traverser, idCounter++); // TODO: Very bad hack. Do not do this.
            tank.load(content);
            tank.npc.steering = new Seek();
            ((Seek)tank.npc.steering).targetRadius = TowerManager.blockSize;
            tank.npc.steering.maxSpeed = 100;
            tank.npc.steering.maxAcceleration = 200;

            // Set position to start of path
            tank.npc.kinematic.position = level.coordinatesOf(new iVec2(path[0] / TowerManager.blockSize));

            // THEN set the mover (for the quadtree to insert correctly)
            traverser.setMover(tank);

            traverser.addPath(new List<Vector2>(path));
            //give it path
            monsters.Add(traverser);
        }

        public void updatePathfinder()
        {
            pathfinder.updateSearchNodes(level);
        }

        public bool canPathfind()
        {
            try
            {
                updatePathfinder();
                List<Vector2> path = pathfinder.FindPath(startPoint, endPoint);
                return path.Count > 0;
            }
            catch (Exception e)
            {
                // If pathfinding caused an exception,
                // we certainly cannot allow the user to place a block here!
                return false;
            }
        }

        public void calculatePaths()
        {
            path = getPath(startPoint, endPoint);
            foreach (TowerTraverser monster in monsters)
                monster.pathfindTo(endPoint, level, pathfinder);
        }

        /// <summary>
        /// Is the user permitted to place a block at 'cid'?
        /// </summary>
        public bool placementAllowedAt(TowerManager level, iVec2 cid)
        {
            // Be sure that it is still possible to pathfind
            if (!canPathfind())
                return false;

            // Cannot place on start or end nodes
            if (startPoint == cid || endPoint == cid)
                return false;

            // Check each mover
            foreach (TowerTraverser traverser in monsters)
                if (traverser.currentBlock(level) == cid
                    || traverser.nextBlock(level) == cid)
                    return false;
            return true;
        }

        /// <summary>
        /// Remove a tower traverser from the list of traversers
        /// </summary>
        public void remove(TowerTraverser traverser)
        {
            monsters.Remove(traverser);
        }

        public void Update(UpdateParams updateParams)
        {
            //for each wave wavedelay between waves 
            waveDelay += (float)updateParams.gameTime.ElapsedGameTime.TotalSeconds;
            if (wavenum < numberToSpawn.Length && waveDelay > defaultWaveDelay)
            {
                if (numberToSpawn[wavenum] > 0)
                {
                    delay += (float)updateParams.gameTime.ElapsedGameTime.TotalSeconds;
                    if (delay > defaultDelay)
                    {
                        numberToSpawn[wavenum]--;
                        delay = 0;

                        TowerTraverser newTraverser = new TowerTraverser(quadtree, level, this);
                        addMonster(game.Content, newTraverser);
                    }
                }
                else
                {
                    waveDelay = 0;
                    wavenum++;
                }
            }
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].Update(updateParams, level);
                if (monsters[i].pathLength() == 0)
                {
                    monsters[i].destroy();
                    monsters.RemoveAt(i);//could add to a remove list and remove them later so they have time to shoot or somthing
                    i--;
                }
                //if monster.waypoints.count == 0 add to a remove list and if its close enough have it deal damage to the player 
            }
        }

        public List<Entity> getEntities()
        {
            List<Entity> entities = new List<Entity>();
            foreach (TowerTraverser traverser in monsters)
                entities.Add(traverser.mover);
            return entities;
        }

        public void Draw(DrawParams drawParams)
        {
            foreach (TowerTraverser monster in monsters)
                monster.Draw(drawParams);
        }
    }
}
