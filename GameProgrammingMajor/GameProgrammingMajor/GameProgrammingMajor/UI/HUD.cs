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
        public static string TEXT_DEBUG = "Camera (1,2,3): {2}\nTank A Priority: {3}    Tank B Priority: {4}";

        private SpriteFont uiFont;
        private string message = "";

        Player player;

        public bool visible = true; 
        public bool debugVisible = true;

        public HUD(MainGame game, Player player)
        {
            this.game = game;
            this.player = player;

            uiFont = game.Content.Load<SpriteFont>("Font\\UIFont");
        }

        public void update(UpdateParams updateParams)
        {
            Type camType = updateParams.camera.GetType();

            message = string.Format(
                TEXT_PLAYER + (debugVisible ? TEXT_DEBUG : ""),
                player.score,
                player.health,
                camType.ToString(),
                updateParams.world.npcManager.npcs[0].state,
                updateParams.world.npcManager.npcs[1].state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawParams"></param>
        public void draw(DrawParams drawParams)
        {
            if (!visible)
                return;

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
