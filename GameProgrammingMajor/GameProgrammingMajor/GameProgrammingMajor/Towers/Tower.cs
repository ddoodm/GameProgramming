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
        protected BoundingBox boundingBox;
        protected DrawableBoundingBox drawableBoundingBox;
        protected PlaneEntity shadowTex;

        public bool shadowEnabled = false;

        // Path-finding weight
        private int gWeight = 0;

        public Tower(Game game, Matrix world, float size)
            : base(game, world)
        {
            this.size = size;

            init_shadowPlane();
        }

        public Tower(Tower rhs)
            : this(rhs.game, rhs.world, rhs.size)
        {

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

        public virtual int getGWeight()
        {
            return gWeight;
        }

        /// <summary>
        /// Is it impossible to pathfind through this tower?
        /// </summary>
        public virtual bool isSolid()
        {
            return false;
        }

        public virtual void update(UpdateParams updateParams)
        {
            shadowTex.update(updateParams);
        }

        protected virtual void drawShadow(DrawParams drawParams)
        {
            if (!shadowEnabled)
                return;

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            shadowTex.draw(drawParams);
        }

        public virtual void draw(DrawParams drawParams)
        {

        }

        public bool collidesWith(BoundingSphere sphere)
        {
            if (boundingBox == null || sphere == null)
                return false;

            return boundingBox.Intersects(sphere);
        }

        public bool collidesWith(BoundingBox box)
        {
            if (boundingBox == null || box == null)
                return false;

            return boundingBox.Intersects(box);
        }

        public bool collidesWith(Ray ray)
        {
            if (boundingBox == null || ray == null)
                return false;

            return boundingBox.Intersects(ray).HasValue;
        }
    }

    public class WallTower : Tower
    {
        private int gWeight = 0;

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

            // Create a Bounding Box from the model using our utility class
            boundingBox = BoundingBoxUtilities.createBoundingBox(model.model, world);

            // Create a drawable Bounding Box from the Bounding Box created above
            drawableBoundingBox = new DrawableBoundingBox(boundingBox, game.GraphicsDevice, Color.White);
        }

        public override int getGWeight()
        {
            return gWeight;
        }

        /// <summary>
        /// Is it impossible to pathfind through this tower?
        /// </summary>
        public override bool isSolid()
        {
            return true;
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
            drawableBoundingBox.draw(drawParams);
        }
    }
}
