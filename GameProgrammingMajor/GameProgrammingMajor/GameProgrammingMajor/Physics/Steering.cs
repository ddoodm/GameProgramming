﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Steering
    {
        public Vector3 linear;
        public float angular;

        public float maxAcceleration = 600f, maxSpeed = 300f;

        /// <summary>
        /// Used to draw the "predicted target" waypoint dummy model.
        /// </summary>
        public Vector3 predictedTarget { get; protected set; }

        /// <summary>
        /// Nullify this steering object
        /// </summary>
        public void zero()
        {
            linear = Vector3.Zero;
            angular = 0;
        }

        public virtual void update(Kinematic character, Kinematic target) { }
    }

    /// <summary>
    /// A seek AI gradually approaches the target over time.
    /// </summary>
    public class Seek : Steering
    {
        public float targetRadius = 5f;

        public override void update(Kinematic character, Kinematic target)
        {
            // Find direction to target from position
            Vector3 direction = target.position - character.position;

            // Distance to target is direction's magnitude
            float distance = direction.Length();

            // If the target radius has been reached, there is no need to update
            if (distance < targetRadius)
            {
                linear = Vector3.Zero;
                character.velocity = Vector3.Zero;
                return;
            }

            // In seek mode, the target speed is always max speed
            float targetSpeed = maxSpeed;

            // The desired velocity is the direction raised to the desired speed
            Vector3 targetVelocity = Vector3.Normalize(direction) * targetSpeed;

            // The steering force is the vector between the current velocity
            // and the desired velocity, and as such, the steering force
            // attempts to pull the character towards the target. 
            Vector3 steeringForce = targetVelocity - character.velocity;

            // Clip if acceleration is too great so that animation is distributed over time
            if (steeringForce.Length() > maxAcceleration)
                steeringForce = Vector3.Normalize(steeringForce) * maxAcceleration;

            // The linear steering force is the newly computed steering force.
            this.linear = steeringForce;

            // Compute and store the orientation
            this.angular = 0;

            // Provide the target "prediction" for debugging, which, in this case, is just the target.
            this.predictedTarget = target.position;
        }
    }

    /// <summary>
    /// An Approach steering AI will seek toward the target
    /// until it reaches the slowing radius, at which point,
    /// the character will decelerate.
    /// </summary>
    public class Arrive : Seek
    {
        public float slowRadius = 120f, timeToTarget = 0.1f;

        public override void update(Kinematic character, Kinematic target)
        {
            // Find direction to target from position
            Vector3 direction = target.position - character.position;

            // Distance to target is direction's magnitude
            float distance = direction.Length();

            // If the target radius has been reached, there is no need to update
            if (distance < targetRadius)
            {
                linear = Vector3.Zero;
                character.velocity = Vector3.Zero;
                return;
            }

            // If the object is outside the slowing radius, use Seek
            if (distance > slowRadius)
            {
                base.update(character, target);
                return;
            }

            // We are inside the "slowing radius", so reduce speed
            float targetSpeed = maxSpeed * distance / slowRadius;

            // The desired velocity is the direction raised to the desired speed
            Vector3 targetVelocity = Vector3.Normalize(direction) * targetSpeed;

            // Through acceleration, the object's velocity approaches the target velocity
            Vector3 steeringForce = (targetVelocity - character.velocity) / timeToTarget;

            // Clip if acceleration is too great
            if (steeringForce.Length() > maxAcceleration)
                steeringForce = Vector3.Normalize(steeringForce) * maxAcceleration;

            // Store force
            this.linear = steeringForce;

            // Compute and store the orientation
            this.angular = 0f;

            // Provide the target "prediction" for debugging, which, in this case, is just the target.
            this.predictedTarget = target.position;
        }
    }

    public class Pursue : Arrive
    {
        public float predictionLimit = 2f;

        public override void update(Kinematic character, Kinematic evader)
        {
            // Distance of evader from the character
            float distance = (evader.position - character.position).Length();

            // Used as timing coefficient in target position computation
            float timeToReachTarget;
            float speed = character.velocity.Length();
            if (speed <= predictionLimit)
                timeToReachTarget = predictionLimit;
            else
                timeToReachTarget = distance / speed;

            // Predicted target is the evader's position + their velocity
            Vector3 target = evader.position + evader.velocity * timeToReachTarget;

            // Arrive on newly predicted target
            Kinematic kTarget = new Kinematic(evader);
            kTarget.position = target;
            base.update(character, kTarget);
        }
    }
}