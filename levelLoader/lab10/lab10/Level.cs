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
    class Level
    {
        Cube[,] map;
        //Vector2 offset;
        int[,] indexs;/* = new int[,] 
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
        };*/
        Game1 game;
        //BasicModel mover;
        int scale = 5;
        MouseState preMouseState;
        Rectangle rect;
        WaveManager waves;
        public int Width
        {
            get { return indexs.GetLength(1); }
        }
        public int Height
        {
            get { return indexs.GetLength(0); }
        }

        public int GetTerrainCost(int x, int y)
        {
            switch (indexs[y, x])
            {
                case 0: return 1;
                case 1: return 0;
                case 2: return 1;
                default: return -1;
            }
        }
        public Level(Game1 game, String fileName)
        {//

            LevelData data = LevelLoader.Load(fileName, game.Content);
            indexs = data.indexs;

            this.game = game;
            preMouseState = Mouse.GetState();
            rect = new Rectangle(0, 0, indexs.GetLength(1)*scale*2, indexs.GetLength(0)*scale*2);
            map = new Cube[indexs.GetLength(1), indexs.GetLength(0)];
            Point endPoint = initLevel();
            Vector3 spawnPosition = new Vector3(2 * scale, 10, 2 * scale);
            waves = new WaveManager(this, endPoint, spawnPosition, game, scale, data);
        }
        private Point initLevel()
        {
            Point endPoint = new Point();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    setCube(j, i);
                    if (indexs[j, i] == 2)//there are better ways of doing this
                    {
                        endPoint = new Point(i, j);
                    }
                }
            }
            return endPoint;
        }
        private void setCube(int j, int i) 
        {
            switch (indexs[j, i])
            {
                case 0: map[i, j] = new Cube(game, scale, new Vector3(i * scale * 2, scale, j * scale * 2), "Textures/crate(2)");//game.Content.Load<Texture2D>(@"Textures/crate(2)"));
                    break;
                case 1: map[i, j] = new Cube(game, scale, new Vector3(i * scale * 2, scale * 2, j * scale * 2), "Textures/crate(1)");//game.Content.Load<Texture2D>(@"Textures/crate(1)"));
                    break;
                case 2: map[i, j] = new Cube(game, scale, new Vector3(i * scale * 2, scale, j * scale * 2), "Textures/crate(3)");
                    break;
            }
        }
        /*private void moverPathCheck(MouseState currMouseState)
        {
            if (currMouseState.LeftButton == ButtonState.Pressed && preMouseState.LeftButton != ButtonState.Pressed)
            {
                Vector3? m = MouseHelper.mousepickingPlane(game, new Plane(Vector3.Up, -scale));
                if (m != null)
                {
                    Point point = new Point((int)m.Value.X, (int)m.Value.Z);
                    if (rect.Contains(point))
                    {//(new Point((int)Math.Ceiling(mover.position.X) / (scale * 2), (int)Math.Ceiling(mover.position.X) / (scale * 2)), new Point(Math.Round(point.X / (scale * 2)), (point.Y / (scale * 2))));

                        //if a wall wasnt clicked on
                        mover.endPoint = new Point((int)Math.Round(m.Value.X / (scale * 2)), (int)Math.Round(m.Value.Z / (scale * 2)));
                        moverPathCreate();
                    }
                }
            }
        }*/
        /*private void moverPathCreate()
        {//save end point in entity
            Point startPoint = new Point((int)Math.Round(mover.position.X / (scale * 2)), (int)Math.Round(mover.position.Z / (scale * 2)));
            if (!(mover.endPoint.X > indexs.GetLength(1) || mover.endPoint.Y > indexs.GetLength(0)))
            {
                List<Vector2> path = pathfinder.FindPath(startPoint, mover.endPoint);
                List<Vector3> waypoints = new List<Vector3>();
                foreach (Vector2 vec in path)
                {
                    waypoints.Add(new Vector3(vec.X * scale * 2, mover.position.Y, vec.Y * scale * 2));
                }
                mover.addWayPoint(waypoints);
            }
        }*/
        private void modifieLevel(MouseState currMouseState)
        {
            if (currMouseState.RightButton == ButtonState.Pressed && preMouseState.RightButton != ButtonState.Pressed)
            {
                Vector3? m = MouseHelper.mousepickingPlane(game, new Plane(Vector3.Up, -scale));
                if (m != null)
                {
                    Point point = new Point((int)m.Value.X, (int)m.Value.Z);
                    if (rect.Contains(point))
                    {
                        Point targetPoint = new Point((int)Math.Round(m.Value.X / (scale * 2)), (int)Math.Round(m.Value.Z / (scale * 2)));
                        if (!(targetPoint.X > indexs.GetLength(1) || targetPoint.Y > indexs.GetLength(0)))
                        {
                            switch (indexs[targetPoint.Y, targetPoint.X])
                            {
                                case 0: indexs[targetPoint.Y, targetPoint.X] = 1;
                                    break;
                                case 1: indexs[targetPoint.Y, targetPoint.X] = 0;
                                    break;
                            }
                            
                            bool flag = waves.pathCheck();
                            if (flag)
                            {
                                waves.updatePathfinder();
                                waves.calculatePaths();
                                setCube(targetPoint.Y, targetPoint.X);
                            }
                            else
                            {
                                switch (indexs[targetPoint.Y, targetPoint.X])
                                {
                                    case 0: indexs[targetPoint.Y, targetPoint.X] = 1;
                                        break;
                                    case 1: indexs[targetPoint.Y, targetPoint.X] = 0;
                                        break;
                                }
                                waves.updatePathfinder();
                                //add a on screen message to tell them they blocked the only path
                            }                            
                        }
                    }
                }

            }
        }
        public void Update(GameTime time)
        {
            MouseState currMouseState = Mouse.GetState();
           // moverPathCheck(currMouseState);
            modifieLevel(currMouseState);
            //mover.Update(time);
            waves.Update(time);
            preMouseState = Mouse.GetState();
        }
        public void Draw()
        {
            //mover.Draw(game.camera);
            //mover.Draw(game.camera.view, game.camera.projection);
            waves.Draw();
            foreach (Cube cube in map)
            {
                if (cube != null) cube.Draw();
            }
        }
    }
}
