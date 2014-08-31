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


namespace Lab5
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Skybox : Microsoft.Xna.Framework.GameComponent
    {

        Texture2D[] textures;
        Model model;



        public Skybox(Game game, Model model, Texture2D[] textures)
            : base(game)
        {

            this.textures = textures;
            this.model = model;

            // TODO: Construct any child components here
        }


        public void Draw(GraphicsDevice device, Camera camera)
        {
            //clamps the texture which removes aretefacts between primitives
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            //enable the skybox to look infinitly far away
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //this allows the skybox to follow the player giving the appearance of it being static
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(camera.cameraPosition);
                    effect.Projection = camera.projection;
                    effect.View = camera.view;
                    effect.TextureEnabled = true;
                    effect.Texture = textures[i++];
                }
                mesh.Draw();
            }

            //re-enable the depth buffer for other objects being drawn after the skybox
            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;

        }
    }
}
