using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The World is responsible for storing and managing
    /// world geometry. 
    /// </summary>
    public class World
    {
        public StaticModelManager staticManager;
        public EntityManager entityManager;

        public World(Game game)
        {
            staticManager = new StaticModelManager();
            entityManager = new EntityManager();

            // Add a few test entities
            entityManager.add(
                new PrimitiveEntity<VertexPositionNormalTexture>(
                   game, new PlanePrimitive(game, 200f, Vector3.Up)));
        }

        public void update(EntityUpdateParams updateParams)
        {
            entityManager.update(updateParams);
            staticManager.update(updateParams);
        }

        public void draw(EntityDrawParams drawParams)
        {
            entityManager.draw(drawParams);
            staticManager.draw(drawParams);
        }
    }
}
