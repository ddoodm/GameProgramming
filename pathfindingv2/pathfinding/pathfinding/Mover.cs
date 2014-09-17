using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace pathfinding
{
    
    class Mover
    {
        public Vector2 position;
        List<Vector2> path;
        Texture2D marker;
        Texture2D movertexture;
        float time;
        float defaultDelay = 1f;
        float delay = 1f;
        //int age;
        //public Mover(Vector2 position)
        //{
        //    this.position = position;
        //}
        public void addPath(List<Vector2> path)
        {
            this.path = path;
            //time = 0;
        }
        public void addTexture(Texture2D marker)
        {
            this.marker = marker;
        }
        public void addMoverTexture(Texture2D movertexture)
        {
            this.movertexture = movertexture;
        }
        public void Update(GameTime gametime, Level level)
        {
            if (path.Count > 0)
            {

                time += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (time >= delay)
                {
                    position = path[0];
                    time = 0;
                    int index = level.GetIndex((int)path[0].X / 32, (int)path[0].Y / 32);
                    switch (index)
                    {
                        case 1: delay = 0.5f;//road
                            break;
                        case 2: delay = 1;//grass
                            break;
                        case 3: delay = 1.5f;//tar
                            break;
                    }
                    path.RemoveAt(0);
                }

            }
            else delay = defaultDelay;
        }
        public void Draw(SpriteBatch batch)
        {
            DrawPath(batch);
            batch.Draw(movertexture, new Rectangle(
                (int)position.X, (int)position.Y, 32, 32), Color.White);
        }
        private void DrawPath(SpriteBatch batch)
        {
            foreach (Vector2 v in path)
            {
                batch.Draw(marker, new Rectangle(
                    (int)v.X, (int)v.Y, 32, 32), Color.White);
            }
        }

 
    }
}
