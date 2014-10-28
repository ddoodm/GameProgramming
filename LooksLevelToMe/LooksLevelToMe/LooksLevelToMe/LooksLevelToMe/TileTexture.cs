using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LooksLevelToMe
{
    /// <summary>
    /// Represents a tile type definition from a level .XML file. This is what holds what Symbols represent
    /// each texture and any related properties.
    /// </summary>
    class TileTexture
    {
        public char Symbol;
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public bool Walkable;

        public TileTexture()
        {
        }
    }
}
