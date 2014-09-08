using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace _11688025_lab06
{
    public abstract class Entity
    {
        protected Game game;
        public Matrix world;
        public Kinematic kinematic;
        public NPC npc;

        //public AxisHelper axis { get; protected set; }

        public Entity(Game game)
        {
            this.game = game;
            world = Matrix.Identity;

            kinematic = new Kinematic();

            //axis = new AxisHelper(game, 32f);
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

        public virtual void load(ContentManager content) { }

        public virtual void update(GameTime gameTime) { }

        public virtual void draw(Camera camera) { }
    }
}
