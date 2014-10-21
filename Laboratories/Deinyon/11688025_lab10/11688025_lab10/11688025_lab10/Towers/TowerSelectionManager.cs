using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab10
{
    /// <summary>
    /// Performs a ray-intersection computation
    /// to determine the currently selected block
    /// </summary>
    public class TowerSelectionManager
    {
        private TowerManager towerManager;

        public Plane plane;

        public TowerSelectionManager(TowerManager towerManager)
        {
            this.towerManager = towerManager;

            computePlane();
        }

        private void computePlane()
        {
            plane = new Plane(Vector3.Up, 0);
        }

        /// <summary>
        /// Performs a ray-intersection on the unprojected mouse position.
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public iVec2 getSelectedBlock(Viewport viewport, Camera camera, MouseState mouseState)
        {
            // Obtain mouse coordinates in Cartesian screen space as they
            // lie on the near and far planes
            Vector2 mouse = new Vector2(mouseState.X, mouseState.Y);
            Vector3 nearMouse = new Vector3(mouse, 0);
            Vector3 farMouse = new Vector3(mouse, 1);

            // Perform a reverse perspective projection on the mouse coordinates
            // and obtain a near-far vector delta
            Vector3 near = viewport.Unproject(
                nearMouse, camera.projection, camera.view, towerManager.world);
            Vector3 far = viewport.Unproject(
                farMouse, camera.projection, camera.view, towerManager.world);

            // Obtain a ray that projects from the near mouse to the far mouse
            Vector3 direction = Vector3.Normalize(far - near);
            Ray mouseRay = new Ray(near, direction);

            // Perform the ray-intersection
            return getIntersectionBlockId(mouseRay);
        }

        /// <summary>
        /// Performs a ray-intersection to compute the currently
        /// selected block ID.
        /// </summary>
        /// <param name="ray">A ray which should be tested against the Tower grid.</param>
        /// <returns>The two-dimensional ID of the block that is selected. (-1,-1) if none.</returns>
        public iVec2 getIntersectionBlockId(Ray ray)
        {
            Vector3 intPosition;

            // Find intersection with plane
            float? distance = ray.Intersects(plane);

            // The ray does not intersect
            if (!distance.HasValue || float.IsNaN((float)distance))
                return new iVec2(-1,-1);

            // The intersection exists, and so, we can find the intersection coordinates
            intPosition = ray.Position + ray.Direction * (float)distance;

            // Compute the block ID of the intersection
            return towerManager.idOf(intPosition);
        }
    }
}
