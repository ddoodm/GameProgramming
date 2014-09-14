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
            PlanePrimitive floor = new PlanePrimitive(game, 1000f, Vector3.Up);
            floor.texture = game.Content.Load<Texture2D>("Textures\\concreteNew");
            floor.textureTiling = new Vector2(10, 10);
            floor.specularColour = new Vector3(0.5f);
            entityManager.add(new PlaneEntity(game, floor, Vector3.Zero, 0));

            // Create a skybox
            staticManager.add(new Skybox(game, game.Content.Load<Model>("Models\\DSkyboxMesh")));
            
            // Add the "Fire Here" sign
            Vector3 shootHerePosition = new Vector3(0,0,-600f);
            staticManager.add(new StaticModel(game, game.Content.Load<Model>("Models\\ShootHere"), Matrix.CreateTranslation(shootHerePosition)));

            // Create a "Tower Manager" which allows for the placement of towers in the area
            towerManager = new TowerManager(game, Matrix.CreateTranslation(new Vector3(0,10f,0)), staticManager);

            // Add a tank that pursues the player
            Tank pursueTank = new Tank(game, new Vector3(-200f,0,200f), this);
            NPC pursueNPC = new NPC(game, pursueTank);
            pursueTank.turretTarget = player.kinematic;
            pursueNPC.addPriority(NPCState.PURSUE);
            pursueNPC.target = player.kinematic;
            ((Pursue)pursueNPC.steering).targetRadius = 80f;
            ((Pursue)pursueNPC.steering).slowRadius = 250f;
            pursueNPC.steering.maxSpeed = 65f;
            npcManager.add(pursueNPC);

            // Add a tank that arrives at the Pursue Tank
            Tank arriveTank = new Tank(game, new Vector3(-200f, 0, 250f), this);
            NPC arriveNPC = new NPC(game, arriveTank);
            arriveTank.turretTarget = player.kinematic;
            arriveNPC.addPriority(NPCState.ARRIVE);
            arriveNPC.target = pursueNPC.kinematic;
            ((Arrive)arriveNPC.steering).targetRadius = 75f;
            arriveNPC.steering.maxSpeed = 45f;
            npcManager.add(arriveNPC);
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
