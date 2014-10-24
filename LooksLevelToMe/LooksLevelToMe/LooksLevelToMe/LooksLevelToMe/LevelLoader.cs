using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace LooksLevelToMe
{
    class LevelLoader
    {
        //The types of textures (symbols) being defined by and being used in the level
        private Dictionary<char, TileTexture> mTileTextures = new Dictionary<char, TileTexture>();
        
        //All of the tiles that make up the level
        public List<Tile> mTiles = new List<Tile>();

        //The starting position for the first tile. 
        int mStartX = 0;
        int mStartY = 0;

        //The default height and width for the tiles that make up the level
        int mHeight = 50;
        int mWidth = 50;

        public LevelLoader(string theFile, ContentManager theContent)
        {
            LoadLevelFile(theFile, theContent);
        }

        public void LoadLevelFile(string theLevelFile, ContentManager theContent)
        {
            //Cycle through the elements in the Level XML file. For each "tile" element encountered
            //load the tile information. For each "level" element encountered, load the level information
            XmlReader reader = XmlReader.Create(theLevelFile);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "tile":
                            {
                                LoadTile(reader, theContent);
                                break;
                            }

                        case "level":
                            {
                                LoadLevel(reader);
                                break;
                            }
                    }
                }
            }
        }
        
        //Load information about the tile defined in the Level XML file
        private void LoadTile(XmlReader reader, ContentManager theContent)
        {
            string currentElement = string.Empty;
            TileTexture aTileTexture = new TileTexture();

            while (reader.Read())
            {
                //Exit the While loop when the end node is encountered and add the Tile
                if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name.Equals("tile", StringComparison.OrdinalIgnoreCase))
                {
                    mTileTextures.Add(aTileTexture.Symbol, aTileTexture);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    currentElement = reader.Name;

                    switch (currentElement)
                    {
                        case "symbol":
                            {
                                aTileTexture.Symbol = reader.ReadElementContentAsString().ToCharArray()[0];
                                break;
                            }
                        case "texture":
                            {
                                LoadTexture(reader, theContent, aTileTexture);
                                break;
                            }
                        case "properties":
                            {
                                LoadProperties(reader, aTileTexture);
                                break;
                            }
                    }
                }
            }
        }

        private void LoadTexture(XmlReader reader, ContentManager theContent, TileTexture theTile)
        {
            string currentElement = string.Empty;
            Rectangle aSource = new Rectangle();

            while (reader.Read())
            {
                //Exit the While loop when the end node is encountered and add the Tile
                if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name.Equals("texture", StringComparison.OrdinalIgnoreCase))
                {
                    theTile.SourceRectangle = aSource;
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    currentElement = reader.Name;

                    switch (currentElement)
                    {
                        case "name":
                            {
                                string aAssetName = reader.ReadElementContentAsString();
                                theTile.Texture = theContent.Load<Texture2D>(aAssetName);
                                break;
                            }

                        case "startX":
                            {
                                aSource.X = reader.ReadElementContentAsInt();
                                break;
                            }

                        case "startY":
                            {
                                aSource.Y = reader.ReadElementContentAsInt();
                                break;
                            }

                        case "width":
                            {
                                aSource.Width = reader.ReadElementContentAsInt();
                                break;
                            }

                        case "height":
                            {
                                aSource.Height = reader.ReadElementContentAsInt();
                                break;
                            }
                    }
                }
            }



        }

        private void LoadProperties(XmlReader theReader, TileTexture theTile)
        {
            string aCurrentElement = string.Empty;

            while (theReader.Read())
            {
                if (theReader.NodeType == XmlNodeType.EndElement &&
                    theReader.Name.Equals("properties", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (theReader.NodeType == XmlNodeType.Element)
                {
                    aCurrentElement = theReader.Name;
                    switch (aCurrentElement)
                    {
                        case "walkable":
                            {
                                theTile.Walkable = theReader.ReadElementContentAsBoolean();
                                break;
                            }
                    }
                }
            }
        }

        private void LoadLevel(XmlReader theReader)
        {
            int aPositionY = 0;
            int aPositionX = 0;

            string aCurrentElement = string.Empty;

            while (theReader.Read())
            {
                if (theReader.NodeType == XmlNodeType.EndElement &&
                    theReader.Name.Equals("level", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (theReader.NodeType == XmlNodeType.Element)
                {
                    aCurrentElement = theReader.Name;
                    switch (aCurrentElement)
                    {
                        case "startX":
                            {
                                mStartX = theReader.ReadElementContentAsInt();
                                break;
                            }

                        case "startY":
                            {
                                mStartY = theReader.ReadElementContentAsInt();
                                break;
                            }

                        case "height":
                            {
                                mHeight = theReader.ReadElementContentAsInt();
                                break;
                            }
                        case "width":
                            {
                                mWidth = theReader.ReadElementContentAsInt();
                                break;
                            }
                    }
                }
                else if (theReader.NodeType == XmlNodeType.EndElement)
                {
                    if (aCurrentElement == "row")
                    {
                        //A new "row" of tiles is being defined for the level
                        //increase the Y Position of the tiles and reset the X Position
                        aPositionY += 1;
                        aPositionX = 0;
                    }
                }
                else if (theReader.NodeType == XmlNodeType.Text)
                {
                    if (aCurrentElement == "row")
                    {
                        //Cycle through all the elements in the current row to position
                        //the tiles at that Y position
                        string aRow = theReader.Value;

                        for (int aCounter = 0; aCounter < aRow.Length; ++aCounter)
                        {
                            bool isWalkable = true;
                            if (mTileTextures.ContainsKey(aRow[aCounter]) == true)
                            {
                                isWalkable = mTileTextures[aRow[aCounter]].Walkable;
                            }

                            //Add the tile in the row and increase the X position for the next tile in the row
                            mTiles.Add(new Tile(aRow[aCounter], new Vector2(aPositionX, aPositionY), isWalkable));
                            aPositionX += 1;
                        }
                    }
                }
            }
        }

        //Draw the currently loaded level
        public void Draw(SpriteBatch theBatch)
        {
            foreach (Tile aTile in mTiles)
            {
                if (mTileTextures.ContainsKey(aTile.Symbol) == true)
                {
                    //Scale the texture for the tile up or down based on the size defined for the level
                    float aScaleX = 1.0f;
                    aScaleX = (float)mWidth / (float)mTileTextures[aTile.Symbol].SourceRectangle.Width;
 
                    float aScaleY = 1.0f;
                    aScaleY = (float)mHeight / (float)mTileTextures[aTile.Symbol].SourceRectangle.Width;
 
                    //Draw the tile with it's texture at it's defined position from the level XML file
                    theBatch.Draw(mTileTextures[aTile.Symbol].Texture, new Vector2(mStartX + (aTile.Position.X * (mWidth)), mStartY + ((aTile.Position.Y) * (mHeight))), mTileTextures[aTile.Symbol].SourceRectangle, Color.White, 0.0f, new Vector2(0, 0), new Vector2(aScaleX, aScaleY), SpriteEffects.None, 0);                
                }
            }
        }
    }
}
