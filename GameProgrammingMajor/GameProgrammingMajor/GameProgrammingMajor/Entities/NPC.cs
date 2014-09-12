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
        EVADE
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

        public Kinematic kinematic
        {
            get { return entity.kinematic; }
        }

        public void setState(NPCState state)
        {
            this.state = state;

            switch (state)
            { 
                case NPCState.SEEK:     steering = new Seek();    break;
                case NPCState.ARRIVE:   steering = new Arrive();  break;
                case NPCState.PURSUE:   steering = new Pursue();  break;
            }
        }

        public NPC(Game game, Entity entity)
        {
            this.entity = entity;

            // Provide the entity with this NPC object
            entity.npc = this;

            // Default steering algorithm
            steering = new Arrive();
            state = NPCState.ARRIVE;

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

            // Update steering force
            steering.update(entity.kinematic, target);

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
            if(DEBUG) waypointModel.draw(drawParams);
        }
    }
}
