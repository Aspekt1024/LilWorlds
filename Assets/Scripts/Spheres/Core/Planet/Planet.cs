using UnityEngine;

namespace Aspekt.Spheres
{
    public class Planet : MonoBehaviour
    {
        public ComputeShader generator;
        public float radius;
        public GameObject spherePrefab;

        private GameObject planet;

        [ContextMenu("Generate")]
        public void Generate()
        {
            var planetMesh = new PlanetMesh(transform);
            planetMesh.GenerateMesh(generator, radius, spherePrefab);
        }
    }
}