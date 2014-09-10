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


namespace lab4._1
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();
        Tank tank, stalkerTank;
        public ModelManager(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            models.Add(new BasicModel(Game.Content.Load<Model>(@"Models\spaceship"), new Vector3(0, 10, 0), 1));
            models.Add(new BasicModel(Game.Content.Load<Model>(@"Models\spaceship"), new Vector3(-50, 10, 25), 1));
            tank = new Tank(Matrix.CreateScale(1 / 60f) * Matrix.CreateTranslation(new Vector3(25, 0, 0)));
            tank.Load(Game.Content);
            stalkerTank = new Tank(Matrix.CreateScale(1 / 60f) * Matrix.CreateTranslation(new Vector3(-50, 0, 0)));
            stalkerTank.Load(Game.Content);
            base.LoadContent();
        }
        public void addWayPoint(Vector3 position)
        {
            tank.addWayPoint(position);
        }
        public override void Update(GameTime gameTime)
        {    // Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i){
                models[i].Update();   
            }
            tank.Update(gameTime);
            if (Mouse.GetState().RightButton == ButtonState.Pressed) stalkerTank.addTargetEntity(tank);
            stalkerTank.Update(gameTime);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {    // Loop through and draw each model   
            foreach (BasicModel bm in models){
                bm.Draw(((Game1)Game).camera);
            } 
           tank.Draw(((Game1)Game).camera.view, ((Game1)Game).camera.projection);
           stalkerTank.Draw(((Game1)Game).camera.view, ((Game1)Game).camera.projection);
           base.Draw(gameTime);
        } 
    }
}
