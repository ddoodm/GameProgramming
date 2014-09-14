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


namespace _11697822_lab06
{
    public class Ground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D[] textures;
        Model model;

        public Ground(Game game, Model model, Texture2D[] textures)
            : base(game)
        {
            this.textures = textures;
            this.model = model;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = ((Game1)Game).camera;

            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Wrap;
            ss.AddressV = TextureAddressMode.Wrap;
            Game.GraphicsDevice.SamplerStates[0] = ss;

            Matrix[] groundTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(groundTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = mesh.ParentBone.Transform * Matrix.Identity;
                    effect.Projection = camera.projection;
                    effect.View = camera.view;
                    effect.TextureEnabled = true;
                    effect.Texture = textures[0];

                    //effect.FogEnabled = true;
                    //effect.FogColor = Vector3.Zero;
                    //effect.FogStart = 20;
                    //effect.FogEnd = 70;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
