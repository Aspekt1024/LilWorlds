using UnityEngine;

namespace Aspekt.Universe.Planets
{
    public class PlanetChunk
    {
        private GameObject chunk;
        private Mesh mesh;

        private PlanetChunk(PlanetDensitySettings densitySettings, Vector3 offset, Transform parent)
        {
            // chunk = new GameObject("PlanetChunk");
            // var mf = chunk.AddComponent<MeshFilter>();
            // var mr = chunk.AddComponent<MeshRenderer>();

            var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);

            prim.name = "PlanetChunk";
            
            var tf = prim.transform;
            tf.SetParent(parent);
            prim.transform.localPosition = offset;
            prim.transform.localScale = Vector3.one * (PlanetGenerator.ChunkLengthInVoxels * densitySettings.spacing);
            
            chunk = prim;
        }

        public static PlanetChunk Generate(PlanetDensitySettings densitySettings, Vector3 offset, Transform parent)
        {
            return new PlanetChunk(densitySettings, offset, parent);
        }
        
        // TODO regenerate chunk function
        
        public void Destroy()
        {
            if (Application.isPlaying)
            {
                Object.Destroy(chunk);
            }
            else
            {
                Object.DestroyImmediate(chunk);
            }
        }
        
    }
}