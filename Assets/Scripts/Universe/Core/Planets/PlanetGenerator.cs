using UnityEngine;

namespace Aspekt.Universe.Planets
{
    public class PlanetGenerator
    {
        private PlanetChunk[] chunks;
        
        public const int ChunkLengthInVoxels = 32;
        
        public Transform Generate(PlanetShapeSettings shapeSettings, PlanetDensitySettings densitySettings)
        {
            var parent = new GameObject("PlanetObject").transform;
            
            // Current logic:
            //  - Divide planet up into chunks (cubes)
            //  - Each chunk has a density calculation, then marching cubes to mesh generation
            //  - Chunks account for max vertex count based on most expensive marching cubes outcome possible
            //    - max vertex count = 65535
            //    - max vertices for marching cubes = 5 tris * 3 = 15 vertices (includes
            //    - max voxel count in chunk = 65535 / 15 = 4369
            //    - max chunk size = 16 (16^3 = 4096) - refer to ChunkLengthInVoxels for practically used value
            //    - We could almost definitely go higher, but let's see what results we get first
            //      - hypothesis: smaller chunk size means less regeneration time for terraforming

            var lengthInChunksRaw = shapeSettings.radius * 2f / (ChunkLengthInVoxels * densitySettings.spacing);
            var lengthInChunks = Mathf.CeilToInt(lengthInChunksRaw);
            var numChunks = lengthInChunks * lengthInChunks * lengthInChunks;

            chunks = new PlanetChunk[numChunks];

            var virtualRadius = shapeSettings.radius * lengthInChunks / lengthInChunksRaw;

            var chunkSize = ChunkLengthInVoxels * densitySettings.spacing;
            for (int x = 0; x < lengthInChunks; x++)
            {
                for (int y = 0; y < lengthInChunks; y++)
                {
                    for (int z = 0; z < lengthInChunks; z++)
                    {
                        var index = x * lengthInChunks * lengthInChunks + y * lengthInChunks + z;
                        var offset = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) * chunkSize - Vector3.one * virtualRadius;
                        chunks[index] = PlanetChunk.Generate(densitySettings, offset, parent);
                    }
                }
            }

            return parent;
        }
    }
}