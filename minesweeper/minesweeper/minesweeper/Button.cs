using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace minesweeper
{
    class Button
    {
        protected Rectangle rectangle;
        protected Texture2D texture;
        protected MouseState oldMouse;
        //protected Color colour = Color.White;
        protected bool hovered, leftPressed, leftHeldDown, leftToggled, selected, rightPressed, rightHeldDown;
        public bool getHovered(){return hovered;}
        public bool getLeftPressed() { return leftPressed; }
        public bool getLeftHeldDown() { return leftHeldDown; }
        public bool getLeftToggled() { return leftToggled; }
        public bool getSelected() { return selected; }
        public bool getRightPressed() { return rightPressed; }
        public bool getRightHeldDown() { return rightHeldDown; }
        //public bool getRightToggled() { return rightToggled; }

        public Button(Rectangle rectangle, Texture2D texture)
        {
            this.rectangle = rectangle;
            this.texture = texture;

            oldMouse = Mouse.GetState();
        }
        public Vector2 getPosition()
        {
            return new Vector2(rectangle.Location.X, rectangle.Location.Y);
        }
        public void setPosition(Vector2 newPosition)
        {
            rectangle.Location = new Point((int)newPosition.X, (int)newPosition.Y);
        }
        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            hovered = rectangle.Contains(mouse.X, mouse.Y);

            leftHeldDown = hovered && mouse.LeftButton == ButtonState.Pressed;
            leftPressed = leftHeldDown && oldMouse.LeftButton == ButtonState.Released;
            if(leftPressed)leftToggled = !leftToggled;
            if(mouse.LeftButton == ButtonState.Pressed)selected = leftHeldDown;

            rightHeldDown = hovered && mouse.RightButton == ButtonState.Pressed;
            rightPressed = leftHeldDown && oldMouse.RightButton == ButtonState.Released;
            oldMouse = mouse;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Color colour = Color.White;
            //debug colours
            if(hovered) colour = Color.Yellow;
            if(leftHeldDown) colour = Color.Red;
            if(leftPressed) colour = Color.Green;
            spriteBatch.Draw(texture, rectangle, colour);
        }
    }
}
