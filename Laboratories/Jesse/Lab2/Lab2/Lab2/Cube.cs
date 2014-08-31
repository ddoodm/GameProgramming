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


//code to create a cube
namespace Lab2
{
    class Cube
    {
        public Vector3 cubeSize;
        private VertexPositionNormalTexture[] cubeVertices;
        private int cubeTriangles;
        private VertexBuffer cubeBuffer;
        public Texture2D cubeTexture;


        public Cube(Vector3 size, Vector3 position)
        {
            cubeSize = size;

        }


        private void BuildCube()
        {
            cubeTriangles = 12;

            cubeVertices = new VertexPositionNormalTexture[36];

            //Front face vertices
            Vector3 frontTL = new Vector3(-1.0f, 1.0f, -1.0f) * cubeSize;
            Vector3 frontBL = new Vector3(-1.0f, -1.0f, -1.0f) * cubeSize;
            Vector3 frontBR = new Vector3(1.0f, -1.0f, -1.0f) * cubeSize;
            Vector3 frontTR = new Vector3(1.0f, 1.0f, -1.0f) * cubeSize;

            //Back face vertices
            Vector3 backTL = new Vector3(-1.0f, 1.0f, 1.0f) * cubeSize;
            Vector3 backBL = new Vector3(-1.0f, -1.0f, 1.0f) * cubeSize;
            Vector3 backBR = new Vector3(1.0f, -1.0f, 1.0f) * cubeSize;
            Vector3 backTR = new Vector3(1.0f, 1.0f, 1.0f) * cubeSize;

            //face normals
            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f) * cubeSize;
            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * cubeSize;
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * cubeSize;
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * cubeSize;
            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * cubeSize;
            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * cubeSize;

            //Texture coordinates
            Vector2 textureFrontTL = new Vector2(0.0f, 0.0f);
            Vector2 textureFrontBL = new Vector2(0.0f, 0.5f);
            Vector2 textureFrontBR = new Vector2(0.5f, 0.5f);
            Vector2 textureFrontTR = new Vector2(0.5f, 0.0f);

            Vector2 textureBackTL = new Vector2(0.0f, 0.5f);
            Vector2 textureBackBL = new Vector2(0.0f, 0.0f);
            Vector2 textureBackBR = new Vector2(0.5f, 0.0f);
            Vector2 textureBackTR = new Vector2(0.5f, 0.5f);

            Vector2 textureTopTL = new Vector2(0.5f, 1.0f);
            Vector2 textureTopBL = new Vector2(0.5f, 0.5f);
            Vector2 textureTopBR = new Vector2(1.0f, 0.5f);
            Vector2 textureTopTR = new Vector2(1.0f, 1.0f);

            Vector2 textureBottomTL = new Vector2(0.5f, 0.5f);
            Vector2 textureBottomBL = new Vector2(0.5f, 0.0f);
            Vector2 textureBottomBR = new Vector2(1.0f, 0.0f);
            Vector2 textureBottomTR = new Vector2(1.0f, 0.5f);


            //Construction of cube using vertices, normals and texture coordinates
            //Front face
            cubeVertices[0] = new VertexPositionNormalTexture(frontTL, normalFront, textureFrontTL);
            cubeVertices[1] = new VertexPositionNormalTexture(frontBL, normalFront, textureFrontBL);
            cubeVertices[2] = new VertexPositionNormalTexture(frontTR, normalFront, textureFrontTR);
            cubeVertices[3] = new VertexPositionNormalTexture(frontBL, normalFront, textureFrontBL);
            cubeVertices[4] = new VertexPositionNormalTexture(frontBR, normalFront, textureFrontBR);
            cubeVertices[5] = new VertexPositionNormalTexture(frontTR, normalFront, textureFrontTR);

            //back face
            cubeVertices[6] = new VertexPositionNormalTexture(backTL, normalBack, textureBackTR);
            cubeVertices[7] = new VertexPositionNormalTexture(backTR, normalBack, textureBackTL);
            cubeVertices[8] = new VertexPositionNormalTexture(backBL, normalBack, textureBackBR);
            cubeVertices[9] = new VertexPositionNormalTexture(backBL, normalBack, textureBackBR);
            cubeVertices[10] = new VertexPositionNormalTexture(backTR, normalBack, textureBackTL);
            cubeVertices[11] = new VertexPositionNormalTexture(backBR, normalBack, textureBackBL);
 
            //top face
            cubeVertices[12] = new VertexPositionNormalTexture(frontTL, normalTop, textureTopBL);
            cubeVertices[13] = new VertexPositionNormalTexture(backTR, normalTop, textureTopTR);
            cubeVertices[14] = new VertexPositionNormalTexture(backTL, normalTop, textureTopTL);
            cubeVertices[15] = new VertexPositionNormalTexture(frontTL, normalTop, textureTopBL);
            cubeVertices[16] = new VertexPositionNormalTexture(frontTR, normalTop, textureTopBR);
            cubeVertices[17] = new VertexPositionNormalTexture(backTR, normalTop, textureTopTR);
 
            //bottom face 
            cubeVertices[18] = new VertexPositionNormalTexture(frontBL, normalBottom, textureBottomTL);
            cubeVertices[19] = new VertexPositionNormalTexture(backBL, normalBottom, textureBottomBL);
            cubeVertices[20] = new VertexPositionNormalTexture(backBR, normalBottom, textureBottomBR);
            cubeVertices[21] = new VertexPositionNormalTexture(frontBL, normalBottom, textureBottomTL);
            cubeVertices[22] = new VertexPositionNormalTexture(backBR, normalBottom, textureBottomBR);
            cubeVertices[23] = new VertexPositionNormalTexture(frontBR, normalBottom, textureBottomTR);

            // right face
            cubeVertices[30] = new VertexPositionNormalTexture(frontTR, normalRight, textureTopTL);
            cubeVertices[31] = new VertexPositionNormalTexture(frontBR, normalRight, textureTopBL);
            cubeVertices[32] = new VertexPositionNormalTexture(backBR, normalRight, textureTopBR);
            cubeVertices[33] = new VertexPositionNormalTexture(backTR, normalRight, textureTopTR);
            cubeVertices[34] = new VertexPositionNormalTexture(frontTR, normalRight, textureTopTL);
            cubeVertices[35] = new VertexPositionNormalTexture(backBR, normalRight, textureTopBR);

            //left face
            cubeVertices[24] = new VertexPositionNormalTexture(frontTL, normalLeft, textureBottomTR);
            cubeVertices[25] = new VertexPositionNormalTexture(backBL, normalLeft, textureBottomBL);
            cubeVertices[26] = new VertexPositionNormalTexture(frontBL, normalLeft, textureBottomBR);
            cubeVertices[27] = new VertexPositionNormalTexture(backTL, normalLeft, textureBottomTL);
            cubeVertices[28] = new VertexPositionNormalTexture(backBL, normalLeft, textureBottomBL);
            cubeVertices[29] = new VertexPositionNormalTexture(frontTL, normalLeft, textureBottomTR);
 
        }


        public void RenderCube(GraphicsDevice device)
        {
            //build the cube using the verticies, normals and texture coordinates
            BuildCube();

            //create a shape buffer to contain the verticies
            cubeBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, 36, BufferUsage.WriteOnly);

            //put the verticies into the buffer just created
            cubeBuffer.SetData(cubeVertices);

            //send the vertex buffer to the device
            device.SetVertexBuffer(cubeBuffer);

            //draw the buffer triangles to the device
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, cubeTriangles);

        }

    }
}
