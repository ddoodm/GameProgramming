using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _11688025_lab10
{
    /// <summary>
    /// The World is responsible for storing and managing
    /// world geometry. 
    /// </summary>
    public class World
    {
        public Player player;

        private PlaneEntity plane;
        private TowerManager towerManager;
        private Terrain terrain;

        public World(Game game)
        {
            hardcodedWorldPopulation(game);
        }

        public void hardcodedWorldPopulation(Game game)
        {
            // The height-map terrain
            terrain = new Terrain(game, game.Content.Load<Texture2D>("heightMap"), new Vector3(20f, 0.25f, 20f));

            // The A* pathfinding demo area
            towerManager = new TowerManager(game, Matrix.Identity, terrain);
            towerManager.setPathFinderStartNode(towerManager.idOf(Vector3.Zero));

            // A large ground plane
            PlanePrimitive planePrim = new PlanePrimitive(game, 500f, Vector3.Up);
            planePrim.texture = game.Content.Load<Texture2D>("Grass0139_33_S");
            planePrim.textureTiling = new Vector2(6);
            planePrim.specularColour = Vector3.Zero;
            plane = new PlaneEntity(game, planePrim, Vector3.Zero, 0);
        }

        public void load(ContentManager content)
        {
            towerManager.load(content);
        }

        public void update(UpdateParams updateParams)
        {
            // Set player on ground
            player.setFootY(towerManager.terrain.getYAt(player.getFootCoords()));

            towerManager.update(updateParams);
            terrain.update(updateParams);
            plane.update(updateParams);
        }

        public void draw(DrawParams drawParams)
        {
            plane.draw(drawParams);
            terrain.draw(drawParams);
            towerManager.draw(drawParams);
        }
    }
}
