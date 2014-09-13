using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// List of NPC AI states.
    /// </summary>
    public enum NPCState
    {
        IDLE,
        SEEK,
        FLEE,
        ARRIVE,
        ALIGN,
        MATCH,
        PURSUE,
        EVADE,
        AVOID
    }

    public class NPC 
    {
        public static bool DEBUG = true;

        /// <summary>
        /// The waypoint model is for debugging purposes.
        /// It is used to illustrate the NPC's steering target,
        /// or target prediction. It is only visible when
        /// NPC.DEBUG is true.
        /// </summary>
        private StaticModel waypointModel;

        public Entity entity { get; private set; }
        public Kinematic target { private get; set; }
        public Steering steering;
        public NPCState state { get; private set; }
        public Stack<NPCState> priorities { get; private set; }

        public float lookAheadDistance = 75f;

        public Kinematic kinematic
        {
            get { return entity.kinematic; }
        }

        public void setState(NPCState state)
        {
            if (this.state == state)
                return;

            this.state = state;

            // Backup old steering state
            Steering oldSteering = steering;

            switch (state)
            {
                case NPCState.SEEK:     steering = new Seek(oldSteering);      break;
                case NPCState.ARRIVE:   steering = new Arrive(oldSteering);    break;
                case NPCState.PURSUE:   steering = new Pursue(oldSteering);    break;
                case NPCState.AVOID:    steering = new Avoid(oldSteering);     break;
            }
        }

        public NPC(Game game, Entity entity)
        {
            this.entity = entity;

            // Provide the entity with this NPC object
            entity.npc = this;

            // Configure the priorities stack
            priorities = new Stack<NPCState>();

            // Default steering algorithm
            priorities.Push(NPCState.ARRIVE);

            if(DEBUG)
                waypointModel = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));
        }

        public void load(ContentManager content)
        {
            entity.load(content);
        }

        public void update(UpdateParams updateParams)
        {
            // Obtain time difference
            float timeDelta = (float)updateParams.gameTime.ElapsedGameTime.TotalSeconds;

            // Update steering AI state using a state machine
            updateSteeringState(updateParams);

            // Update steering force
            steering.update(updateParams, entity.kinematic, target);

            // Process velocity and orientation into position and rotation
            entity.kinematic.update(steering, timeDelta);
            entity.kinematic.updateBasicFacing();

            entity.update(updateParams);

            // Update Waypoint dummy model
            if (DEBUG)
            {
                waypointModel.update(updateParams);
                waypointModel.world = Matrix.CreateScale(4f) * Matrix.CreateTranslation(steering.predictedTarget);
            }
        }

        private void updateSteeringState(UpdateParams updateParams)
        {
            // The "ahead" vector is a 'feeler'
            Vector3 direction = kinematic.velocity == Vector3.Zero ? new Vector3(1f, 0f, 0f) : kinematic.velocity;
            Vector3 ahead = kinematic.position + Vector3.Normalize(direction) * lookAheadDistance;

            // Get the nearest collision sphere
            BoundingSphere? nearestCollision =
                updateParams.world.staticManager.findNearestCollisionSphere(
                    kinematic.position, ahead, lookAheadDistance);

            switch (state)
            {
                case NPCState.SEEK:
                case NPCState.ARRIVE:
                case NPCState.PURSUE:

                    // If there is a collision, we should attempt to avoid it
                    if (nearestCollision.HasValue)
                        priorities.Push(NPCState.AVOID);

                    break;

                case NPCState.AVOID:

                    // If there are no longer any plausible collisions, revert to the old AI
                    if (!nearestCollision.HasValue)
                        priorities.Pop();

                    break;
            }

            // Set the current state to the topmost priority
            setState(priorities.Peek());
        }

        /// <summary>
        /// A very hacky method for spherical collision detection on a tank.
        /// </summary>
        /// <param name="other">The tank with which we wish to avoid.</param>
        public void tankColUpdate(Tank other)
        {
            if (entity.GetType() != typeof(Tank))
                throw new Exception("tankColUpdate() called on a " + entity.GetType().ToString() + " NPC.");

            if (((Tank)entity).colliding(other))
            {
                // Stop on collision with another tank
                kinematic.velocity *= 0f;
                steering.linear *= 0f;
            }
        }

        public void draw(DrawParams drawParams)
        {
            entity.draw(drawParams);

            // Draw target waypoint
            if (DEBUG)
            {
                drawParams.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                waypointModel.draw(drawParams);
                drawParams.graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            }
        }
    }
}
