using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab10
{
    /// <summary>
    /// Composed with comprehensive direction from:
    /// Riemer Grootjans' XNA 3.0 Game Programming Recipes
    /// 
    /// I have attempted to comment the code well to
    /// demonstrate my understanding where possible.
    /// </summary>
    public class Terrain
    {
        private Game game;
        private Vector3 size;
        private VertexPositionNormalTexture[] verts;
        private short[] indices;
        private float[,] heights;
        private VertexDeclaration vertexDeclaration;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private BasicEffect shader;
        private Texture2D texture;

        private Vector2 TEXTURE_TILE = new Vector2(10, 10);

        public Terrain(Game game, float[,] heights, Vector3 size)
        {
            this.game = game;
            this.heights = heights;
            this.size = size;

            texture = game.Content.Load<Texture2D>("Grass0139_33_S");

            initialize();
        }

        public Terrain(Game game, Texture2D heightMap, Vector3 size)
        {
            this.game = game;
            this.heights = createHeightArray(heightMap);
            this.size = size;

            texture = game.Content.Load<Texture2D>("Grass0139_33_S");

            initialize();
        }

        private void initialize()
        {
            // Create the triangle strip structure that defines the terrain
            verts = createVerts();
            indices = createIndices();

            // Find the vertex normals of the terrain
            verts = NormalComputer.computeTriangleStrip(verts, indices);

            vertexDeclaration = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
            createBuffers(verts, indices);

            shader = new BasicEffect(game.GraphicsDevice);
        }

        private float[,] createHeightArray(Texture2D heightMap)
        {
            // Create a new 2D float array at the size of the height map
            float[,] heights = new float[heightMap.Height, heightMap.Width];

            // Store texture data in a colour array
            Color[] colors = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(colors);

            // Copy red values into the float array
            for(int y=0;y<heightMap.Height;y++)
                for(int x=0;x<heightMap.Width;x++)
                    heights[x, y] = colors[y * heightMap.Width + x].R;

            return heights;
        }

        private VertexPositionNormalTexture[] createVerts()
        {
            int
                height = heights.GetLength(0),
                width = heights.GetLength(1);

            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[width * height];

            int i = 0;
            for(int z=0; z<height; z++)
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = new Vector3(x - width/2, heights[x, z], z - height/2);
                    Vector3 normal = Vector3.Up;
                    Vector2 texCoord = new Vector2(
                        z / (float)height * TEXTURE_TILE.X,
                        x / (float)width * TEXTURE_TILE.Y);

                    // Resize the terrain
                    position *= size;

                    verts[i++] = new VertexPositionNormalTexture(position, normal, texCoord);
                }

            return verts;
        }

        private short[] createIndices()
        {
            int
                height = heights.GetLength(0),
                width = heights.GetLength(1);

            short[] indices = new short[width * 2 * (height - 1)];

            // A loop that iterates through each column, moves to the next row, and creates a ghost triangle
            int i=0, z=0;
            while (z < height-1)
            {
                for (int x = 0; x < width; x++)
                {
                    indices[i++] = (short)(x + z * width);
                    indices[i++] = (short)(x + (z + 1) * width);
                }
                z++;

                if (z < height - 1)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        indices[i++] = (short)(x + (z + 1) * width);
                        indices[i++] = (short)(x + z * width);
                    }
                }
                z++;
            }

            return indices;
        }

        private void createBuffers(VertexPositionNormalTexture[] verts, short[] indices)
        {
            vertexBuffer = new VertexBuffer(
                game.GraphicsDevice,                            // Game's graphics device
                VertexPositionNormalTexture.VertexDeclaration,  // The vertex format is now specified as a declaration
                verts.Length,                                   // Length is now in vertices, not bytes
                BufferUsage.WriteOnly);
            vertexBuffer.SetData(verts);

            indexBuffer = new IndexBuffer(
                game.GraphicsDevice,
                typeof(short),
                indices.Length,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        /// <summary>
        /// Get the Y value (bilinearly interpolated height) at the specified coordinates.
        /// </summary>
        /// <param name="coord">The coordinates (X,Z) to solve</param>
        /// <returns>The Y value at the coordinate supplied</returns>
        public float getYAt(Vector2 coord)
        {
            return this.getYAt(new Vector3(coord.X, 0, coord.Y));
        }

        /// <summary>
        /// Get the Y value (bilinearly interpolated height) at the specified coordinates.
        /// </summary>
        /// <param name="coord">The coordinates (X,0,Z) to solve</param>
        /// <returns>The Y value at the coordinate supplied</returns>
        public float getYAt(Vector3 coord)
        {
            // Map the coordinate to the terrain size
            Vector3 resizedCoord = coord / size;
            resizedCoord = new Vector3(resizedCoord.X + heights.GetLength(1) / 2, resizedCoord.Y, resizedCoord.Z + heights.GetLength(0) / 2);

            // The truncated (and unified) 2D coordinates.
            int
                truncX = (int)(resizedCoord.X),
                truncZ = (int)(resizedCoord.Z);

            // The next vertex in the grid (so that we can interpolate between them)
            int
                nextX = truncX + 1,
                nextZ = truncZ + 1;

            // Check that the coordinate is within the computable space
            if (truncX < 0 || truncZ < 0 || nextX < 0 || nextZ < 0)
                return 0f;
            if (truncX >= heights.GetLength(0) ||
                truncZ >= heights.GetLength(1) ||
                nextX >= heights.GetLength(0) ||
                nextZ >= heights.GetLength(1))
                return 0f;

            // The coordinate interpolated across its nearest triangle
            float
                lerpX = (float)(resizedCoord.X - truncX) / (float)(nextX - truncX),
                lerpZ = (float)(resizedCoord.Z - truncZ) / (float)(nextZ - truncZ);

            // Find the heights of the four vertices that compose the quad surrounding the point (coord)
            float
                LL = heights[truncX, truncZ],        // Low-left
                LH = heights[truncX, nextZ],         // Low-right
                HL = heights[nextX, truncZ],         // High-left
                HH = heights[nextX, nextZ];          // High-right

            // As explained by Grootjans,
            // when the sum of the interpolated (and normalized) coordinates yields a value smaller
            // than 1, the vertex is in the lower-left triangle.
            bool coordInUpperTri = (lerpX + lerpZ) < 1.0f;

            // Perform bilinear interpolation
            float finalHeight;
            if (coordInUpperTri)
            {
                // Start at lower-left vertex height
                finalHeight = LL;

                // Lerp between the low vertex and opposing high vertex
                finalHeight += lerpZ * (LH - LL);
                finalHeight += lerpX * (HL - LL);
            }
            else
            {
                // Start and upper-right vertex height
                finalHeight = HH;

                // Subtract down towards the opposing vertex
                finalHeight += (1f-lerpZ) * (HL - HH);
                finalHeight += (1f-lerpX) * (LH - HH);
            }

            // Scale by size scalar
            return finalHeight * size.Y;
        }

        /// <summary>
        /// Get the coordinates of the intersection of the ray and the terrain surface
        /// </summary>
        /// <param name="ray">The ray with which to check collisions</param>
        /// <returns>The coordinates of the intersection</returns>
        public Vector3 getRayCollision(Ray ray)
        {
            // The height accuracy threshold
            const float threshold = 0.1f;

            // The terrain height at the ray's start point
            float startHeight = getYAt(ray.Position);

            // The current difference in height
            float heightDelta = ray.Position.Y - startHeight;

            // Loop until the approximation is good enough
            int loopDepth = 0;
            while (heightDelta > threshold)
            {
                // Half the ray direction
                ray.Direction *= 0.5f;
                
                // The new mid-point between the two ends of the ray
                Vector3 midPoint = ray.Position + ray.Direction;

                // The terrain height at the mid-point
                float heightAtMidPoint = getYAt(midPoint);

                // If the mid-point is still above the terrain, send for more refinement
                if (midPoint.Y > heightAtMidPoint)
                {
                    ray.Position = midPoint;
                    heightDelta = midPoint.Y - heightAtMidPoint;
                }

                if (loopDepth++ >= 32)
                    break;
            }

            return ray.Position;
        }

        public void update(UpdateParams updateParams)
        {

        }

        public void draw(DrawParams drawParams)
        {
            int width = heights.GetLength(1), height = heights.GetLength(0);

            shader.World = Matrix.Identity;
            shader.View = drawParams.camera.view;
            shader.Projection = drawParams.camera.projection;

            shader.Texture = texture;
            shader.TextureEnabled = true;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            shader.EnableDefaultLighting();
            shader.AmbientLightColor = new Vector3(0.5f);
            shader.DirectionalLight0.DiffuseColor = new Vector3(0.95f, 0.87f, 0.80f);
            shader.DirectionalLight0.Enabled = false;
            shader.DirectionalLight2.Enabled = false;

            //shader.FogEnabled = true;
            shader.FogStart = 550;
            shader.FogEnd = 700f;
            shader.FogColor = new Vector3(0.70f, 0.67f, 0.63f);

            foreach(EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                game.GraphicsDevice.Indices = indexBuffer;

                game.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    width,
                    height,
                    width * 2 * (height - 1) - 2);
            }
        }

        public BasicEffect getEffect()
        {
            return shader;
        }
    }
}
