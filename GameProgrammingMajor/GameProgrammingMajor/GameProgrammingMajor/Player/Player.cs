using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The structure that defines a player of the game. 
    /// </summary>
    public class Player
    {
        public int score = 0;

        /// <summary>
        /// The camera that is linked with the player
        /// </summary>
        private Camera camera;

        public Kinematic kinematic;
        public Steering steering;

        private ProjectileManager projectileMan;

        /// <summary>
        /// Create a player that is linked to a camera
        /// </summary>
        /// <param name="game">The main game object</param>
        /// <param name="camera">The camera to link with the player</param>
        public Player(Game game, Camera camera, World world)
        {
            this.camera = camera;
            this.kinematic = camera.kinematic;
            this.steering = camera.steering;

            projectileMan = new ProjectileManager(game, world);
        }

        public void loadContent(ContentManager content)
        {
            projectileMan.loadContent(content);
        }

        public void update(UpdateParams updateParams)
        {
            // Re-sync with the camera's data
            kinematic = camera.kinematic;
            steering = camera.steering;

            // Fire a projectile if the mouse button is down
            projectileMan.update(updateParams);
            if (updateParams.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                projectileMan.shoot(camera);
        }

        public void draw(DrawParams drawParams)
        {
            projectileMan.draw(drawParams);
        }
    }
}
