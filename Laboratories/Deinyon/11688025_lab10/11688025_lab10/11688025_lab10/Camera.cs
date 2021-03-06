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

namespace _11688025_lab10
{
    /// <summary>
    /// Data structure that defines the position and orientation
    /// of a camera.
    /// </summary>
    public struct CameraTuple
    {
        public Vector3 position;
        public Vector3 target;
        public Vector3 up;
    }

    /// <summary>
    /// A static camera which does not update per-frame.
    /// The Camera should be extended in order to provide
    /// per-frame updates in the desired form.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// The composited view and projection matrices
        /// ready for per-vertex multiplication by the
        /// shader program.
        /// </summary>
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        /// <summary>
        /// The camera's orientation description in
        /// Cartesian space.
        /// </summary>
        //public Vector3 position { get; protected set; }
        public Vector3 direction { get; protected set; }
        public Vector3 up { get; protected set; }

        /// <summary>
        /// The camera's physical state
        /// (position, velocity, and orientation)
        /// </summary>
        public Kinematic kinematic;
        public Steering steering;

        public Vector3 position { get { return kinematic.position; } }

        /// <summary>
        /// Camera translation speed settings
        /// </summary>
        public float
            maxSpeed = 500f,
            shiftMultiplier = 2f,
            friction = 0.95f;

        /// <summary>
        /// Perspective projection parameters
        /// </summary>
        public float
            near = 1.0f,        // The near plane
            far = 2000.0f,      // The far plane
            fov = 65.0f;        // Field of View

        /// <summary>
        /// Describes the window dimensions.
        /// 'halfWindow' is the (window size / 2).
        /// </summary>
        protected Rectangle window, halfWindow;

        /// <summary>
        /// The graphics device's viewport
        /// </summary>
        protected Viewport viewport;

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {
            this.direction = Vector3.Normalize(target - position);
            this.up = up;
            this.viewport = game.GraphicsDevice.Viewport;

            // Initialize physical state
            kinematic = new Kinematic(position);
            steering = new Steering();

            // Store window dimensions for later calculation
            window = game.GraphicsDevice.PresentationParameters.Bounds;
            halfWindow = window;
            halfWindow.Width /= 2; halfWindow.Height /= 2;

            /* Initialize view and projection matrices */
            createLookAt();
            createPerspectiveProjection();
        }

        public Camera(Game game, CameraTuple tuple)
            : this(game, tuple.position, tuple.target, tuple.up)
        { }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update Kinematic
            kinematic.update(steering, (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            // Apply friction
            if(kinematic.velocity.Length() > 0)
                kinematic.velocity *= friction;

            // Call base update routine
            base.Update(gameTime);
        }

        /// <summary>
        /// Rebuild the view matrix
        /// </summary>
        protected void createLookAt()
        {
            //view = Matrix.CreateLookAt(position, position + direction, up);
            view = Matrix.CreateLookAt(kinematic.position, kinematic.position + direction, up);
        }

        /// <summary>
        /// Rebuild the projection matrix
        /// </summary>
        protected void createPerspectiveProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(
                fov / 180.0f * (float)Math.PI,
                (float)window.Width / (float)window.Height,
                near, far);
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
                    kinematic.position += new Vector3(shaleDelta, 0, shaleDelta * (float)r.Next(-5, 5) / 5f);
                    Thread.Sleep(50);
                }
            }).Start();
        }

        /// <summary>
        /// Unprojects the mouse position as a 3D ray.
        /// </summary>
        /// <param name="mouseState">The mouse state</param>
        /// <returns>The 3D unprojected mouse ray</returns>
        public Ray getMouseRay(MouseState mouseState)
        {
            // Obtain mouse coordinates in Cartesian screen space as they
            // lie on the near and far planes
            Vector2 mouse = new Vector2(mouseState.X, mouseState.Y);
            Vector3 nearMouse = new Vector3(mouse, 0);
            Vector3 farMouse = new Vector3(mouse, 1);

            // Perform a reverse perspective projection on the mouse coordinates
            // and obtain a near-far vector delta
            Vector3 near = viewport.Unproject(
                nearMouse, projection, view, Matrix.Identity);
            Vector3 far = viewport.Unproject(
                farMouse, projection, view, Matrix.Identity);

            // Obtain a ray that projects from the near mouse to the far mouse
            Vector3 direction = Vector3.Normalize(far - near);
            return new Ray(near, direction);
        }
    }

    /// <summary>
    /// A First-Person camera which responds to key-presses and mouse input.
    /// Camera pitch is constrained to -179' to +179'
    /// </summary>
    public class FPCamera : Camera
    {
        /// <summary>
        /// Dan's jump height variables
        /// </summary>
        /*
        private float jumpSpeed = 0f;
        private float baseHeight;
         */

        /// <summary>
        /// The pitch constraint
        /// </summary>
        private float
            maxPitch = 80f / 180f * (float)Math.PI,
            currentPitch = 0;

        private Player player;

        public FPCamera(Game game, Player player)
            : base(game, player.kinematic.position, player.kinematic.position + new Vector3(1f, 0f, 0f), Vector3.Up)
        {
            this.player = player;
        }

        public override void Update(GameTime gameTime)
        {
            this.kinematic = player.kinematic;
            this.steering = player.steering;

            // Do an FPS-style pitch and yaw computation
            fpsPitchYaw();

            // Re-build the view matrix
            createLookAt();

            // Reset mouse position to mid-window so as to obtain differential coordinates on next update
            Mouse.SetPosition(halfWindow.Width, halfWindow.Height);

            // Call parent update routine
            base.Update(gameTime);
        }

        protected void fpsPitchYaw()
        {
            // Obtain mouse coordinates
            MouseState mouse = Mouse.GetState();

            // Construct yaw rotation matrix by rotating about the positive Y (up) vector
            Matrix rot_yaw =
                Matrix.CreateFromAxisAngle(up, -(float)(mouse.X - halfWindow.Width) / (float)window.Width);

            // Multiply normalized direction vector by yaw rotation matrix
            direction = Vector3.Normalize(Vector3.Transform(direction, rot_yaw));

            // Limit pitch rotation
            float pitchAngle = (float)(mouse.Y - halfWindow.Height) / (float)window.Height;
            if (Math.Abs(currentPitch + pitchAngle) < maxPitch)
            {
                // Bidirectional vector is orthogonal to the direction and up vectors
                Vector3 biDirection = Vector3.Normalize(Vector3.Cross(up, direction));

                Matrix rot_pitch =
                    Matrix.CreateFromAxisAngle(biDirection, pitchAngle);
                direction = Vector3.Normalize(Vector3.Transform(direction, rot_pitch));
                currentPitch += pitchAngle;
            }
        }

        /*
        private float get_jump_y_v2(GameTime gameTime)
        {
            KeyboardState keyboard = .GetState();

            float timeDelta = gameTime.ElapsedGameTime.Seconds;
            float result = 0f;

            if (keyboard.IsKeyDown(Keys.Space) && (position.Y == baseHeight))
                jumpSpeed = 20;

            if (position.Y > baseHeight)
                jumpSpeed--;

            if ((position + Vector3.Up * jumpSpeed * timeDelta).Y < baseHeight)
            {
                jumpSpeed = 0;
                result = baseHeight;
            }

            result = position.Y + jumpSpeed * timeDelta;

            return result;
        }
         */
    }

    /// <summary>
    /// A Sim-style camera.
    /// </summary>
    public class TopdownCamera : Camera
    {
        // The speed at which to zoom the camera.
        public float zoomSpeed = 40f;

        // The speed at which to rotate the camera.
        public float rotSpeed = 0.05f;

        // The scroll value as of the last update.
        int prevScroll;

        public TopdownCamera(Game game, Vector3 eye, Vector3 target, Vector3 up)
            : base(game, eye, target, up)
        {
            prevScroll = Mouse.GetState().ScrollWheelValue;
        }

        public TopdownCamera(Game game, CameraTuple tuple)
            : base(game, tuple)
        {
            prevScroll = Mouse.GetState().ScrollWheelValue;
        }

        public override void Update(GameTime gameTime)
        {
            steering.linear = Vector3.Zero;

            rotate();

            scrollZoom();

            transform();

            createLookAt();

            base.Update(gameTime);
        }

        /// <summary>
        /// Rotate the camera using the middle mouse button
        /// </summary>
        protected void rotate()
        {
            MouseState mouse = Mouse.GetState();

            // Do not rotate when mouse is not pressed
            if (mouse.MiddleButton == ButtonState.Released)
                return;

            // Rotation angle
            float theta = (float)(mouse.X - halfWindow.Width) / (float)window.Width * MathHelper.Pi * rotSpeed;

            // Create a rotation matrix
            Matrix rotation = Matrix.CreateRotationY(-theta);

            // Rotate the direction vector
            direction = Vector3.Transform(direction, rotation);
        }

        /// <summary>
        /// Zoom the camera using the scrollwheel
        /// </summary>
        protected void scrollZoom()
        {
            MouseState mouse = Mouse.GetState();

            int scrollDelta = mouse.ScrollWheelValue - prevScroll;

            steering.linear += direction * scrollDelta * zoomSpeed;

            prevScroll = mouse.ScrollWheelValue;
        }

        /// <summary>
        /// Perform keyboard-input translation *after* having rotated the view
        /// </summary>
        protected void transform()
        {
            // Keyboard key states
            KeyboardState ks = Keyboard.GetState();

            // Normalized Y-ignorant direction vector
            Vector3 flatDirection = Vector3.Normalize(new Vector3(
                direction.X,
                0,
                direction.Z));

            // Raise the direction to the max speed
            flatDirection *= maxSpeed;

            // Shift multiplier
            if (ks.IsKeyDown(Keys.LeftShift))
                flatDirection *= shiftMultiplier;

            // Orthonormal direction vector for strafe movement
            Vector3 orthoDirection = Vector3.Cross(up, flatDirection);

            if (ks.IsKeyDown(Keys.W))
                steering.linear += flatDirection;
            if (ks.IsKeyDown(Keys.S))
                steering.linear -= flatDirection;
            if (ks.IsKeyDown(Keys.A))
                steering.linear += orthoDirection;
            if (ks.IsKeyDown(Keys.D))
                steering.linear -= orthoDirection;
        }
    }

    /// <summary>
    /// Similar to an FPCamera, without a Y-constraint or 'jump' operation.
    /// </summary>
    public class FlyingCamera : Camera
    {
        /// <summary>
        /// The pitch constraint
        /// </summary>
        private float
            maxPitch = 150f / 180f * (float)Math.PI,
            currentPitch = 0;

        public FlyingCamera(Game game, Vector3 eye, Vector3 target, Vector3 up)
            : base(game, eye, target, up)
        {

        }

        public FlyingCamera(Game game, CameraTuple tuple)
            : base(game, tuple)
        {

        }

        public override void Update(GameTime gameTime)
        {
            steering.linear = Vector3.Zero;

            // Obtain mouse coordinates
            MouseState mouse = Mouse.GetState();

            // Construct yaw rotation matrix by rotating about the positive Y (up) vector
            Matrix rot_yaw =
                Matrix.CreateFromAxisAngle(up, -(float)(mouse.X - halfWindow.Width) / (float)window.Width);

            // Multiply normalized direction vector by yaw rotation matrix
            direction = Vector3.Normalize(Vector3.Transform(direction, rot_yaw));

            // Limit pitch rotation
            float pitchAngle = (float)(mouse.Y - halfWindow.Height) / (float)window.Height;
            if (Math.Abs(currentPitch + pitchAngle) < maxPitch)
            {
                // Bidirectional vector is orthogonal to the direction and up vectors
                Vector3 biDirection = Vector3.Normalize(Vector3.Cross(up, direction));

                Matrix rot_pitch =
                    Matrix.CreateFromAxisAngle(biDirection, pitchAngle);
                direction = Vector3.Normalize(Vector3.Transform(direction, rot_pitch));
                currentPitch += pitchAngle;
            }

            // Transform camera through new direction
            transform();

            // Re-build the view matrix
            createLookAt();

            // Reset mouse position to mid-window so as to obtain differential coordinates on next update
            Mouse.SetPosition(halfWindow.Width, halfWindow.Height);

            // Call parent update routine
            base.Update(gameTime);
        }

        /// <summary>
        /// Perform keyboard-input translation *after* having rotated the view
        /// </summary>
        private void transform()
        {
            // Keyboard key states
            KeyboardState ks = Keyboard.GetState();

            // Normalized Y direction vector
            Vector3 nDirection = Vector3.Normalize(direction) * maxSpeed;

            // Orthonormal direction vector for strafe movement
            Vector3 orthoDirection = Vector3.Normalize(Vector3.Cross(up, nDirection)) * maxSpeed;

            // Shift multiplier
            if (ks.IsKeyDown(Keys.LeftShift))
            {
                nDirection *= shiftMultiplier;
                orthoDirection *= shiftMultiplier;
            }

            if (ks.IsKeyDown(Keys.W))
                steering.linear += nDirection;
            if (ks.IsKeyDown(Keys.S))
                steering.linear -= nDirection;
            if (ks.IsKeyDown(Keys.A))
                steering.linear += orthoDirection;
            if (ks.IsKeyDown(Keys.D))
                steering.linear -= orthoDirection;
        }
    }
}
