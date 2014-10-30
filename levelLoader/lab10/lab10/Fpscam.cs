using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace lab10
{
    class Fpscam:Camera
    {
        float speed = 30;
        float xRotationSpeed = 90;
        float yRotationSpeed = 90;
        public float groundHeight;
        //Vector3 moveDirection;
        //Vector3 velocity;
        //float currentHeight;
        float jumpSpeed = 0;
        Keys forwardKey = Keys.W;
        Keys backwardKey = Keys.S;
        Keys rightKey = Keys.D;
        Keys leftKey = Keys.A;
        Keys jumpKey = Keys.Space;
        MouseState prevMouseState;
        KeyboardState prevkeyboardstate;
        enum Camstate {fps, flight};
        Camstate state = Camstate.flight;
        public Fpscam(Game1 game, Vector3 position, Vector3 target, Vector3 upVector) : base(game, position, target, upVector)
        {
            prevMouseState = Mouse.GetState();
            //if (Game.IsActive)
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2); 
            prevMouseState = Mouse.GetState();
            groundHeight = position.Y;
            prevkeyboardstate = Keyboard.GetState();
        }
        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.C) && !prevkeyboardstate.IsKeyDown(Keys.C))
            {
                if (state == Camstate.fps)
                {
                    state = Camstate.flight;
                }
                else
                {
                    state = Camstate.fps;
                }
            }
            rotateCamera(time);
            moveCamera(time);
            prevkeyboardstate = keyboard;

            //CreateLookAtPoint();

            base.Update(gameTime);
        }
        private void rotateCamera(float time)
        {//turnings a bit jagged can i do it better
            if (Game.IsActive)
            {
                MouseState currMouseState = Mouse.GetState();
                float xdelta = currMouseState.X - prevMouseState.X;
                float ydelta = currMouseState.Y - prevMouseState.Y;
                Matrix rot = Matrix.Identity;
                float offset = 0.5f;//scale by move size
                if (Math.Abs(xdelta) > offset)
                {
                    int xrotdirection = xdelta > 0 ? -1 : 1;
                    rot *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi / 180 * xrotdirection * xRotationSpeed * time);
                }
                if (Math.Abs(ydelta) > offset)
                {
                    int yrotdirection = ydelta > 0 ? -1 : 1;
                    rot *= Matrix.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.Pi / 180 * yrotdirection * yRotationSpeed * time);
                }
                direction = Vector3.Transform(direction, rot);
                //up = Vector3.Transform(up, rot);
                //if(Game.IsActive)
                    Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            }
            //prevMouseState = currMouseState;
        }
        private void moveCamera(float time)
        {// the physics is a bit hacky want to fix

            KeyboardState keyboard = Keyboard.GetState();
            Vector3 dir = Vector3.Normalize(direction);
            dir.Y = 0;
            Vector3 right = Vector3.Normalize(Vector3.Cross(direction, Vector3.Up));
            Vector3 moveDirection = Vector3.Zero;

            if (keyboard.IsKeyDown(forwardKey))
            {
                moveDirection += dir;
            }
            if (keyboard.IsKeyDown(backwardKey))
            {
                moveDirection -= dir;
            }
            if (keyboard.IsKeyDown(rightKey))
            {
                moveDirection += right;
            }
            if (keyboard.IsKeyDown(leftKey))
            {
                moveDirection -= right;
            }
            position += moveDirection * speed * time;
            
            if (keyboard.IsKeyDown(jumpKey) && (position.Y == groundHeight || state == Camstate.flight))
            {
                jumpSpeed = 20;
            }
            if(position.Y > groundHeight && state == Camstate.fps)
            {
                jumpSpeed--;
            }

            if ((position + Vector3.Up * jumpSpeed * time).Y < groundHeight)
            {
                jumpSpeed = 0;
                position.Y = groundHeight;
            }
            position += Vector3.Up * jumpSpeed * time;
            if (state == Camstate.flight)
            {
                jumpSpeed = 0;
            }

        }
    }
}
