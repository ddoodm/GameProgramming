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
    class PlaneEntity : PrimitiveEntity<VertexPositionNormalTexture>
    {
        public PlaneEntity(Game game, Vector3 position, Vector3 up, float size, float rotation)
            : base(game, new PlanePrimitive(game, size, up), position, rotation)
        {

        }
    }
}
