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

        public static string TEXT_PLAYER = "Teapot Health: {0:0.0}\nPlayer Cash: ${1:0.00}\nPlacing Block (4,5,6,7,8): {2}\n";
        public static string TEXT_DEBUG = "Camera (1,2,3): {3}\nQuadtree Nodes: {4}";

        // Constants from Photoshop
        private int HEALTHBAR_START_X = 665, HEALTHBAR_START_Y = 35, HEALTHBAR_WIDTH = 437, HEALTHBAR_HEIGHT = 19;
        private Texture2D healthbar;

        private SpriteFont uiFont;
        private string message = "";

        private Texture2D uiBG;
        private Texture2D gameOverSprite;
        private Texture2D levelCompleteSprite;
        private Texture2D blackTexture;
        private Texture2D[] towerTexes;
        private Texture2D cTowerTex;

        private const int deathMapSize = 200;
        private Texture2D deathMap;

        public int quadtreeNodeCount;

        private float teapotHealth;
        public bool teapotDead;
        public float playerMoney;
        public TowerType blockToPlace;
        public bool levelComplete;

        public bool visible = true; 
        public bool debugVisible = true;

        public HUD(MainGame game)
        {
            this.game = game;

            uiBG = game.Content.Load<Texture2D>("Textures\\UI\\TDS14_UI");
            uiFont = game.Content.Load<SpriteFont>("Font\\UIFont");
            gameOverSprite = game.Content.Load<Texture2D>("Textures\\UI\\GameOver");
            levelCompleteSprite = game.Content.Load<Texture2D>("Textures\\UI\\LevelComplete");

            // Load tower images
            towerTexes = new Texture2D[]
            {
                game.Content.Load<Texture2D>("Textures\\UI\\tower_grass_small"),
                game.Content.Load<Texture2D>("Textures\\UI\\tower_wall_small"),
                game.Content.Load<Texture2D>("Textures\\UI\\tower_path_small"),
                game.Content.Load<Texture2D>("Textures\\UI\\tower_tar_small"),
                game.Content.Load<Texture2D>("Textures\\UI\\tower_turret_small"),
            };
            cTowerTex = towerTexes[0];

            // Make a 1x1 semi-transparent texture
            blackTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            blackTexture.SetData(new Color[] {new Color(0, 0, 0, 0.75f)});

            // Null deathmap texture
            deathMap = new Texture2D(game.GraphicsDevice, 1, 1);
            deathMap.SetData(new Color[] { new Color(0, 0, 0, .75f) });

            healthbar = new Texture2D(game.GraphicsDevice, 1, 1);
            healthbar.SetData(new Color[] { new Color(.8f, 0, 0, .15f) });
        }

        public void setDeathMap(Texture2D deathMap)
        {
            this.deathMap = deathMap;
        }

        public void setPlayerMoney(float money)
        {
            this.playerMoney = money;
        }

        public void setTeapotHealth(float health)
        {
            teapotHealth = health;
            if (teapotHealth < 0) teapotHealth = 0;
            if (teapotHealth > 1) teapotHealth = 1;

            healthbar.SetData(new Color[] { new Color(1f - .8f * health, .8f * health, 0, .15f) });
        }

        public void setBlockToPlace(TowerType blockToPlace)
        {
            this.blockToPlace = blockToPlace;

            if((int)blockToPlace < towerTexes.Length)
                cTowerTex = towerTexes[(int)blockToPlace];
        }

        public void update(UpdateParams updateParams)
        {
            Type camType = updateParams.camera.GetType();

            message = string.Format(
                TEXT_PLAYER + (debugVisible ? TEXT_DEBUG : ""),
                teapotHealth,
                playerMoney,
                Enum.GetName(typeof(TowerType), blockToPlace),
                camType.ToString(),
                quadtreeNodeCount);

            if (teapotDead || levelComplete)
                if (updateParams.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                    game.startOver();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawParams"></param>
        public void draw(DrawParams drawParams, SpriteBatch spriteBatch)
        {
            if (levelComplete)
                drawLevelComplete(drawParams, spriteBatch);
            else if (teapotDead)
                drawGameOver(drawParams, spriteBatch);

            if (!visible)
                return;

            game.spriteBatch.Begin();
            
            // Draw UI background
            game.spriteBatch.Draw(
                uiBG, game.GraphicsDevice.Viewport.Bounds, Color.White);

            // Debug text
            drawText(message, new Vector2(20, 80), spriteBatch);

            // Draw cash
            drawText(String.Format("Cash: ${0:0.00}", playerMoney), new Vector2(95, 35), spriteBatch);

            // Draw health bar
            int healthbarWidth = (int)(teapotHealth * (float)HEALTHBAR_WIDTH);
            Rectangle healthBarRect = new Rectangle(HEALTHBAR_START_X, HEALTHBAR_START_Y, healthbarWidth, HEALTHBAR_HEIGHT);
            game.spriteBatch.Draw(
                healthbar, healthBarRect, Color.White);

            // Draw "block to place"
            game.spriteBatch.Draw(
                cTowerTex,
                new Rectangle(
                    game.GraphicsDevice.Viewport.Width - cTowerTex.Width - 50 - 22, 0,
                    cTowerTex.Width + 50, cTowerTex.Height + 50), Color.White);

            // Draw death map
            if (deathMap != null)
            {
                game.spriteBatch.Draw(
                    deathMap, new Rectangle(
                        game.GraphicsDevice.Viewport.Width - deathMapSize - 15,
                        game.GraphicsDevice.Viewport.Height - deathMapSize - 32,
                        deathMapSize, deathMapSize), Color.White);

                Vector2 textPosition = new Vector2(
                    game.GraphicsDevice.Viewport.Width - deathMapSize,
                    game.GraphicsDevice.Viewport.Height - 32);

                drawText("Kill Heatmap", textPosition, spriteBatch);
            }

            game.spriteBatch.End();

            // Restore GD states after spriteBatch draw
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void drawText(string text, Vector2 position, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 2; i++)
            {
                game.spriteBatch.DrawString(
                    uiFont,                                                 // Font object
                    text,                                                   // String
                    new Vector2(position.X - i * 2, position.Y - i * 2),    // Position
                    i == 0 ? Color.Black : Color.White);
            }
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

        private void drawLevelComplete(DrawParams drawParams, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            Rectangle view = drawParams.graphicsDevice.Viewport.Bounds;
            Vector2 midView = new Vector2(view.Center.X, view.Center.Y);
            midView -= new Vector2(levelCompleteSprite.Bounds.Center.X, levelCompleteSprite.Bounds.Center.Y);

            spriteBatch.Draw(blackTexture, view, Color.White);
            spriteBatch.Draw(levelCompleteSprite, midView, Color.White);

            spriteBatch.End();
        }
    }
}
