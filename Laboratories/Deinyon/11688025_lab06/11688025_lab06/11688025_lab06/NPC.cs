using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
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

            waypointModel = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));
        }

        public void load(ContentManager content)
        {
            entity.load(content);
        }

        public void update(GameTime gameTime)
        {
            // Obtain time difference
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update steering force
            steering.update(entity.kinematic, target);

            // Process velocity and orientation into position and rotation
            entity.kinematic.update(steering, timeDelta);
            entity.kinematic.updateBasicFacing();

            entity.update(gameTime);

            // Update Waypoint dummy model
            waypointModel.update(gameTime);
            waypointModel.world = Matrix.CreateScale(4f) * Matrix.CreateTranslation(steering.predictedTarget);
        }

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

        public void draw(Camera camera)
        {
            entity.draw(camera);

            // Draw target waypoint
            waypointModel.draw(camera);
        }
    }
}
