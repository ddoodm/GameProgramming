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
        public TowerManager towerManager;

        public World(Game game)
        {
            staticManager = new StaticModelManager();
            entityManager = new EntityManager();

            // Add a test floor primitive to the Entity manager
            PlanePrimitive floor = new PlanePrimitive(game, 400f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\Grass0139_33_S");
            floor.textureTiling = new Vector2(4, 4);
            entityManager.add(
                new PrimitiveEntity<VertexPositionNormalTexture>(game, floor));

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.Identity);
        }

        public void update(UpdateParams updateParams)
        {
            entityManager.update(updateParams);
            staticManager.update(updateParams);
            towerManager.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            entityManager.draw(drawParams);
            staticManager.draw(drawParams);
            towerManager.draw(drawParams);
        }
    }
}
