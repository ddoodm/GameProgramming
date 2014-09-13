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
        public Vector2 position = Vector2.Zero;
        List<Vector2> path;
        Texture2D marker;
        Texture2D movertexture;
        float time;
        float delay = 0.5f;
        public void addPath(List<Vector2> path)
        {
            this.path = path;
            time = 0;
        }
        public void addTexture(Texture2D marker)
        {
            this.marker = marker;
        }
        public void addMoverTexture(Texture2D movertexture)
        {
            this.movertexture = movertexture;
        }
        public void Update(GameTime gametime)
        {
            if (path.Count > 0)
            {
                time += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (time >= delay)
                {
                    position = path[0];
                    time = 0;
                    path.RemoveAt(0);
                }

            }
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
