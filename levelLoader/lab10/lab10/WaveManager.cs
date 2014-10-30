using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace lab10
{
    class WaveManager
    {
        List<Tank> monsters;
        Pathfinder pathfinder;
        Point endPoint;
        Vector3 spawnPosition;
        Game1 game;
        //GameTime time;
        float delay, defaultDelay = 3f;//could make this different for each wave
        float waveDelay, defaultWaveDelay = 5f;//could make this differnt for each wave
        int[] numberToSpawn = { 2, 5, 5, 15 };
        //char[] spawnTypes = {'n', 'f', 'b', 'f'};
        int scale;
        public int wavenum;
        List<Vector3> path;
        Level level;
        public WaveManager(Level level, Point endPoint, Vector3 spawnPosition, Game1 game, int scale, LevelData data)
        {
            numberToSpawn = data.numberToSpawn;
            monsters = new List<Tank>();
            pathfinder = new Pathfinder(level);//mabe just have reference to level and use its path finder
            this.endPoint = endPoint;
            this.spawnPosition = spawnPosition;
            this.game = game;
            this.scale = scale;
            //load wave Manager details from xml
            this.level = level;
            path = getPath(spawnPosition, endPoint);
            
        }
        private List<Vector3> getPath(Vector3 startPosition, Point endPoint)
        {
            Point startPoint = new Point((int)Math.Round(startPosition.X / (scale * 2)), (int)Math.Round(startPosition.Z / (scale * 2)));
            List<Vector2> path = pathfinder.FindPath(startPoint, endPoint);
            List<Vector3> waypoints = new List<Vector3>();
            foreach (Vector2 vec in path)
            {
                waypoints.Add(new Vector3(vec.X * scale * 2, startPosition.Y, vec.Y * scale * 2));
            }
            return waypoints;
        }
        private void addMonster(Vector3 position, float scale, float speed)
        {
            Tank mover = new Tank(Matrix.CreateScale(1 / 120f) * Matrix.CreateTranslation(position), 1.5f);
            mover.Load(game.Content);
            mover.addWayPoint(new List<Vector3>(path));
            //give it path
            monsters.Add(mover);
        }
        public void updatePathfinder()
        {
            pathfinder.updateGWeights(level);
        }
        public bool pathCheck()
        {
            updatePathfinder();
            Point startPoint = new Point((int)Math.Round(spawnPosition.X / (scale * 2)), (int)Math.Round(spawnPosition.Z / (scale * 2)));
            List<Vector2> path = pathfinder.FindPath(startPoint, endPoint);
            return path.Count != 0;
        }
        public void calculatePaths()
        {
            path = getPath(spawnPosition, endPoint);
            foreach (Tank monster in monsters)
            {
                monster.addWayPoint(getPath(monster.position, endPoint));
            }
        }
        public void Update(GameTime time)
        {            //for each wave wavedelay between waves 
            waveDelay +=(float)time.ElapsedGameTime.TotalSeconds;
            if(wavenum < numberToSpawn.Length && waveDelay > defaultWaveDelay)
            {
                if (numberToSpawn[wavenum] > 0)
                {
                    delay += (float)time.ElapsedGameTime.TotalSeconds;
                    if (delay > defaultDelay)
                    {
                        numberToSpawn[wavenum]--;
                        delay = 0;
                        /*switch (spawnTypes[wavenum])
                        {
                            case 'n': addMonster(spawnPosition, 1/100f, 1.0f);
                                break;
                            case 'f': addMonster(spawnPosition, 1 / 200f, 2f);
                                break;
                            case 'b': addMonster(spawnPosition, 1 / 50f, 0.5f);
                                break;
                            //case 'n':
                            //    break;
                        }
                         * */
                        addMonster(spawnPosition, 1 / 120f, 2);
                        //addMonster(spawnPosition);//spawn monster type from allthewaves(currentwavenumber(number to spawn))
                    }
                }
                else
                {
                    waveDelay = 0;
                    wavenum++;
                }

            }
            for (int i = 0; i < monsters.Count; i++ )
            {
                monsters[i].Update(time);
                if (monsters[i].wayPoints.Count == 0)
                {
                    monsters.RemoveAt(i);//could add to a remove list and remove them later so they have time to shoot or somthing
                    i--;
                }
                //if monster.waypoints.count == 0 add to a remove list and if its close enough have it deal damage to the player 
            }
        }

        public void Draw()
        {
            foreach (Entity monster in monsters)
            {
                monster.Draw(game.camera.view, game.camera.projection);
            }
        }
    }
}
