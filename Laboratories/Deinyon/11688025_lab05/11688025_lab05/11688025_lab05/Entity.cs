using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _11688025_lab05
{
    public abstract class Entity
    {
        protected Game game;
        public Matrix world;
        public Vector3 position { get; protected set; }
        public float rotation { get; protected set; }

        public AxisHelper axis { get; protected set; }

        public Entity(Game game)
        {
            this.game = game;
            world = Matrix.Identity;

            axis = new AxisHelper(game, 32f);
        }

        public Entity(Game game, Vector3 position, float rotation)
            : this(game)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public Entity(Game game, Matrix world)
            : this(game)
        {
            this.world = world;
        }

        public virtual void update(GameTime gameTime)
        {
            axis.update(gameTime, world);
        }
    }
}
