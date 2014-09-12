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
    /// Parameters to pass to an Entity when updating.
    /// </summary>
    public struct UpdateParams
    {
        public GameTime gameTime;
        public Camera camera;
        public KeyboardState keyboardState;
        public MouseState mouseState;
        public Player player;
        public World world;
        public Random random;
    }

    /// <summary>
    /// Parameters to pass to an Entity when drawing.
    /// </summary>
    public struct DrawParams
    {
        public GameTime gameTime;
        public Camera camera;
        public KeyboardState keyboardState;
        public MouseState mouseState;
    }

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

        public virtual void load(ContentManager content) { }

        public virtual void update(UpdateParams updateParams) { }

        public virtual void draw(DrawParams drawParams) { }
    }
}
