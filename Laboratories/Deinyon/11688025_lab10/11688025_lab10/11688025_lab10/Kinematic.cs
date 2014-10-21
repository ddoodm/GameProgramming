using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _11688025_lab10
{
    public class Kinematic
    {
        public Vector3 position         = Vector3.Zero;
        public float orientation        = 0;
        public Vector3 velocity         = Vector3.Zero;
        public float rotation           = 0;

        public Kinematic() { }

        public Kinematic(Kinematic rhs)
        {
            this.position = rhs.position;
            this.orientation = rhs.orientation;
            this.velocity = rhs.velocity;
            this.rotation = rhs.rotation;
        }

        public Kinematic(Vector3 position)
        {
            this.position = position;
        }

        /// <summary>
        /// Implementation of the Newton-Euler-1 Integration Update algorithm.
        /// The algorithm effectively integrates the velocity over time
        /// to determine the position.
        /// </summary>
        /// <param name="steering">The steering structure of the character</param>
        /// <param name="timeDelta">The time difference since the last frame</param>
        public void update(Steering steering, float timeDelta)
        {
            // Update position and orientation
            position += velocity * timeDelta;
            orientation += rotation * timeDelta;

            // Update velocity and rotation
            velocity += steering.linear * timeDelta;
            rotation += steering.angular * timeDelta;
        }

        public void updateBasicFacing()
        {
            // Rotate only of the character is moving
            if (velocity.Length() > 0.5f)
                orientation = (float)Math.Atan2(velocity.X, velocity.Z);
        }
    }
}
