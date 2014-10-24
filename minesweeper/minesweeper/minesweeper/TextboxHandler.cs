using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace minesweeper
{
    class TextboxHandler
    {
        Textbox[] textboxs;
        Vector2[] offsets;
        Vector2 position;
        public Textbox getTextbox(int index)
        {
            return textboxs.ElementAt<Textbox>(index);
        }
        public TextboxHandler(List<Textbox> textboxlist, Vector2 position)
        {
            textboxs = textboxlist.ToArray();
            offsets = new Vector2[textboxs.Length];
            for(int i = 0; i < textboxs.Length; i++)
            {
                offsets[i] = textboxs[i].getPosition();
                //textboxs[i].activated = false;
            }
            offsetall(position);
            this.position = position;
        }
        public void offsetall(Vector2 position)
        {
            int i = 0;
            foreach(Textbox box in textboxs)
            {
                box.setPosition(position+offsets.ElementAt<Vector2>(i));
                i++;
            }
        }
        public void Update()
        {
            /*bool pressed = false;
            for (int i = 0; i < textboxs.Length; i++)
            {
                
                if (textboxs[i].getPressed())
                {
                    textboxs[i].activated = true;
                    break;
                }
            }
             * */
            foreach (Textbox box in textboxs)
            {
                box.activated = box.getSelected();
                box.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Textbox box in textboxs)
            {
                box.Draw(spriteBatch);
            }
        }
    }
}
