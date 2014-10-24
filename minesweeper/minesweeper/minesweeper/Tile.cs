using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace minesweeper
{
    class Tile:Button
    {
        protected bool bomb = false;
        protected bool clicked = false;//do i need this?
        public bool revealed = false;
        public bool getBomb(){return bomb;}
        public void setBomb(bool bomb){ this.bomb = bomb;}
        protected SpriteFont font;
        protected string text; 
        protected Tile[,] neighbours;
        public void setNeighbours(Tile[,] neighbours) { this.neighbours = neighbours; }
        Texture2D bombTexture;
        public Tile(Rectangle rectangle, Texture2D texture, SpriteFont font,Texture2D bombTexture):base(rectangle, texture)
        {
            this.font = font;
            this.bombTexture = bombTexture;
        }
        public void Update()
        {
            clicked = clicked || leftPressed;
            if (clicked & !bomb) reveal();
            base.Update();
        }
        private void reveal()
        {
            revealed = true;
            int count = 0;
            foreach (Tile tile in neighbours)
            {
                if (tile != null && tile.bomb)
                {
                    count++;
                }
            }
            if (count == 0)
            {
                foreach (Tile tile in neighbours)
                {
                    if (tile != null && !tile.revealed)
                    {
                        tile.reveal();
                    }
                }
            }
            else
            {
                text = count.ToString();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Color colour = Color.White;
            spriteBatch.Draw(texture, rectangle, colour);
            if (bomb && clicked) spriteBatch.Draw(bombTexture, rectangle, Color.White);
            if (revealed)
            {
                if (text != null)
                {
                    Vector2 position = new Vector2(rectangle.X, rectangle.Y);
                    spriteBatch.DrawString(font, text, position + new Vector2(12, 4), Color.DarkGray);
                    spriteBatch.DrawString(font, text, position + new Vector2(10, 3), Color.Black);
                }
                else
                {
                    Vector2 position = new Vector2(rectangle.X, rectangle.Y);
                    spriteBatch.DrawString(font, "0", position + new Vector2(12, 4), Color.DarkGray);
                    spriteBatch.DrawString(font, "0", position + new Vector2(10, 3), Color.Black);
                }
            }
        }
    }
}
