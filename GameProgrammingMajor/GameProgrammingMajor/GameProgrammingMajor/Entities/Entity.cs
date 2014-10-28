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
    /// A generic Entity that has a position, and virtual methods.
    /// </summary>
    public abstract class Entity
    {
        protected Game game;            // Handle to the main game object
        public Matrix world;            // Entity's world transformation matrix
        public Kinematic kinematic;     // Stores information about position and velocities
        public NPC npc = null;          // Handle to this entity's NPC controller. Can be null.

        public Entity(Game game)
        {
            this.game = game;
            world = Matrix.Identity;

            kinematic = new Kinematic();
        }

        public Entity(Game game, Vector3 position, float rotation)
            : this(game)
        {
            kinematic.position = position;
            kinematic.orientation = rotation;
        }

        public Entity(Game game, Matrix world)
            : this(game)
        {
            this.world = world;
        }

        public virtual bool collidesWith(StaticModel model) { return false; }
        public virtual bool collidesWith(BoundingSphere boundingSphere) { return false; }
        public virtual bool collidesWith(BoundingBox boundingBox) { return false; }

        public virtual void load(ContentManager content) { }

        public virtual void update(UpdateParams updateParams) { }

        public virtual void draw(DrawParams drawParams) { }
    }
}
