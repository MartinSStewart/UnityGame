using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class SimpleMesh
    {
        public int[] Triangles;
        public Vector3[] Vertices;

        public SimpleMesh()
        {
        }

        public SimpleMesh(Mesh mesh)
        {
            Triangles = mesh.triangles.ToArray();
            Vertices = mesh.vertices.ToArray();
        }

        public void Translate(Vector3 v)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] += v;
            }
        }

        public void Scale(float scale)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] *= scale;
            }
        }
    }
}
