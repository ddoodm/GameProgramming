using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    public class LevelLoader
    {
        // Dodgy, dodgy, dodgy hack
        private static int ASCII_TO_INT_CONVERSION = -48;

        public static LevelDescription loadLevel(string path, ContentManager content)
        {
            XmlReader xml;
            try { xml = XmlReader.Create(path); }
            catch { throw new Exception("Level XML File \"" + path + "\" does not exist. "); }
            LevelDescription level = new LevelDescription();

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    switch (xml.Name)
                    {
                        case "waves":
                            {
                                level = loadWaves(xml, level);
                                break;
                            }

                        case "level":
                            {
                                level = loadLevel(xml, level);
                                break;
                            }
                    }
                }
            }

            return level;
        }

        private static LevelDescription loadWaves(XmlReader xml, LevelDescription level)
        {
            string aCurrentElement = "";
            while (xml.Read())
            {
                // Read until the tag ends
                if (xml.NodeType == XmlNodeType.EndElement &&
                    xml.Name.Equals("waves", StringComparison.OrdinalIgnoreCase))
                    break;

                if (xml.NodeType == XmlNodeType.Element)
                {
                    aCurrentElement = xml.Name;
                    switch (aCurrentElement)
                    {
                        /*case "waveNumbers":
                        {
                            data.numberToSpawn = stringToIntArray(theReader.Value);//Array.ConvertAll<char, int>(theReader.ReadContentAsString().ToArray<char>(), );
                            break;
                        */
                    }
                }
                else if (xml.NodeType == XmlNodeType.Text)
                {
                    if (aCurrentElement == "waveNumbers")
                    {
                        level.numberToSpawn = stringToIntArray(xml.ReadContentAsString(), ';');
                    }
                }
            }

            return level;
        }

        private static LevelDescription loadLevel(XmlReader xml, LevelDescription level)
        {
            int aPositionY = 0;
            int aPositionX = 0;

            string aCurrentElement = string.Empty;

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.EndElement &&
                    xml.Name.Equals("level", StringComparison.OrdinalIgnoreCase))
                    break;

                if (xml.NodeType == XmlNodeType.Element)
                {
                    aCurrentElement = xml.Name;
                    switch (aCurrentElement)
                    {
                        case "money":
                            level.money = xml.ReadElementContentAsFloat();
                            break;
                        case "height":
                            level.height = xml.ReadElementContentAsInt();
                            break;
                        case "width":
                            level.width = xml.ReadElementContentAsInt();
                            break;
                    }
                }
                else if (xml.NodeType == XmlNodeType.EndElement)
                {
                    if (aCurrentElement == "row")
                    {
                        //A new "row" of tiles is being defined for the level
                        //increase the Y Position of the tiles and reset the X Position
                        aPositionY += 1;
                        aPositionX = 0;
                    }
                }
                else if (xml.NodeType == XmlNodeType.Text)
                {
                    if (aCurrentElement == "row")
                    {
                        //Cycle through all the elements in the current row to position
                        //the tiles at that Y position
                        if (aPositionY == 0) level.indices = new int[level.height, level.width];//this is backward becuse of have my level is set up
                        string aRow = xml.Value;
                        for (int aCounter = 0; aCounter < aRow.Length; ++aCounter)
                        {
                            level.indices[aPositionY, aPositionX] =
                                aRow.ElementAt<char>(aCounter) + ASCII_TO_INT_CONVERSION;
                            aPositionX += 1;
                        }
                    }
                }
            }

            return level;
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
