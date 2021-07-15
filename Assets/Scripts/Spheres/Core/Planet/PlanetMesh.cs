using System.Linq;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class PlanetMesh
    {
        private readonly Transform parent;
        private Vector3[] vertices;
        public int[] Triangles;
        
        public PlanetMesh(Transform parent)
        {
            this.parent = parent;
        }

        public void GenerateMesh(ComputeShader shader, float radius, GameObject spherePrefab)
        {
            
            var mainKernel = shader.FindKernel("CSMain");

            ComputeBuffer vertexBuffer = null;

            var size = 10;
            var length = (size + 1) * (size + 1) * (size + 1);
            
            shader.SetFloat("length", size + 1);

            ComputeHelper.CreateAndSetBuffer<Vector4>(ref vertexBuffer, length, shader, "vertices", mainKernel);
            ComputeHelper.Run(shader, vertexBuffer.count, kernelIndex: mainKernel);
                
            var vertexData = new Vector4[vertexBuffer.count];
            vertexBuffer.GetData (vertexData);
            ComputeHelper.Release(vertexBuffer);

            var max = vertexData.Max(v => v.z);
            var min = vertexData.Min(v => v.z);
            var container = new GameObject("pdsada");
            container.transform.SetParent(parent);
            foreach (var vertex in vertexData)
            {
                var sph = Object.Instantiate(spherePrefab, container.transform);
                sph.transform.position = new Vector3(vertex.x, vertex.y, vertex.z);
                var weight = vertex.w / (max - min);
                sph.GetComponent<MeshRenderer>().material.SetFloat("weight", weight);
            }
        }

        private Mesh GetMesh()
        {
            var meshFilter = parent.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                if (meshFilter.sharedMesh.vertices.Length != vertices.Length)
                {
                    meshFilter.sharedMesh = new Mesh();
                }
                return meshFilter.sharedMesh;
            }
            
            var gameObject = new GameObject("SphereMesh");
            gameObject.transform.SetParent(parent);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = new Mesh();

            return meshFilter.sharedMesh;
        }
    }
}