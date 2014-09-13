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

namespace pathfinding
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Level level = new Level();
        Pathfinder pathfinder;
        List<Vector2> path;
        Mover mover;
        MouseState preMouse;

        Texture2D target;
        int targetX;
        int targetY;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = level.Width * 32;
            graphics.PreferredBackBufferHeight = level.Height * 32;
            graphics.ApplyChanges();
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            pathfinder = new Pathfinder(level);
            //pathfinding to do fast road squares
            //is side ways???
            //cant have rectangle??
            path = pathfinder.FindPath(new Point(0, 0), new Point(0, 8));
            mover = new Mover();
            mover.addPath(path);
            preMouse = Mouse.GetState();
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
            Texture2D path = Content.Load<Texture2D>(@"Images\path");
            Texture2D grass = Content.Load<Texture2D>(@"Images\grass");
            Texture2D marker = Content.Load<Texture2D>(@"Images\marker");
            Texture2D road = Content.Load<Texture2D>(@"Images\road");
            Texture2D tar = Content.Load<Texture2D>(@"Images\tar");

            Texture2D movertexture = Content.Load<Texture2D>(@"Images\mover");
            target = Content.Load<Texture2D>(@"Images\target");
            level.AddTexture(path);
            level.AddTexture(road);
            level.AddTexture(grass);
            level.AddTexture(tar);

            mover.addTexture(marker);
            mover.addMoverTexture(movertexture);

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
            KeyboardState keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            mover.Update(gameTime, level);
            MouseState mouse = Mouse.GetState();
            targetX = mouse.X / 32;
            targetY = mouse.Y / 32;
            if (mouse.LeftButton == ButtonState.Pressed && preMouse.LeftButton != ButtonState.Pressed)
            {
                if (targetX >= 0 && targetX < level.Width && targetY >= 0 && targetY < level.Height && level.GetIndex(targetX, targetY) != 0)
                {
                    Point startPoint = new Point((int)mover.position.X/32, (int)mover.position.Y/32);
                    Point endPoint = new Point(targetX, targetY);
                    path = pathfinder.FindPath(startPoint, endPoint);
                    mover.addPath(path);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void DrawTarget(SpriteBatch batch)
        {
            if (targetX >= 0 && targetX < level.Width && targetY >= 0 && targetY < level.Height && level.GetIndex(targetX, targetY) != 0)
            {
                batch.Draw(target, new Rectangle(
                    targetX * 32, targetY * 32, 32, 32), Color.White);
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            level.Draw(spriteBatch);
            mover.Draw(spriteBatch);
            DrawTarget(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
