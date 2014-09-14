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
    public class Skybox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D[] textures;
        Model model;

        public Skybox(Game game, Model model, Texture2D[] textures)
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

        public void Draw(GraphicsDevice device, Camera camera)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);

            int i = 0;
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(2f)
                        * skyboxTransforms[mesh.ParentBone.Index]
                        * Matrix.CreateTranslation(camera.position);
                    effect.Projection = camera.projection;
                    effect.View = camera.view;
                    effect.TextureEnabled = true;
                    effect.Texture = textures[i++];

                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = 20;
                    effect.FogEnd = 70;
                }
                
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }
    }
}

//translate to negative of camera pos
