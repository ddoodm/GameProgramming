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

        public static string TEXT_PLAYER = "Health: {0:0.0}\n";
        public static string TEXT_DEBUG = "Camera (1,2,3): {1}\nQuadtree Nodes: {2}";

        private SpriteFont uiFont;
        private string message = "";

        private Texture2D gameOverSprite;
        private Texture2D blackTexture;

        private const int deathMapSize = 200;
        private Texture2D deathMap;

        Player player;

        public int quadtreeNodeCount;

        public bool visible = true; 
        public bool debugVisible = true;

        public HUD(MainGame game, Player player)
        {
            this.game = game;
            this.player = player;

            uiFont = game.Content.Load<SpriteFont>("Font\\UIFont");
            gameOverSprite = game.Content.Load<Texture2D>("Textures\\UI\\GameOver");

            // Make a 1x1 semi-transparent texture
            blackTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            blackTexture.SetData(new Color[] {new Color(0, 0, 0, 0.75f)});

            // Null deathmap texture
            deathMap = new Texture2D(game.GraphicsDevice, 1, 1);
            deathMap.SetData(new Color[] { new Color(0, 0, 0, .75f) });
        }

        public void setDeathMap(Texture2D deathMap)
        {
            this.deathMap = deathMap;
        }

        public void update(UpdateParams updateParams)
        {
            Type camType = updateParams.camera.GetType();

            message = string.Format(
                TEXT_PLAYER + (debugVisible ? TEXT_DEBUG : ""),
                player.health,
                camType.ToString(),
                quadtreeNodeCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawParams"></param>
        public void draw(DrawParams drawParams, SpriteBatch spriteBatch)
        {
            if(player.isDead)
                drawGameOver(drawParams, spriteBatch);

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

            // Draw death map
            if (deathMap != null)
            {
                game.spriteBatch.Draw(
                    deathMap, new Rectangle(
                        game.GraphicsDevice.Viewport.Width - deathMapSize - 15, 15,
                        deathMapSize, deathMapSize), Color.White);
            }

            game.spriteBatch.End();

            // Restore GD states after spriteBatch draw
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void drawGameOver(DrawParams drawParams, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            Rectangle view = drawParams.graphicsDevice.Viewport.Bounds;
            Vector2 midView = new Vector2(view.Center.X, view.Center.Y);
            midView -= new Vector2(gameOverSprite.Bounds.Center.X, gameOverSprite.Bounds.Center.Y);

            spriteBatch.Draw(blackTexture, view, Color.White);
            spriteBatch.Draw(gameOverSprite, midView, Color.White);

            spriteBatch.End();
        }
    }
}
