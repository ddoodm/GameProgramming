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
        int[,] indexs = new int[,] 
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
            {1,0,1,1,0,0,0,0,0,0,1,0,0,0,0,1,},
            {1,0,0,1,1,0,0,0,0,0,1,0,0,0,0,1,},
            {1,0,0,0,1,0,1,1,0,0,1,1,1,0,1,1,},
            {1,0,0,1,1,0,0,0,0,0,1,0,0,0,0,1,},
            {1,0,1,1,0,0,0,1,1,1,1,0,0,0,0,1,},
            {1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,},
            {1,0,1,1,0,1,1,1,1,0,0,0,1,0,0,1,},
            {1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,1,},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
        };
        Game1 game;
        //BasicModel mover;
        Tank mover;
        public int Width
        {
            get { return indexs.GetLength(1); }
        }
        public int Height
        {
            get { return indexs.GetLength(0); }
        }
        int scale = 5;
        MouseState preMouseState;
        Pathfinder pathfinder;
        Rectangle rect;
        Point endPoint;
        public int GetTerrainCost(int x, int y)
        {
            switch (indexs[y, x])
            {
                case 0: return 1;
                case 1: return 0;
                default: return -1;
            }
        }
        public Level(Game1 game)
        {//
            mover = new Tank(Matrix.CreateScale(1 / 120f) * Matrix.CreateTranslation(new Vector3(2*scale, 10, 2*scale)));
            mover.Load(game.Content);
            /*List<Vector3> path = new List<Vector3>();
            path.Add(new Vector3(1 * scale * 2, 10, 8 * scale * 2));
            path.Add(new Vector3(7 * scale * 2, 10, 8 * scale * 2));
            mover.addWayPoint(path);*/
            //mover.addWayPoint(new Vector3(2 * scale, 10, 8 * scale));
            //mover = new BasicModel(game.Content.Load<Model>(@"Models\spaceship"), new Vector3(0, 10, 0), 1);
            this.game = game;
            preMouseState = Mouse.GetState();
            pathfinder = new Pathfinder(this);
            rect = new Rectangle(0, 0, indexs.GetLength(1)*scale*2, indexs.GetLength(0)*scale*2);
            map = new Cube[indexs.GetLength(1), indexs.GetLength(0)];
            initLevel();
        }
        private void initLevel()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    setCube(j, i);
                }
            }
        }
        private void setCube(int j, int i) 
        {
            switch (indexs[j, i])
            {
                case 0: map[i, j] = new Cube(game, scale, new Vector3(i * scale * 2, scale, j * scale * 2), "Textures/crate(2)");//game.Content.Load<Texture2D>(@"Textures/crate(2)"));
                    break;
                case 1: map[i, j] = new Cube(game, scale, new Vector3(i * scale * 2, scale * 2, j * scale * 2), "Textures/crate(1)");//game.Content.Load<Texture2D>(@"Textures/crate(1)"));
                    break;
            }
        }
        private void moverPathCheck(MouseState currMouseState)
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
                        endPoint = new Point((int)Math.Round(m.Value.X / (scale * 2)), (int)Math.Round(m.Value.Z / (scale * 2)));
                        moverPathCreate();
                    }
                }
            }
        }
        private void moverPathCreate()
        {//save end point in entity
            Point startPoint = new Point((int)Math.Round(mover.position.X / (scale * 2)), (int)Math.Round(mover.position.Z / (scale * 2)));
            if (!(endPoint.X > indexs.GetLength(1) || endPoint.Y > indexs.GetLength(0)))
            {
                List<Vector2> path = pathfinder.FindPath(startPoint, endPoint);
                List<Vector3> waypoints = new List<Vector3>();
                foreach (Vector2 vec in path)
                {
                    waypoints.Add(new Vector3(vec.X * scale * 2, mover.position.Y, vec.Y * scale * 2));
                }
                mover.addWayPoint(waypoints);
            }
        }
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
                            setCube(targetPoint.Y, targetPoint.X);
                            pathfinder.updateGWeights(this);
                            moverPathCreate();
                            /*if (mover.wayPoints.Count > 0)
                            {
                                Point startPoint = new Point((int)Math.Round(mover.position.X / (scale * 2)), (int)Math.Round(mover.position.Z / (scale * 2)));
                                Vector3 pos = mover.wayPoints[mover.wayPoints.Count -1];
                                Point endPoint = new Point();
                                List<Vector2> path = pathfinder.FindPath(startPoint, endPoint)
                                mover.addWayPoint(path);
                            }*/
                        }
                    }
                }

            }
        }
        public void Update(GameTime time)
        {
            MouseState currMouseState = Mouse.GetState();
            moverPathCheck(currMouseState);
            modifieLevel(currMouseState);
            mover.Update(time);
            preMouseState = Mouse.GetState();
        }
        public void Draw()
        {
            //mover.Draw(game.camera);
            mover.Draw(game.camera.view, game.camera.projection);
            foreach (Cube cube in map)
            {
                if (cube != null) cube.Draw();
            }
        }
    }
}
