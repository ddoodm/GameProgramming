using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace minesweeper
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Button button;
        TextboxHandler textboxHandler;
        Textbox preset1;
        Textbox preset2;
        Level level;
        Player[] players;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont font1 = Content.Load<SpriteFont>(@"TextBoxFont");
            Texture2D texture = Content.Load<Texture2D>(@"Images\selection");
            List<Textbox> textboxs = new List<Textbox>();
            Textbox textbox = new Textbox(new Rectangle(0,0,35, 28), font1, "", 3, "0123456789", null, texture);
            textboxs.Add(textbox);
            textbox = new Textbox(new Rectangle(35, 0, 35, 28),font1, "", 3, "0123456789", null, texture);
            textboxs.Add(textbox);
            textbox = new Textbox(new Rectangle(70, 0, 35, 28), font1, "", 3, "0123456789", null, texture);
            textboxs.Add(textbox);
            textboxHandler = new TextboxHandler(textboxs, Vector2.Zero);
            button = new Button(new Rectangle(105, 0, 30, 28), texture);
            preset1 = new Textbox(new Rectangle(135, 0, 192, 28), font1, "" + 17%16 , -1, null, null, texture);
            preset2 = new Textbox(new Rectangle(324,0,192,28), font1, "16 by 16 40 bombs", -1, null, null, texture);
            Texture2D path = Content.Load<Texture2D>(@"Images\path");//"9 by 9 10 bombs"
            Texture2D bomb = Content.Load<Texture2D>(@"images\bomb");
            level = new Level(path, font1, bomb);
            players = new Player[2];
            textbox = new Textbox(new Rectangle(135 + 192 + 192, 0, 35, 28), font1, "" + (int)15/16, 3, "0123456789", null, texture);
            players[0] = new Player(Color.Green, textbox);
            textbox = new Textbox(new Rectangle(135 + 192 + 192 + 35, 0, 35, 28), font1, "" + (int)17 / 16, 3, "0123456789", null, texture);
            players[1] = new Player(Color.Blue, textbox);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            textboxHandler.Update();

            button.Update();
            String textboxtext1 = textboxHandler.getTextbox(0).getText();
            String textboxtext2 =  textboxHandler.getTextbox(1).getText();
            String textboxtext3 = textboxHandler.getTextbox(2).getText();
            if (button.getLeftPressed() && textboxtext1.Length > 0 && textboxtext2.Length > 0 && textboxtext3.Length > 0)
            {
                int textboxnum1 = Convert.ToInt32(textboxtext1);
                int textboxnum2 = Convert.ToInt32(textboxtext2);
                int textboxnum3 = Convert.ToInt32(textboxtext3);
                if (textboxnum3 < textboxnum1 * textboxnum2) level.createLevel(textboxnum1, textboxnum2, textboxnum3);
            }
            preset1.Update();
            if (preset1.getLeftPressed())
            {
                level.createLevel(9, 9, 10);
            }
            preset2.Update();
            if (preset2.getLeftPressed())
            {
                level.createLevel(16, 16, 40);
            }
            //level.Update(players[activePlayer]);
            foreach (Player player in players)
            {
                player.Update();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
                textboxHandler.Draw(spriteBatch);
                button.Draw(spriteBatch);
                level.Draw(spriteBatch);
                preset1.Draw(spriteBatch);
                preset2.Draw(spriteBatch);
                foreach (Player player in players)
                {
                    player.Draw(spriteBatch);
                }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
