using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameProgrammingMajor
{
    public class EntityManager
    {
        public List<Entity> entities;

        public EntityManager()
        {
            entities = new List<Entity>();
        }

        /// <summary>
        /// Add an entity to the Entity Manager
        /// </summary>
        /// <param name="entity">The entity to add to the manager.</param>
        /// <returns>A handle to the entity that was added to the manager.</returns>
        public Entity add(Entity entity)
        {
            entities.Add(entity);
            return entity;
        }

        public void update(UpdateParams updateParams)
        {
            foreach (Entity entity in entities)
                entity.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            foreach (Entity entity in entities)
                entity.draw(drawParams);
        }
    }
}
