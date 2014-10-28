using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    /// <summary>
    /// This class contains (slightly modified) code from:
    /// http://timjones.tw/blog/archive/2010/12/10/drawing-an-xna-model-bounding-box
    /// Which describes how to create a Bounding Box from a Model,
    /// and how to draw the Bounding Box wireframe around the model.
    /// </summary>
    public class BoundingBoxUtilities
    {
        private static Vector3[] getVertexData(ModelMeshPart meshPart, VertexElementUsage usage)
        {
            VertexDeclaration vd = meshPart.VertexBuffer.VertexDeclaration;
            VertexElement[] elements = vd.GetVertexElements();

            Func<VertexElement, bool> elementPredicate = ve => ve.VertexElementUsage == usage && ve.VertexElementFormat == VertexElementFormat.Vector3;
            if (!elements.Any(elementPredicate))
                return null;

            VertexElement element = elements.First(elementPredicate);

            Vector3[] vertexData = new Vector3[meshPart.NumVertices];
            meshPart.VertexBuffer.GetData((meshPart.VertexOffset * vd.VertexStride) + element.Offset,
                vertexData, 0, vertexData.Length, vd.VertexStride);

            return vertexData;
        }

        /// <summary>
        /// Creates a Bounding Box from a ModelMeshPart, and applies a transform matrix to it.
        /// </summary>
        /// <param name="meshPart">The mesh part that requires a Bounding Box</param>
        /// <param name="transform">The transform to apply to the mesh (world matrix)</param>
        /// <returns>The bounding box that surrounds the meshPart</returns>
        public static BoundingBox? createBoundingBoxPart(ModelMeshPart meshPart, Matrix transform)
        {
            if (meshPart.VertexBuffer == null)
                return null;

            Vector3[] positions = getVertexData(meshPart, VertexElementUsage.Position);
            if (positions == null)
                return null;

            Vector3[] transformedPositions = new Vector3[positions.Length];
            Vector3.Transform(positions, ref transform, transformedPositions);

            return BoundingBox.CreateFromPoints(transformedPositions);
        }

        /// <summary>
        /// Create a composite BoundingBox from an entire model (with multiple meshes)
        /// </summary>
        /// <param name="model">The model to contain within a Bounding Box</param>
        /// <param name="transform">The matrix by which to transform the Bounding Box</param>
        /// <returns>The Bounding Box that surrounds the model</returns>
        public static BoundingBox createBoundingBox(Model model, Matrix transform)
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingBox result = new BoundingBox();
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BoundingBox? meshPartBoundingBox = createBoundingBoxPart(meshPart, boneTransforms[mesh.ParentBone.Index]);
                    if (meshPartBoundingBox != null)
                        result = BoundingBox.CreateMerged(result, meshPartBoundingBox.Value);
                }

            // == Modified: Transform bounding box by the supplied World transform matrix ==
            Vector3 minTransformed = Vector3.Transform(result.Min, transform);
            Vector3 maxTransformed = Vector3.Transform(result.Max, transform);
            result = new BoundingBox(minTransformed, maxTransformed);

            return result;
        }
    }

    /// <summary>
    /// This class contains code from:
    /// http://timjones.tw/blog/archive/2010/12/10/drawing-an-xna-model-bounding-box
    /// Which describes how to create a Bounding Box from a Model,
    /// and how to draw the Bounding Box wireframe around the model.
    /// </summary>
    public class DrawableBoundingBox
    {
        private class BoundingBoxBuffers
        {
            public VertexBuffer Vertices;
            public int VertexCount;
            public IndexBuffer Indices;
            public int PrimitiveCount;
        }

        private BoundingBoxBuffers boxBuffer;
        private BasicEffect effect;
        private Color color;

        public DrawableBoundingBox(BoundingBox boundingBox, GraphicsDevice graphicsDevice, Color color)
        {
            this.color = color;

            boxBuffer = CreateBoundingBoxBuffers(boundingBox, graphicsDevice);

            effect = new BasicEffect(graphicsDevice);
            effect.LightingEnabled = false;
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
        }

        private BoundingBoxBuffers CreateBoundingBoxBuffers(BoundingBox boundingBox, GraphicsDevice graphicsDevice)
        {
            BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();

            boundingBoxBuffers.PrimitiveCount = 24;
            boundingBoxBuffers.VertexCount = 48;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
                typeof(VertexPositionColor), boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            const float ratio = 5.0f;

            Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
            Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
            Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
            Vector3[] corners = boundingBox.GetCorners();

            // Corner 1.
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] + xOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - yOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - zOffset);

            // Corner 2.
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - xOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - yOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - zOffset);

            // Corner 3.
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - xOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] + yOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - zOffset);

            // Corner 4.
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + xOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + yOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] - zOffset);

            // Corner 5.
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + xOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] - yOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + zOffset);

            // Corner 6.
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - xOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - yOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] + zOffset);

            // Corner 7.
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] - xOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + yOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + zOffset);

            // Corner 8.
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + xOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + yOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + zOffset);

            vertexBuffer.SetData(vertices.ToArray());
            boundingBoxBuffers.Vertices = vertexBuffer;

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(Enumerable.Range(0, boundingBoxBuffers.VertexCount).Select(i => (short)i).ToArray());
            boundingBoxBuffers.Indices = indexBuffer;

            return boundingBoxBuffers;
        }

        private void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
        {
            vertices.Add(new VertexPositionColor(position, color));
        }

        public void draw(DrawParams drawParams)
        {
            drawParams.graphicsDevice.SetVertexBuffer(boxBuffer.Vertices);
            drawParams.graphicsDevice.Indices = boxBuffer.Indices;

            effect.World = Matrix.Identity;
            effect.View = drawParams.camera.view;
            effect.Projection = drawParams.camera.projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                drawParams.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    boxBuffer.VertexCount, 0, boxBuffer.PrimitiveCount);
            }
        }
    }
}
