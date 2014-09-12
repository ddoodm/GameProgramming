using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The structure that defines a player of the game. 
    /// </summary>
    public class Player
    {
        public int score = 0;

        public Kinematic kinematic;
        public Steering steering;

        /// <summary>
        /// Player's physical settings
        /// </summary>
        public float
            maxSpeed = 700f,
            shiftMultiplier = 2f,
            friction = 0.95f,
            height = 20f;

        /// <summary>
        /// Parameters to maintain the jump state
        /// </summary>
        private const float JUMP_SPEED = 0.08f;
        private Boolean jumping = false;
        private float jumpTheta = 0f;

        private ProjectileManager projectileMan;

        /// <summary>
        /// Create a player that is linked to a camera
        /// </summary>
        public Player(Game game, Matrix worldMatrix, World world)
        {
            this.kinematic = new Kinematic(worldMatrix.Translation);
            this.steering = new Steering();

            projectileMan = new ProjectileManager(game, world);
        }

        public void loadContent(ContentManager content)
        {
            projectileMan.loadContent(content);
        }

        public void update(UpdateParams updateParams)
        {
            transform(updateParams);

            // Fire a projectile if the mouse button is down
            projectileMan.update(updateParams);
            if (updateParams.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                projectileMan.shoot(updateParams.camera);
        }

        /// <summary>
        /// Transform the player with respect to the camera
        /// </summary>
        /// <param name="updateParams"></param>
        private void transform(UpdateParams updateParams)
        {
            KeyboardState ks = updateParams.keyboardState;

            // Normalized Y-ignorant direction vector
            Vector3 flatDirection = Vector3.Normalize(new Vector3(
                updateParams.camera.direction.X,
                0,
                updateParams.camera.direction.Z));

            // Raise the direction to the max speed
            flatDirection *= maxSpeed;

            // Shift multiplier
            if (ks.IsKeyDown(Keys.LeftShift))
                flatDirection *= shiftMultiplier;

            // Orthonormal direction vector for strafe movement
            Vector3 orthoDirection = Vector3.Cross(updateParams.camera.up, flatDirection);

            steering.linear = Vector3.Zero;

            if (ks.IsKeyDown(Keys.W))
                steering.linear += flatDirection;
            if (ks.IsKeyDown(Keys.S))
                steering.linear -= flatDirection;
            if (ks.IsKeyDown(Keys.A))
                steering.linear += orthoDirection;
            if (ks.IsKeyDown(Keys.D))
                steering.linear -= orthoDirection;

            // Do jump operation
            kinematic.position = new Vector3(
                kinematic.position.X,
                height + get_jump_y(ks),
                kinematic.position.Z);

            // Apply friction
            kinematic.velocity *= friction;
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
        protected virtual float get_jump_y(KeyboardState ks)
        {
            if (jumping)
            {
                if (jumpTheta >= Math.PI - JUMP_SPEED * Math.PI)
                    jumping = false;

                return 42f * (float)Math.Pow(Math.Sin(jumpTheta += JUMP_SPEED), 1.5);
            }
            else if (ks.IsKeyDown(Keys.Space))
            {
                jumping = true;
                jumpTheta = 0f;
            }

            return 0f;
        }

        public void draw(DrawParams drawParams)
        {
            projectileMan.draw(drawParams);
        }
    }
}
