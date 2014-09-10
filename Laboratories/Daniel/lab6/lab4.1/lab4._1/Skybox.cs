using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 
namespace lab4._1
{
    class Skybox
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        Texture2D[] textures;
        Game1 game;
        BasicEffect effect;
        public Skybox(Game1 game, String assetName)
        {
            this.game = game;
            effect = new BasicEffect(game.GraphicsDevice);
            model = LoadModel(assetName, out textures);

        }
        private Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = game.Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
        public void DrawSkybox()
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            game.GraphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            game.GraphicsDevice.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(game.camera.position);// *Matrix.CreateTranslation(0, -game.camera.position.Y / 2, 0);
                    effect.Projection = game.camera.projection;
                    effect.View = game.camera.view;
                    effect.TextureEnabled = true;
                    effect.Texture = textures[i++];
                    //effect.FogEnabled = true;
                    //effect.FogEnabled 

                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            game.GraphicsDevice.DepthStencilState = dss;
        }
    }
}
