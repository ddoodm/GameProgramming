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
        //public TankWave tankWave;

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
            PlanePrimitive floor = new PlanePrimitive(game, 500f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\concreteNew");
            floor.textureTiling = new Vector2(5, 5);
            floor.specularColour = Vector3.Zero;
            entityManager.add(new PlaneEntity(game, floor, Vector3.Zero, 0));

            // Create a skybox
            staticManager.add(new Skybox(game, game.Content.Load<Model>("Models\\DSkyboxMesh")));

            // Add the "Fire Here" sign
            Vector3 shootHerePosition = new Vector3(0,0,-300f);
            staticManager.add(new StaticModel(game, game.Content.Load<Model>("Models\\ShootHere"), Matrix.CreateTranslation(shootHerePosition)));

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.CreateTranslation(new Vector3(150f,10f,150f)), staticManager);

            // Add a tank that pursues the player
            Tank pursueTank = new Tank(game);
            NPC pursueNPC = new NPC(game, pursueTank);
            pursueNPC.setState(NPCState.PURSUE);
            pursueNPC.target = player.kinematic;
            pursueNPC.steering.maxSpeed = 65f;
            npcManager.add(pursueNPC);

            // Add a tank that arrives at the Pursue Tank
            Tank arriveTank = new Tank(game);
            NPC arriveNPC = new NPC(game, arriveTank);
            arriveNPC.setState(NPCState.ARRIVE);
            arriveNPC.target = pursueNPC.kinematic;
            arriveNPC.steering.maxSpeed = 25f;
            npcManager.add(arriveNPC);
        }

        public void update(UpdateParams updateParams)
        {
            staticManager.update(updateParams);
            entityManager.update(updateParams);
            towerManager.update(updateParams);
            npcManager.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            staticManager.draw(drawParams);
            entityManager.draw(drawParams);
            towerManager.draw(drawParams);
            npcManager.draw(drawParams);
        }
    }
}
