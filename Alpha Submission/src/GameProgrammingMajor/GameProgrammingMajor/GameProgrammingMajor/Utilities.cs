using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
