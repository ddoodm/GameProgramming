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

namespace Lab5
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

        float speed = 250;
        int oldScrollValue;


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view Matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            //build camera projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 0.1f, 6000);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public void Update(Viewport viewport, Cube cube, BasicEffect effect)
        {
            //move forwards and backwards
            if (Keyboard.GetState().IsKeyDown(Keys.W))
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

            if (Mouse.GetState().ScrollWheelValue < oldScrollValue)
            {
                cameraPosition += new Vector3(0, 25, 0);
            }
            if (Mouse.GetState().ScrollWheelValue > oldScrollValue)
            {
                cameraPosition += new Vector3(0, -25, 0);
            }
            oldScrollValue = Mouse.GetState().ScrollWheelValue;


            if (cameraPosition.Y < 25)
            {
                cameraPosition.Y = 25;
            }
            if (cameraPosition.Y > 500)
            {
                cameraPosition.Y = 500;
            }

            CreateLookAt();


            

        }

        public Ray MouseRay(Vector2 mouseState, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(mouseState, 0);
            Vector3 farPoint = new Vector3(mouseState, 1);

            nearPoint = viewport.Unproject(nearPoint, projection, view, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, projection, view, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }





        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

            oldScrollValue = Mouse.GetState().ScrollWheelValue;

        }
    }
}