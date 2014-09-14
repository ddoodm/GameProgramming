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


namespace _11697822_lab06
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {   
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 position;
        Vector3 direction;
        Vector3 upVector;

        MouseState prevMouse;
        float speed = 40;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            position = pos;
            direction = target - pos;
            direction.Normalize();
            upVector = up;
            CreateLookAtPoint();

            //view = Matrix.CreateLookAt(pos, target, up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1f, 1000);
        }

        public void CreateLookAtPoint()
        {
            view = Matrix.CreateLookAt(position, position + direction, upVector);
        }
        
        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouse = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                position -= direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                position += Vector3.Cross(upVector, direction) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                position -= Vector3.Cross(upVector, direction) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //direction = Vector3.Transform(direction, 
            //    Matrix.CreateFromAxisAngle(upVector, (-MathHelper.PiOver4 / 150) *
            //    (Mouse.GetState().X - prevMouse.X)));

            //direction = Vector3.Transform(direction,
            //    Matrix.CreateFromAxisAngle(Vector3.Cross(upVector, direction),
            //    (MathHelper.PiOver4 / 100) *
            //    (Mouse.GetState().Y - prevMouse.Y)));

            prevMouse = Mouse.GetState();

            CreateLookAtPoint();
            base.Update(gameTime);
        }
    }
}
