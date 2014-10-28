using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace minesweeper
{
    class Textbox:Button
    {
        private KeyboardState oldKeyboard;
        private String text;
        public String getText() { return text; }
        public void setText(String text) { this.text = text; }
        private SpriteFont font;
        public bool activated = false;
        private int charaterLimit;
        private string whiteList, blackList;
        public Color colour = Color.White;
        public Textbox(Rectangle rectangle, SpriteFont font, String text, int charaterLimit, string whiteList, string blackList, Texture2D texture):base(rectangle, texture)
        {
            this.font = font;
            this.text = text;
            this.charaterLimit = charaterLimit;
            this.whiteList = whiteList;
            this.blackList = blackList;
            oldKeyboard = Keyboard.GetState();
        }
        public Textbox(Rectangle rectangle, SpriteFont font) : this(rectangle, font, "", -1, null, null, null) { }
        public void Update()
        {
            if (activated)
            {
                KeyboardState keyboard = Keyboard.GetState();
                if (charaterLimit != -1 && text.Length < charaterLimit)
                {
                    char key;
                    if (TryConvertKeyboardInput(keyboard, oldKeyboard, out key))
                    {   
                        if((whiteList == null || whiteList.Contains(key)) && (blackList == null ||!blackList.Contains(key)))
                        {
                            text += key;
                        }
                    }
                }
                if (keyboard.IsKeyDown(Keys.Back) && !oldKeyboard.IsKeyDown(Keys.Back) && text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }

                oldKeyboard = Keyboard.GetState();
            }
            base.Update();
        }
        /// <summary>
        /// modified from http://roy-t.nl/index.php/2010/02/11/code-snippet-converting-keyboard-input-to-text-in-xna/
        /// by Roy Triesscheijn
        /// Tries to convert keyboard input to characters and prevents repeatedly returning the 
        /// same character if a key was pressed last frame, but not yet unpressed this frame.
        /// </summary>
        /// <param name="keyboard">The current KeyboardState</param>
        /// <param name="oldKeyboard">The KeyboardState of the previous frame</param>
        /// <param name="key">When this method returns, contains the correct character if conversion succeeded.
        /// Else contains the null, (000), character.</param>
        /// <returns>True if conversion was successful</returns>
        public static bool TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key)
        {
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                int i = 0;
                /*detect new key presses while holding a lower key
                 * for (; i < keys.Length; i++)
                {
                    if (!oldKeyboard.IsKeyDown(keys[i]))
                    {
                        break;
                    }
                }*/
                switch (keys[i])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return true;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;

                    //Special keys
                    case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return true;
                    case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return true;
                    case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return true;
                    case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return true;
                    case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return true;
                    case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return true;
                    case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return true;
                    case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                    case Keys.Enter: key = '\n'; return true;
                }
            }

            key = (char)0;
            return false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {//use charater limit for width or if no limit the number/width of characters height height of characters*number of lines
            //debug colours
            //if (hovered) colour = Color.Yellow;
            //if(selected)colour = Color.Red;
            if(texture != null)spriteBatch.Draw(texture, rectangle, colour);
            //rectangle.
            Vector2 position = new Vector2(rectangle.X, rectangle.Y);
            spriteBatch.DrawString(font, text, position + new Vector2(2, 1), Color.DarkGray);
            spriteBatch.DrawString(font, text, position, Color.Black);
        }
    }
}
