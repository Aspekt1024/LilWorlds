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

        public void GenerateMesh(ComputeShader shader)
        {
            
            var mainKernel = shader.FindKernel("CSMain");

            ComputeBuffer vertexBuffer = null;

            ComputeHelper.CreateAndSetBuffer<float>(ref vertexBuffer, vertexBuffer.count, shader, "heights", mainKernel);
            ComputeHelper.Run(shader, vertexBuffer.count, kernelIndex: mainKernel);
                
            var heights = new float[vertexBuffer.count];
            heightBuffer.GetData (heights);
            ComputeHelper.Release(heightBuffer);
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