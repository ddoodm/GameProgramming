using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace minesweeper
{
    class Player
    {
        Color colour;
        int score = 0;
        Textbox textbox;
        public Player(Color colour, Textbox textbox)
        {
            this.colour = colour;
            this.textbox = textbox;
            textbox.colour = colour;
        }
        public void Update()
        {
            textbox.setText(score.ToString());
            textbox.Update();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            textbox.Draw(spriteBatch);
        }
    }
}
