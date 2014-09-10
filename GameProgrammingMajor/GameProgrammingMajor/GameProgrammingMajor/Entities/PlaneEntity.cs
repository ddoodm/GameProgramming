using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// A simple mechanism for creating and drawing a plane
    /// </summary>
    public class PlaneEntity : PrimitiveEntity<VertexPositionNormalTexture>
    {
        public PlaneEntity(Game game, PlanePrimitive plane, Vector3 position, float rotation)
            : base(game, plane, position, rotation)
        {

        }
    }
}
