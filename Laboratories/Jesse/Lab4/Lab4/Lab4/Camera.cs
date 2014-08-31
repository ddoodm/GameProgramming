using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lab4
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera Matricies
        public Matrix view;
        public Matrix projection;

        //Camera vectors
        public Vector3 cameraPosition;
        public Vector3 cameraDirection;
        public Vector3 cameraUp;

        float speed = 2;
        MouseState oldMouseState;
        float totalPitch = MathHelper.PiOver4;
        float currentPitch = 0;

        float gravity = -0.3f;
        float jumpVel = 5.0f;
        bool jumping = false;


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up) : base(game)
        {
            // Build camera view Matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            //build camera projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 0.1f, 1000);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public void Update()
        {
            //move forwards and backwards
            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                cameraPosition += new Vector3(cameraDirection.X, 0, cameraDirection.Z) * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                cameraPosition -= new Vector3(cameraDirection.X, 0, cameraDirection.Z) * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                cameraPosition += Vector3.Cross(cameraUp, new Vector3(cameraDirection.X, 0, cameraDirection.Z)) * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                cameraPosition -= Vector3.Cross(cameraUp, new Vector3(cameraDirection.X, 0, cameraDirection.Z)) * speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if(jumping == false)
                {
                    jumping = true;
                }
            }
            //code for jumping, it checks whether player is jumping and then adds the jump velocity to the camera and adds more gravity over time
            if(jumping == true)
            {
                cameraPosition.Y += (jumpVel);
                jumpVel += gravity;
                
                //stops jumping and resets jumpvel and camera position
                if(cameraPosition.Y <= 10)
                {
                    jumping = false;
                    cameraPosition.Y = 10;
                    jumpVel = 5.0f;
                }
            }


            CreateLookAt();
            
            //Camera YAW
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Up, (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - oldMouseState.X)));

            //Camera Pitch
            float pitchAngle = (MathHelper.PiOver4 / 150) * (Mouse.GetState().Y - oldMouseState.Y);

            //this ensures that the camera cannot look too high or low
            if(Math.Abs(currentPitch + pitchAngle) < totalPitch)
            {
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), pitchAngle));
                currentPitch += pitchAngle;
            }

            
            //this locks the mouse to the center of the window
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();



        }


        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();

        }
    }
}