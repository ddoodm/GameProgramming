using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class Tower : Entity
    {
        private float size;
        protected StaticModel model;
        protected PlaneEntity shadowTex;

        // Path-finding weight
        public int gWeight = 0;

        public Tower(Game game, Matrix world, float size)
            : base(game, world)
        {
            this.size = size;

            init_shadowPlane();
        }

        /// <summary>
        /// A silly little way to draw a shadow volume below the model.
        /// </summary>
        private void init_shadowPlane()
        {
            PlanePrimitive plane = new PlanePrimitive(game, size*1.5f, Vector3.Up);
            shadowTex = new PlaneEntity(game, plane, world.Translation + Vector3.Up, 0);
            shadowTex.primitive.texture = game.Content.Load<Texture2D>("Textures\\shadowA");
        }

        public virtual void update(UpdateParams updateParams)
        {
            shadowTex.update(updateParams);
        }

        protected virtual void drawShadow(DrawParams drawParams)
        {
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            shadowTex.draw(drawParams);
        }

        public virtual void draw(DrawParams drawParams)
        {

        }
    }

    public class WallTower : Tower
    {
        public int gWeight = 0;

        public WallTower(Game game, Matrix world, float size)
            : base(game, world, size)
        {
            loadModel();
        }

        /// <summary>
        /// Loads the standard "Wall" tower model
        /// </summary>
        private void loadModel()
        {
            model = new StaticModel(game, game.Content.Load<Model>("Models\\tower_wall"), world);
        }

        public override void update(UpdateParams updateParams)
        {
            base.update(updateParams);

            model.update(updateParams);
        }

        public override void draw(DrawParams drawParams)
        {
            base.draw(drawParams);

            drawShadow(drawParams);
            model.draw(drawParams);
        }
    }
}
