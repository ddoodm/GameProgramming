using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pathfinding
{

    class Level
    {
        int[,] map = new int[,] 
        {
            /*
            {0,0,1,0,0,0,0,0,},
            {0,0,1,1,0,0,0,0,},
            {0,0,0,1,1,0,0,0,},
            {0,0,0,0,1,0,0,0,},
            {0,0,0,1,1,0,0,0,},
            {0,0,1,1,0,0,0,0,},
            {0,0,1,0,0,0,0,0,},
            {0,0,1,1,0,1,1,1,},
            {0,0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,0,},
            */            
            {2,2,2,2,2,1,2,2,},
            {2,3,3,2,2,1,2,2,},
            {2,3,2,2,2,1,2,2,},
            {3,3,2,2,2,1,1,1,},
            {3,2,2,0,0,0,0,1,},
            {3,2,2,2,2,1,1,1,},
            {2,2,2,2,2,1,2,2,},
            {2,2,2,2,2,1,2,2,},
            {2,2,2,2,2,1,2,2,},
            {1,1,1,1,1,1,2,2,},


        };
        private List<Texture2D> tileTextures = new List<Texture2D>();
        public void AddTexture(Texture2D texture)
        {
            tileTextures.Add(texture);
        }
        public int Width
        {
            get { return map.GetLength(1); }
        }
        public int Height
        {
            get { return map.GetLength(0); }
        }
        public int GetIndex(int x, int y)
        {
            return map[y, x];
        }
        public int getGWeight(int index)
        {
            switch (index)
            {
                case 1: return 1;//road
                case 2: return 3;//grass
                case 3: return 10;//tar
                default: return 0;
            }
        }
        public void Draw(SpriteBatch batch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int textureIndex = map[y, x];
                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];
                    batch.Draw(texture, new Rectangle(
                        x * 32, y * 32, 32, 32), Color.White);
                }
            }
        }
        
    }
}
