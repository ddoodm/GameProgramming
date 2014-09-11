using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The World is responsible for storing and managing
    /// world geometry. 
    /// </summary>
    public class World
    {
        public StaticModelManager staticManager;
        public EntityManager<Entity> entityManager;
        public TowerManager towerManager;
        public TankWave tankWave;

        public World(Game game)
        {
            staticManager = new StaticModelManager();
            entityManager = new EntityManager<Entity>(game);

            hardcodedWorldPopulation(game);
        }

        private void hardcodedWorldPopulation(Game game)
        {
            // Add a test floor primitive to the Entity manager
            PlanePrimitive floor = new PlanePrimitive(game, 500f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\Grass0139_33_S");
            floor.textureTiling = new Vector2(5, 5);
            floor.specularColour = Vector3.Zero;
            entityManager.add(
                new PrimitiveEntity<VertexPositionNormalTexture>(game, floor));

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.Identity);

            // Initialize an entity wave for entities of type 'Tank'
            Kinematic target = new Kinematic(new Vector3(300f, 0, 300f));
            tankWave = new TankWave(game, new Vector3[1]{towerManager.coordinatesOf(new iVec2(1,1))}, target);
        }

        public void update(UpdateParams updateParams)
        {
            entityManager.update(updateParams);
            staticManager.update(updateParams);
            towerManager.update(updateParams);
            tankWave.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            entityManager.draw(drawParams);
            staticManager.draw(drawParams);
            towerManager.draw(drawParams);
            tankWave.draw(drawParams);
        }
    }
}
