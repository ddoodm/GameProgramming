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

            // Add a test floor primitive to the Entity manager
            PlanePrimitive floor = new PlanePrimitive(game, 200f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\Grass0139_33_S");
            floor.tiling = new Vector2(2, 2);
            entityManager.add(
                new PrimitiveEntity<VertexPositionNormalTexture>(game, floor));
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
