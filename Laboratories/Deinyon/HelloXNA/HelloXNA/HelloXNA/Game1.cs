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

namespace HelloXNA
{
    enum KeyMasks
    {
        W = 0xFE,   // 1111 1110
        S = 0xFD,   // 1111 1101
        A = 0xFB,   // 1111 1011
        D = 0xF7    // 1111 0111
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Fractional FPS ( 1.0 / totalSeconds )
        double fps;

        // Binary bitfield for WASD keys
        int bitField;

        SoundEffect         sf_motion;
        Song                sng_bg;
        SoundEffectInstance sfi_motion, sfi_background;

        Texture2D tx_soHardcore;
        SpriteFont dFont;
        Vector2 dTextPos;

        ThreeRings threeRings;

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
            // TODO: Add your initialization logic here

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

            // Use existing font resource
            dFont = Content.Load<SpriteFont>("SpriteFont1");

            // Load smiley sprite
            tx_soHardcore = Content.Load<Texture2D>("AwesomeSmiley2"); 
          
            // Vector defining viewport centre
            dTextPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height / 2);

            // ThreeRings
            threeRings = new ThreeRings(this);

            // Motion sound effect
            sf_motion = Content.Load<SoundEffect>("start");
            sfi_motion = sf_motion.CreateInstance();

            // Background audio loop
            sng_bg = Content.Load<Song>("imaginaryFriend");
            MediaPlayer.Play(sng_bg);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape) )
                this.Exit();

            // Update bitfield for WASD keys (an experiment)
            KeyboardState keyState = Keyboard.GetState();
            bitField =
                (keyState.IsKeyDown(Keys.W) ? 1 << 0 : 0) |
                (keyState.IsKeyDown(Keys.S) ? 1 << 1 : 0) |
                (keyState.IsKeyDown(Keys.A) ? 1 << 2 : 0) |
                (keyState.IsKeyDown(Keys.D) ? 1 << 3 : 0);

            // Update text position at 100 units per second
            update_text(gameTime);

            // TODO: Add your update logic here
            threeRings.update(gameTime);

            base.Update(gameTime);
        }

        void update_text(GameTime gameTime)
        {
            // Play audio when a key is pressed
            if (bitField > 0) sfi_motion.Play();
            else sfi_motion.Stop();

            // Displace by 100 units per second
            int D = (int)Math.Round(100.0 * fps);

            if ((bitField | (int)KeyMasks.W) == 0xFF)
                dTextPos.Y-=D;
            if ((bitField | (int)KeyMasks.S) == 0xFF)
                dTextPos.Y+=D;
            if ((bitField | (int)KeyMasks.A) == 0xFF)
                dTextPos.X-=D;
            if ((bitField | (int)KeyMasks.D) == 0xFF)
                dTextPos.X+=D;
        }

        /// <summary>
        /// Called on window invalidation
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // FPS is 1.0 / seconds since last draw (should be fractional)
            fps = 1.0 / 1000.0 * gameTime.ElapsedGameTime.TotalMilliseconds;

            float colT = 0.75f * (float)gameTime.TotalGameTime.TotalSeconds;
            GraphicsDevice.Clear(
                new Color( 0.4f + 0.3f*(float)Math.Sin(colT),
                           0.1f,
                           0.4f + 0.3f*(float)Math.Cos(colT)));

            spriteBatch.Begin();

            // Text sprite drawing attempt
            string text = String.Format("Greetings, world!\nFPS:        {0:F1}\nReciprocal: {1:F4}", 1.0/fps, fps);
            Vector2 fontMid = dFont.MeasureString(text);

            spriteBatch.DrawString(
                dFont,                                         // Font object
                text,                                          // String
                dTextPos - fontMid / 2f + new Vector2(4,4),    // Position
                new Color(0, 0, 0));
            spriteBatch.DrawString(
                dFont,                  // Font object
                text,                   // String
                dTextPos - fontMid/2f,  // Position
                new Color(0xFF, 0xFF, 0xFF));

            draw_face(colT, fontMid);

            threeRings.draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void draw_face (float colT, Vector2 fontMid)
        {
            Vector2 tex_center = new Vector2(tx_soHardcore.Bounds.Center.X, tx_soHardcore.Bounds.Center.Y);
            Vector2 tex_pos = dTextPos + new Vector2(fontMid.X * (float)Math.Sin(colT), fontMid.Y * (float)Math.Cos(colT));

            spriteBatch.Draw(
                tx_soHardcore,
                new Rectangle((int)tex_pos.X, (int)tex_pos.Y, tx_soHardcore.Width/4, tx_soHardcore.Height/4),
                tx_soHardcore.Bounds,
                new Color(0xFF, 0xFF, 0xFF, 0x50),
                colT*1.8f,
                tex_center,
                SpriteEffects.None,
                1);
        }

        class ThreeRings
        {
            private Game1 game;
            private Texture2D sheet;
            private int
                dTime = 0,
                animSpeed = 50,
                cFrameId = 0;
            private Point
                frameSize = new Point(75, 75),
                cFrame = new Point(0, 0),
                pageSize = new Point(6, 8);

            public ThreeRings(Game1 game)
            {
                this.game = game;
                sheet = game.Content.Load<Texture2D>("threerings");
            }

            public void update(GameTime gameTime)
            {
                // Animate sprite sheet at 50 tiles / second, instead of game frame rate
                dTime += gameTime.ElapsedGameTime.Milliseconds;
                if (dTime > animSpeed)
                {
                    dTime -= animSpeed;

                    // Mod-based frame selection.
                    cFrameId = (cFrameId+1) % (pageSize.X * pageSize.Y);
                    cFrame.X = cFrameId % pageSize.X;
                    cFrame.Y = (cFrameId / pageSize.X) % pageSize.Y;
                }
            }

            public void draw(GameTime gameTime)
            {
                game.spriteBatch.Draw(sheet,
                    new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                    new Rectangle(
                        cFrame.X * frameSize.X,
                        cFrame.Y * frameSize.Y,
                        frameSize.X,
                        frameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None, 0);
            }
        }
    }
}
