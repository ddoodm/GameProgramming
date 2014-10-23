using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public class EntityManager<EntityType> where EntityType : Entity
    {
        private Game game;

        public List<EntityType> entities;

        public EntityManager(Game game)
        {
            this.game = game;
            entities = new List<EntityType>();
        }

        /// <summary>
        /// Add an entity to the Entity Manager
        /// </summary>
        /// <param name="entity">The entity to add to the manager.</param>
        /// <returns>A handle to the entity that was added to the manager.</returns>
        public Entity add(EntityType entity)
        {
            entities.Add(entity);
            entity.load(game.Content);
            return entity;
        }

        public virtual void update(UpdateParams updateParams)
        {
            foreach (Entity entity in entities)
                entity.update(updateParams);
        }

        public virtual void draw(DrawParams drawParams)
        {
            foreach (Entity entity in entities)
                entity.draw(drawParams);
        }
    }
}
