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


namespace Lab03_3DDemo
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private MouseState pMouse;
        public Matrix view          { get; protected set; }
        public Matrix projection    { get; protected set; }
        public Vector3 eye          { get; protected set; }
        public Vector3 target       { get; protected set; }
        public Vector3 up           { get; protected set; }

        public Camera(Game game, Vector3 eye, Vector3 target, Vector3 up)
            : base(game)
        {
            this.eye = eye; this.target = target; this.up = up;

            /* Initialize view and projection matrices */
            view = Matrix.CreateLookAt(eye, target, up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                60.0f / 180.0f * (float)Math.PI,
                (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height,
                0.1f, 100.0f);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (pMouse == null)
                pMouse = Mouse.GetState();
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Released
                /*&& mouse.ScrollWheelValue == pMouse.ScrollWheelValue*/)
                goto _END_UPDATE; // Nasty hack !!

            eye = new Vector3(
                mouse.X / (float)base.Game.GraphicsDevice.PresentationParameters.Bounds.Width * 20f - 10f,
                mouse.Y / (float)base.Game.GraphicsDevice.PresentationParameters.Bounds.Height * 20f - 10f,
                eye.Z /*15f + (float)Mouse.GetState().ScrollWheelValue * -0.025f*/);
            view = Matrix.CreateLookAt(eye, target, up);

            pMouse = mouse;

_END_UPDATE: // Very cheeky
            base.Update(gameTime);
        }
    }
}
