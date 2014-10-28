using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace minesweeper
{
    class Level
    {
        private Tile[,] tiles;
        //private List<Texture2D> tileTextures = new List<Texture2D>();
        int scale = 32;
        Vector2 offset = new Vector2(0, 28);
        Vector2 gap = new Vector2(0, 0);
        Texture2D texture;
        Random random = new Random();
        SpriteFont font;
        Texture2D bomb;
        public Level(Texture2D texture, SpriteFont font, Texture2D bomb)
        {
            this.texture = texture;
            this.font = font;
            this.bomb = bomb;
        }
        public void createLevel(int width, int height, int bombs)
        {
            tiles = new Tile[width, height];
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    tiles[x, y] = new Tile(new Rectangle(
                        (int)offset.X + x * (scale + (int)gap.X), (int)offset.Y + y * (scale + (int)gap.Y), scale, scale),
                        texture, font, bomb);
                }
            }
            for (int i = 0; i < bombs; i++)
            {
                int randomX = random.Next(0, tiles.GetLength(0));
                int randomY = random.Next(0, tiles.GetLength(1));
                /*if (!tiles[randomX, randomY].getBomb())
                {
                    tiles[randomX, randomY].setBomb(true);
                }*/
                while (tiles[randomX, randomY].getBomb())
                {
                    randomX = random.Next(0, tiles.GetLength(0));
                    randomY = random.Next(0, tiles.GetLength(1));
                }
                tiles[randomX, randomY].setBomb(true);
            }
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile[,] neighbours = new Tile[3,3];
                    for (int i = 0; i < neighbours.GetLength(0); i++)
                    {
                        for (int j = 0; j < neighbours.GetLength(1); j++)
                        {
                            if (x + i-1 >= 0 && x + i-1 < tiles.GetLength(0) && y + j-1 >= 0 && y + j-1 < tiles.GetLength(1) && (i != 1 || j != 1))
                            {
                                neighbours[i,j] = tiles[x + i-1, y + j-1];
                            }
                            else
                            {
                                neighbours[i, j] = null;
                            }
                        }
                    }
                    tiles[x, y].setNeighbours(neighbours);
                }
            }
        }
        public void Update()
        {
            if (tiles == null) return;
            foreach (Tile tile in tiles)
            {
                tile.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (tiles == null) return;
            foreach (Tile tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}
