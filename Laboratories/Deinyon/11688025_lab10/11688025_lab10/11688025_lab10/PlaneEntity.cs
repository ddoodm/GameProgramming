using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab10
{
    /// <summary>
    /// A simple mechanism for creating and drawing a plane
    /// </summary>
    public class PlaneEntity : PrimitiveEntity<VertexPositionNormalTexture>
    {
        PlanePrimitive plane;

        public PlaneEntity(Game game, PlanePrimitive plane, Vector3 position, float rotation)
            : base(game, plane, position, rotation)
        {
            this.plane = plane;
        }

        public override bool collidesWith(StaticModel other)
        {
            foreach (ModelMesh mesh in other.model.Meshes)
                if (this.collidesWith(mesh.BoundingSphere))
                    return true;
            return false;
        }

        public override bool collidesWith(BoundingSphere boundingSphere)
        {
            if (boundingSphere.Intersects(plane.plane) == PlaneIntersectionType.Back)
                return true;
            return false;
        }
    }
}
