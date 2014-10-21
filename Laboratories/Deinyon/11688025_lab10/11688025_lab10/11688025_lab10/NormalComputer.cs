using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab10
{
    /// <summary>
    /// Computes vertex normals for an indexed triangle list or strip structure.
    /// </summary>
    class NormalComputer
    {
        /// <summary>
        /// Compute vertex normals for all triangles defined in an indexed triangle list.
        /// </summary>
        /// <param name="vertices">The coordinates of each vertex</param>
        /// <param name="indices">The order in which the vertices should be structured</param>
        /// <returns>The same vertex structure with the vertex normals computed</returns>
        public static VertexPositionNormalTexture[] computeTriangleList(VertexPositionNormalTexture[] vertices, short[] indices)
        {
            // Reset all vertex normals
            for(int i=0; i<vertices.Length; i++)
                vertices[i].Normal = Vector3.Zero;

            // Each triangle in a list is defined by three consecutive vertices,
            // therefore, the number of triangles is indices.Length / 3
            int numTris = indices.Length / 3;

            // For each triangle
            for (int i = 0; i < numTris; i++ )
            {
                // Find vector deltas between the edges of the triangle
                Vector3 d1 =
                    vertices[indices[i * 3 + 1]].Position - vertices[indices[i * 3]].Position;
                Vector3 d2 =
                    vertices[indices[i * 3 + 2]].Position - vertices[indices[i * 3]].Position;

                // Compute the orthogonal vector to the plane defined by the two deltas
                Vector3 normal = Vector3.Normalize(Vector3.Cross(d2, d1));

                // Add this normal contribution to the existing normals of this triangle
                if (!float.IsNaN(normal.X))
                    for (int j=0; j<3; j++)
                        vertices[indices[i*3 + j]].Normal += normal;
            }

            // Normalize all of the combined contributions
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        /// <summary>
        /// Compute vertex normals for all triangles defined in an indexed triangle strip.
        /// </summary>
        /// <param name="vertices">The coordinates of each vertex</param>
        /// <param name="indices">The order in which the vertices should be structured</param>
        /// <returns>The same vertex structure with the vertex normals computed</returns>
        public static VertexPositionNormalTexture[] computeTriangleStrip(VertexPositionNormalTexture[] vertices, short[] indices)
        {
            // Reset all vertex normals
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = Vector3.Zero;

            bool windingSwapped = false;

            // For triangle strips, the loop starts at vertex three,
            // because each triangle is defined by the three prior
            // vertices. Therefore, the number of triangles is:
            // indices.Length - 2
            for (int i = 2; i < indices.Length; i++)
            {
                // Find vector deltas between the edges of the triangle
                Vector3 d1 =
                    vertices[indices[i - 1]].Position - vertices[indices[i]].Position;
                Vector3 d2 =
                    vertices[indices[i - 2]].Position - vertices[indices[i]].Position;

                // Compute the orthogonal vector to the plane defined by the two deltas
                Vector3 normal = Vector3.Normalize(Vector3.Cross(d1, d2));

                // Triangle strips invert the winding order for each triangle, so we need to compensate
                if (windingSwapped)
                    normal *= -1;

                // Add this normal contribution to the existing normals of this triangle
                if(!float.IsNaN(normal.X))
                    for (int j = 0; j < 3; j++)
                        vertices[indices[i - j]].Normal += normal;

                windingSwapped = !windingSwapped;
            }

            // Normalize all of the combined contributions
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }
    }
}
