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
        public NPCManager npcManager;
        public TowerManager towerManager;
        public Quadtree tankTree;

        public Player player;

        public World(Game game)
        {
            staticManager = new StaticModelManager();
            entityManager = new EntityManager<Entity>(game);
            npcManager = new NPCManager(game);
        }

        public void hardcodedWorldPopulation(Game game)
        {
            // Add a test floor primitive to the Entity manager
            PlanePrimitive floor = new PlanePrimitive(game, 1000f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\concreteNew");
            floor.textureTiling = new Vector2(10, 10);
            floor.specularColour = new Vector3(0.5f);
            entityManager.add(new PlaneEntity(game, floor, Vector3.Zero, 0));

            // Create a terrain
            Terrain terrain = new Terrain(game, game.Content.Load<Texture2D>("Textures\\terrain00"), new Vector3(20f, 0.25f, 20f));
            entityManager.add(terrain);

            // Create a skybox
            staticManager.add(new Skybox(game, game.Content.Load<Model>("Models\\DSkyboxMesh")));

            // Create the quadtree structure
            Vector3 halfTreeSize = new Vector3(500, 100, 500);
            tankTree = new Quadtree(new BoundingBox(-halfTreeSize, halfTreeSize), game.GraphicsDevice);

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.CreateTranslation(new Vector3(0,10f,0)), terrain, tankTree);
        }

        public void load(ContentManager content)
        {
            // Load path finding demo content 
            towerManager.load(content);
        }

        public void update(UpdateParams updateParams)
        {
            staticManager.update(updateParams);
            entityManager.update(updateParams);
            towerManager.update(updateParams);
            npcManager.update(updateParams);
            tankTree.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            staticManager.draw(drawParams);
            entityManager.draw(drawParams);
            towerManager.draw(drawParams);
            npcManager.draw(drawParams);
            tankTree.draw(drawParams);
        }
    }
}
