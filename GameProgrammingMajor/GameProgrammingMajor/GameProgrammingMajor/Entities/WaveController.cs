using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Sinusoid
    {
        public float
            amplitude = 1f,     // The peak-to-peak amplitude of the wave
            theta = 0f,         // The current angle
            phase = 0f,         // The sine function offset
            yOffset = 0f;       // The Y-coordinate offset of the function

        public float sin(float theta)
        {
            this.theta = theta;
            return yOffset + amplitude * (float)Math.Sin(theta + phase);
        }

        public float cos(float theta)
        {
            this.theta = theta;
            return yOffset + amplitude * (float)Math.Cos(theta + phase);
        }
    }

    /// <summary>
    /// Controls enemy spawning waves.
    /// </summary>
    public class TankWave
    {
        private Game game;

        /// <summary>
        /// The list of Tanks to update and draw
        /// </summary>
        public List<NPC> tankNpcs;

        /// <summary>
        /// The maximum number of entities that can spawn from this controller
        /// </summary>
        public int maxTanks = 5;

        /// <summary>
        /// The entity spawning locations
        /// </summary>
        public Vector3[] spawnOrigins;

        /// <summary>
        /// The target for the entities to chase
        /// </summary>
        public Kinematic target;

        /// <summary>
        /// The sinusoid that describes the "wave"
        /// </summary>
        public Sinusoid wave;

        /// <summary>
        /// A debugging model that illustrates the origin
        /// </summary>
        private StaticModel[] originSpheres;

        public TankWave(Game game, Vector3[] spawnOrigins, Kinematic target)
        {
            this.game = game;
            this.spawnOrigins = spawnOrigins;
            this.target = target;

            tankNpcs = new List<NPC>();

            initOriginSpheres(game);
        }

        public void initOriginSpheres(Game game)
        {
            originSpheres = new StaticModel[spawnOrigins.Length];

            int i=0;
            foreach(Vector3 origin in spawnOrigins)
            {
                originSpheres[i] = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"),
                    Matrix.CreateScale(5f) * Matrix.CreateTranslation(origin));

                i++;
            }
        }

        public void update(UpdateParams updateParams)
        {
            foreach(StaticModel sphere in originSpheres)
                sphere.update(updateParams);

            addEntities(updateParams);

            foreach (NPC tank in tankNpcs)
                tank.update(updateParams);
        }

        private void addEntities(UpdateParams updateParams)
        {
            // Do not exceed limit
            if (tankNpcs.Count >= maxTanks)
                return;

            // Spawn at a random origin
            Random rand = new Random();
            int randIndex = rand.Next(0, spawnOrigins.Length);

            // Create the NPC
            Tank newTank = new Tank(game, spawnOrigins[randIndex]);

            // Create an NPC for the tank
            NPC tankNpc = new NPC(game, newTank);

            if (tankNpcs.Count == 0)
            {
                // Arrive at the target
                tankNpc.setState(NPCState.ARRIVE);
                tankNpc.target = target;

                // Set leader tank steering properties
                Arrive leaderSteering = (Arrive)tankNpc.steering;
                leaderSteering.maxSpeed = 50f;
            }
            else
            {
                // Pursue every other tank
                tankNpc.setState(NPCState.PURSUE);
                tankNpc.target = tankNpcs[tankNpcs.Count - 1].kinematic;

                // Set follower tank steering properties
                Arrive followerSteering = (Pursue)tankNpc.steering;
                followerSteering.maxSpeed = 40f;
                followerSteering.targetRadius = 80f;
                followerSteering.slowRadius = 200f;
            }

            // Add the entity to the EntityManager
            addNpc(tankNpc);
        }

        private void addNpc(NPC npc)
        {
            npc.load(game.Content);
            tankNpcs.Add(npc);
        }

        public void draw(DrawParams drawParams)
        {
            foreach (StaticModel sphere in originSpheres)
                sphere.draw(drawParams);

            foreach (NPC tank in tankNpcs)
                tank.draw(drawParams);
        }
    }
}
