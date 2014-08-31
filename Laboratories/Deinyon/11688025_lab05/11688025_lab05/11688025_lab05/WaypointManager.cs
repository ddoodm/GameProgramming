using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _11688025_lab05
{
    public class WaypointManager
    {
        Game1 game;
        StaticModel dummy;

        public Vector3 position;
        public Vector3 livePosition;

        public WaypointManager(Game1 game)
        {
            this.game = game;
            this.position = Vector3.Zero;

            dummy = new StaticModel(game, game.Content.Load<Model>("Models\\DSphere"));
        }

        public void update(GameTime gameTime)
        {
            // Obtain mouse coordinates in Cartesian screen space as they
            // lie on the near and far planes
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector3 nearMouse = new Vector3( mouse, 0 );
            Vector3 farMouse = new Vector3( mouse, 1 );

            // Perform a reverse perspective projection on the mouse coordinates
            // and obtain a near-far vector delta
            Vector3 near = game.GraphicsDevice.Viewport.Unproject(
                nearMouse, game.camera.projection, game.camera.view, Matrix.Identity);
            Vector3 far = game.GraphicsDevice.Viewport.Unproject(
                farMouse, game.camera.projection, game.camera.view, Matrix.Identity);

            // Obtain a ray that projects from the near mouse to the far mouse
            Vector3 direction = Vector3.Normalize(far - near);
            Ray mouseRay = new Ray(near, direction);

            // Find intersection with world plane
            float? distance = mouseRay.Intersects(game.plane.plane);

            if (!distance.HasValue || float.IsNaN((float)distance))
                return;

            // Update the 'live' position
            // The intersection exists, and so, we can find the intersection coordinates
            livePosition = mouseRay.Position + mouseRay.Direction * (float)distance;

            // Update only when mouse is down
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                return;

            // Update the 'mouse down' position
            position = livePosition;

            // Update dummy model
            dummy.update(gameTime);
            dummy.world = Matrix.CreateScale(4f) * Matrix.CreateTranslation(position);
        }

        public void draw(Camera camera)
        {
            // Draw dummy model
            dummy.draw(camera);
        }
    }
}
