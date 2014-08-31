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


namespace _116880025_lab04
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public List<StaticModel> models = new List<StaticModel>();

        public ModelManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Load models into model list(s).
        /// </summary>
        protected override void LoadContent()
        {
            models.Add(new StaticModel(Game, Game.Content.Load<Model>("Models\\DFountain")));
            models.Add(new StaticModel(Game, Game.Content.Load<Model>("Models\\DPlane")));
            models.Add(new Skybox(Game, Game.Content.Load<Model>("Models\\DSkyboxMesh")));
            models.Add(new BoingyBall(Game, Game.Content.Load<Model>("Models\\DSphere")));

            base.LoadContent();
        }

        /// <summary>
        /// Invoke update() on all models.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (StaticModel m in models)
                m.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws all models.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (StaticModel m in models)
            {
                m.draw(((Game1)Game).camera);
                m.draw_axis();
            }

            base.Draw(gameTime);
        }
    }
}
