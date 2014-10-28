using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace LooksLevelToMe
{
    /// <summary>
    /// An individual tile that makes up a larger image being drawn on the screen.
    /// This class can be enhanced with more properties that make sense for your
    /// game. Walkable is one example of that type of property.
    /// </summary>
    class Tile
    {
        public Vector2 Position;
        public char Symbol;
        public bool Walkable;

        public Tile(char theSymbol, Vector2 thePosition, bool isWalkable)
        {
            Symbol = theSymbol;
            Position = thePosition;
            Walkable = isWalkable;
        }
    }
}
