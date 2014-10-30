using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Two-dimensional integer vector
    /// </summary>
    public struct iVec2
    {
        public int x, y;

        public iVec2(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public iVec2(Vector2 vec)
        {
            this.x = (int)vec.X; this.y = (int)vec.Y;
        }

        public iVec2(Point point)
        {
            this.x = point.X;
            this.y = point.Y;
        }

        public static bool operator== (iVec2 lhs, iVec2 rhs)
        {
            return (lhs.x == rhs.x) && (lhs.y == rhs.y);
        }

        public static bool operator!= (iVec2 lhs, iVec2 rhs)
        {
            return (lhs.x != rhs.x) || (lhs.y != rhs.y);
        }
    }
}
