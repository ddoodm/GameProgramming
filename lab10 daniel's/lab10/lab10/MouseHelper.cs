using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace lab10
{
    static class MouseHelper
    {
        public static Vector3? mousepickingPlane(Game1 game, Plane plane)
        {
            MouseState mouseState = Mouse.GetState();
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;
            Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
            Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearsource,
                game.camera.projection, game.camera.view, world);

            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farsource,
                game.camera.projection, game.camera.view, world);
            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);
            //ground plane can replace with boundings of colidable objects
            //Plane plane = new Plane(Vector3.Up, 0);
            float? dist = pickRay.Intersects(plane);
            if (dist == null) return null;
            return nearPoint + direction * dist;
        } 

    }
}
