using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lab5
{
    class Ground : Microsoft.Xna.Framework.GameComponent
    {
        //variables for the ground tiles vertexbuffer to hold the vertices of all the tiles
        VertexBuffer groundVertexBuffer;
        BasicEffect effect;


        public Ground(Game game)
            : base(game)
        {

        }


        private void SetUpVertices(GraphicsDevice device)
        {
            //width and length of the ground
            int areaWidth = 100;
            int areaLength = 100;

            int centerXOffSet = areaWidth / 2;
            int centerZOffSet = areaLength / 2;

            //A list to hold the vertices of the ground tiles
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

            for (int x = -centerXOffSet; x < areaWidth - centerXOffSet; x++)
            {
                for (int z = -centerZOffSet; z < areaLength - centerZOffSet; z++)
                {
                    //Verticesof the two triangles in each tile on the ground each entry contains the position, normal and texture coordinate of a vertex
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 1, 0), new Vector2(1, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));

                }
            }

            //initialize the vertex buffer
            groundVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            //set the vertex buffer with the vertices of all the primitives
            groundVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }



        public void Initialize(GraphicsDevice device, BasicEffect basic)
        {
            SetUpVertices(device);
            effect = basic;

        }

        public void Draw(GraphicsDevice device, Camera camera, Texture2D sceneryTexture)
        {
            //scale the ground tiles to cover a larger area and enable and set the texture ont the tiles
            effect.World = Matrix.CreateScale(25f, 1f, 25f);
            effect.Projection = camera.projection;
            effect.View = camera.view;
            effect.TextureEnabled = true;
            effect.Texture = sceneryTexture;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(groundVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, groundVertexBuffer.VertexCount / 3);
            }
        }
    }
}
