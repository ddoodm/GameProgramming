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
    public class Ground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        int size = 50;
        int scale = 10;
        // Vertex data
        VertexPositionTexture[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;

        // Movement and rotation stuff
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;
        Matrix worldScale = Matrix.Identity;

        // Texture info
        Texture2D texture;
        Camera camera;
        Game game;
        public Ground(Game game, Camera camera)
            : base(game)
        {
            this.camera = camera;
            this.game = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            worldScale = Matrix.CreateScale(scale/2);
            //worldTranslation = Matrix.CreateTranslation(new Vector3(0, 2, 0));
            //worldRotation = Matrix.CreateFromYawPitchRoll(0,-MathHelper.PiOver2,0);
            // Initialize vertices
            verts = new VertexPositionTexture[6 * size * size];
            int count = 0;
            Vector3 topleft = new Vector3(-size/2*scale, 0, -size/2*scale);
            Vector3 topright = new Vector3(-size/2 * scale + scale, 0, -size/2 * scale);
            Vector3 backright = new Vector3(-size/2 * scale + scale, 0, -size/2 * scale + scale);
            Vector3 backleft = new Vector3(-size/2 * scale, 0, -size/2 * scale + scale);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    verts[count++] = new VertexPositionTexture(
                        topleft, new Vector2(0, 0));
                    verts[count++] = new VertexPositionTexture(
                        topright, new Vector2(1, 0));
                    verts[count++] = new VertexPositionTexture(
                        backleft, new Vector2(0, 1));
                    verts[count++] = new VertexPositionTexture(
                        topright, new Vector2(1, 0));
                    verts[count++] = new VertexPositionTexture(
                        backright, new Vector2(1, 1));
                    verts[count++] = new VertexPositionTexture(
                        backleft, new Vector2(0, 1));
                    topleft.Z += scale;
                    topright.Z += scale;
                    backright.Z += scale;
                    backleft.Z += scale;
                }
                topleft.X += scale;
                topright.X += scale;
                backright.X += scale;
                backleft.X += scale;
                topleft.Z = -size / 2 * scale;
                topright.Z = -size / 2 * scale;
                backright.Z = -size / 2 * scale + scale;
                backleft.Z = -size / 2 * scale + scale;
            }


            // Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            // Initialize the BasicEffect
            effect = new BasicEffect(game.GraphicsDevice);

            // Load texture
            texture = Game.Content.Load<Texture2D>(@"Textures\Ak4c");
           
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
                                game.GraphicsDevice.SetVertexBuffer(vertexBuffer);

                    //Set object and camera info
                    effect.World = worldScale * worldRotation * worldTranslation;
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.Texture = texture;
                    effect.TextureEnabled = true;

                    // Begin effect and draw for each pass
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                            (PrimitiveType.TriangleList, verts, 0, 2*size*size);

                    }
          
            base.Draw(gameTime);
        }
    }
}
