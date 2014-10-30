using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.IO;
using System.Text;

namespace lab10
{
    static class LevelLoader
    {
        private static int ASCII_TO_INT_CONVERSION = -48;
        public static LevelData Load(string theFile, ContentManager theContent)
        {
            return LoadLevelFile(theFile, theContent);
        }
        private static LevelData LoadLevelFile(string theLevelFile, ContentManager theContent)
        {
            LevelData data = new LevelData();
            XmlReader reader = XmlReader.Create(theLevelFile);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "waves":
                            {
                                LoadWaves(reader, data);
                                break;
                            }
                        
                        case "level":
                            {
                                LoadLevel(reader, data);
                                break;
                            }
                    }
                }
            }
            return data;
        }
        private static void LoadLevel(XmlReader theReader, LevelData data)
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
                        /*case "startX":
                            {
                                mStartX = theReader.ReadElementContentAsInt();
                                break;
                            }

                        case "startY":
                            {
                                mStartY = theReader.ReadElementContentAsInt();
                                break;
                            }
                        */
                        case "height":
                            {
                                data.height = theReader.ReadElementContentAsInt();
                                break;
                            }
                        case "width":
                            {
                                data.width = theReader.ReadElementContentAsInt();
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
                        if(aPositionY == 0)data.indexs = new int[data.height, data.width];//this is backward becuse of have my level is set up
                        string aRow = theReader.Value;
                        for (int aCounter = 0; aCounter < aRow.Length; ++aCounter)
                        {
                            data.indexs[aPositionY, aPositionX] = aRow.ElementAt<char>(aCounter) + ASCII_TO_INT_CONVERSION;
                            aPositionX += 1;
                        }
                    }
                }
            }
        
        }
        private static void LoadWaves(XmlReader theReader, LevelData data)
        {
            string aCurrentElement = string.Empty;
            while (theReader.Read())
            {
                if (theReader.NodeType == XmlNodeType.EndElement &&
                    theReader.Name.Equals("waves", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (theReader.NodeType == XmlNodeType.Element)
                {
                    aCurrentElement = theReader.Name;
                    switch (aCurrentElement)
                    {
                        /*case "waveNumbers":
                        {
                            data.numberToSpawn = stringToIntArray(theReader.Value);//Array.ConvertAll<char, int>(theReader.ReadContentAsString().ToArray<char>(), );
                            break;
                        */
                    }
                }
                else if (theReader.NodeType == XmlNodeType.Text)
                {
                    if (aCurrentElement == "waveNumbers")
                    {
                        data.numberToSpawn = stringToIntArray(theReader.ReadContentAsString(), ';');
                    }
                }

            }
        }
        private static int[] stringToIntArray(string input, char spliter)
        {
            String[] charArray = input.Split(spliter);
            int[] intArray = new int[charArray.Length];
            for (int aCounter = 0; aCounter < charArray.Length; ++aCounter)
            {
                intArray[aCounter] = Convert.ToInt32(charArray[aCounter]);
            }
            return intArray;
        }
    }
}

