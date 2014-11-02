using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameProgrammingMajor
{
    public struct LevelDescription
    {
        public int[] numberToSpawn;
        public int[] typesToSpawn;

        public int width, height;
        public float money;

        public int[,] indices;
    }
}
