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


namespace _11688025_lab06
{
    /// <summary>
    /// Maintains a list of models to draw
    /// </summary>
    public class ModelManager
    {
        private Game game;
        public List<StaticModel> models = new List<StaticModel>();

        public ModelManager(Game game)
        {
            this.game = game;
        }

        public void LoadContent()
        {
            // Add target with a large collision sphere
            models.Add(new StaticModel(game, game.Content.Load<Model>("Models\\ShootHere"), new Vector3(0, 0, -175f), 50f));

            // Add a boingy ball
            models.Add(new BoingyBall(game, game.Content.Load<Model>("Models\\DSphere")));
        }

        public void Update(GameTime gameTime)
        {
            foreach (StaticModel m in models)
                m.update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (StaticModel m in models)
            {
                m.draw(((Game1)game).camera);
                //m.draw_axis(gameTime);
            }
        }
    }
}
