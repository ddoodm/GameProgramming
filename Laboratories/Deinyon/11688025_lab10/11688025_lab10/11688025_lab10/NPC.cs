using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab10
{
    /// <summary>
    /// List of NPC AI states.
    /// </summary>
    public enum NPCState
    {
        IDLE,
        SEEK,
        ARRIVE,
        PURSUE,
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

        public float lookAheadDistance = 150f;

        public Kinematic kinematic
        {
            get { return entity.kinematic; }
        }

        public void addPriority(NPCState state)
        {
            priorities.Push(state);
            setState(state);
        }

        private void setState(NPCState state)
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

        public bool inTargetRadius()
        {
            if (target == null)
                return false;

            try
            {
                return ((Seek)steering).inTargetRadius(entity.kinematic, target);
            } catch (Exception e)
            {
                throw new Exception("NPC::inTargetRadius() - Steering must be at least a 'Seek'");
            }
        }

        private void updateSteeringState(UpdateParams updateParams)
        {

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
