using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _116880025_lab04
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private const float JUMP_COEF = 0.08f;

        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 eye { get; protected set; }
        public Vector3 direction { get; protected set; }
        public Vector3 up { get; protected set; }

        private Rectangle window, halfWindow;
        private Boolean jumping = false;
        private float jumpTheta = 0f;
        private float baseHeight;

        private float
            maxPitch = 80f / 180f * (float)Math.PI,
            currentPitch = 0;

        public Camera(Game game, Vector3 eye, Vector3 target, Vector3 up)
            : base(game)
        {
            this.eye = eye;
            this.direction = Vector3.Normalize(target - eye);
            this.up = up;

            baseHeight = eye.Y;

            window = game.GraphicsDevice.PresentationParameters.Bounds;
            halfWindow = window;
            halfWindow.Width /= 2; halfWindow.Height /= 2;

            /* Initialize view and projection matrices */
            CreateLookAt();
            projection = Matrix.CreatePerspectiveFieldOfView(
                65.0f / 180.0f * (float)Math.PI,
                (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height,
                1.0f, 1100.0f);
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
            // Obtain mouse coordinates
            MouseState mouse = Mouse.GetState();

            // Construct yaw rotation matrix by rotating about the positive Y (up) vector
            Matrix rot_yaw =
                Matrix.CreateFromAxisAngle(up, -(float)(mouse.X - halfWindow.Width) / (float)window.Width);

            // Multiply normalized direction vector by yaw rotation matrix
            direction = Vector3.Normalize( Vector3.Transform(direction, rot_yaw) );

            // Limit pitch rotation
            float pitchAngle = (float)(mouse.Y - halfWindow.Height) / (float)window.Height;
            if (Math.Abs(currentPitch + pitchAngle) < maxPitch)
            {
                // Bidirectional vector is orthogonal to the direction and up vectors
                Vector3 biDirection = Vector3.Normalize( Vector3.Cross(up, direction) );

                Matrix rot_pitch =
                    Matrix.CreateFromAxisAngle(biDirection, pitchAngle);
                direction = Vector3.Normalize(Vector3.Transform(direction, rot_pitch));
                currentPitch += pitchAngle;
            }

            // Transform camera through new direction
            transform();

            // Re-build the view matrix
            CreateLookAt();

            // Reset mouse position to mid-window so as to obtain differential coordinates on next update
            Mouse.SetPosition(halfWindow.Width, halfWindow.Height);

            // Call base update routine
            base.Update(gameTime);
        }

        private void transform()
        {
            // Keyboard key states
            KeyboardState ks = Keyboard.GetState();

            // Normalized Y-ignorant direction vector
            Vector3 flatDirection = Vector3.Normalize(new Vector3(
                direction.X,
                0,
                direction.Z));

            // Shift multiplier
            if (ks.IsKeyDown(Keys.LeftShift))
                flatDirection *= 2.0f;

            // Orthonormal direction vector for strafe movement
            Vector3 orthoDirection = Vector3.Cross(up, flatDirection);

            // Collision detection boundaries
            Rectangle bounds = new Rectangle(0, 0, 245, 245);

            Vector3 prevEye = eye;

            if (ks.IsKeyDown(Keys.W))
                eye += flatDirection;
            if (ks.IsKeyDown(Keys.S))
                eye -= flatDirection;
            if (ks.IsKeyDown(Keys.A))
                eye += orthoDirection;
            if (ks.IsKeyDown(Keys.D))
                eye -= orthoDirection;

            // Restore old eye on collision
            if (eye.X > bounds.Width || eye.X < -bounds.Width
                || eye.Z > bounds.Height || eye.Z < -bounds.Height)
                eye = prevEye;

            // Do jump operation
            eye = new Vector3(eye.X, baseHeight + get_jump_y(ks), eye.Z);
        }

        /// <summary>
        /// Computes the jump height at the current frame,
        /// and takes a KeyboardState to update jumping state.
        /// 
        /// This method could have been written by defining a starting acceleration,
        /// and by decrementing that acceleration every update until the object hits the floor.
        /// 
        /// I have instead implemented a more computationally expensive algorithm as
        /// a technical experiment. 
        /// </summary>
        /// <param name="ks"></param>
        /// <returns></returns>
        private float get_jump_y(KeyboardState ks)
        {
            if (jumping)
            {
                if (jumpTheta >= Math.PI - JUMP_COEF * Math.PI)
                    jumping = false;

                return 42f * (float)Math.Pow(Math.Sin(jumpTheta += JUMP_COEF), 1.5);
            }
            else if (ks.IsKeyDown(Keys.Space))
            {
                jumping = true;
                jumpTheta = 0f;
            }

            return 0f;
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(eye, eye + direction, up);
        }

        /// <summary>
        /// Shake the camera using a new thread delegate.
        /// </summary>
        /// <param name="amp">Amplitude of the random modulation</param>
        public void shake(int amp)
        {
            Random r = new Random();

            new Thread(delegate()
            {
                for (int i = 0; i < amp; i++)
                {
                    float shaleDelta = Math.Min(3, r.Next(-amp + i, amp - i) * 0.25f);
                    eye += new Vector3(shaleDelta, 0, shaleDelta * (float)r.Next(-5, 5) / 5f);
                    Thread.Sleep(50);
                }
            }).Start();
        }
    }
}
