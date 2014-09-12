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
        //public TankWave tankWave;

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
            entityManager.add(new PlaneEntity(game, floor, Vector3.Zero, 0));

            // Create a skybox
            staticManager.add(new Skybox(game, game.Content.Load<Model>("Models\\DSkyboxMesh")));

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.CreateTranslation(new Vector3(150f,0,150f)));
        }

        public void update(UpdateParams updateParams)
        {
            staticManager.update(updateParams);
            entityManager.update(updateParams);
            towerManager.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            staticManager.draw(drawParams);
            entityManager.draw(drawParams);
            towerManager.draw(drawParams);
        }
    }
}
