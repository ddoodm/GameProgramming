using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class HUD
    {
        MainGame game;

        public static string TEXT_PLAYER = "Score: {0}    Health: {1}\n";
        public static string TEXT_DEBUG = "Camera (1,2,3): {2}";

        SpriteFont uiFont;

        Player player;
        Entity baseTower;

        public bool visible = true; 
        public bool debugVisible = true;

        public HUD(MainGame game, Player player)
        {
            this.game = game;
            this.player = player;

            uiFont = game.Content.Load<SpriteFont>("Font\\UIFont");
        }

        /// <summary>
        /// Display:
        /// Score,
        /// Health,
        /// Time,
        /// Enemy wave information,
        /// Debug
        /// </summary>
        /// <param name="drawParams"></param>
        public void draw(DrawParams drawParams)
        {
            if (!visible)
                return;

            Type camType = drawParams.camera.GetType();
            string message = string.Format(
                TEXT_PLAYER + (debugVisible? TEXT_DEBUG : ""),
                player.score,
                player.health,
                drawParams.camera.GetType().ToString());

            game.spriteBatch.Begin();
            for (int i = 0; i < 2; i++)
            {
                game.spriteBatch.DrawString(
                    uiFont,                                 // Font object
                    message,                                // String
                    new Vector2(20 - i * 2, 20 - i * 2),    // Position
                    i == 0 ? Color.Black : Color.White);
            }
            game.spriteBatch.End();

            // Restore GD states after spriteBatch draw
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
