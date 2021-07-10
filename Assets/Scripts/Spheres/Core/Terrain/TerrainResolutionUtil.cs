using System;
using System.Collections.Generic;

namespace Aspekt.Spheres
{
    public static class TerrainResolutionUtil
    {
        // Max tris for a mesh with the default indexFormat of 16 is 65535.
        // Since we are using square terrain chunks, the max number of vertices along and edge is 255.
        // Our resolution refers to the number of squares in either direction, rather than the number
        // of vertices, where #verts = resolution + 1, so the max resolution is 254.

        // We also want our mesh to have a detail setting, so we can skip a number of vertices to generate
        // a mesh quicker (e.g. in the distance).
        // The resolution must be divisible by this skip increment.
        // The following resolutions are all divisible by 1, 2, 4, 6, 8 and 12
        
        private const int MaxResolution = 240;
        private const int LargeResolution = 192;
        private const int MediumResolution = 120;
        private const int SmallResolution = 96;
        
        private const int LOD0Interval = 1;
        private const int LOD1Interval = 2;
        private const int LOD2Interval = 4;
        private const int LOD3Interval = 8;
        
        public enum Resolutions
        {
            Max = 0,
            Large = 1000,
            Medium = 2000,
            Small = 3000,
        }

        public enum LOD
        {
            LOD0 = 0,
            LOD1 = 1000,
            LOD2 = 2000,
            LOD3 = 3000,
        }

        public static int GetResolutionSize(Resolutions resolution)
        {
            return resolution switch
            {
                Resolutions.Max => MaxResolution,
                Resolutions.Large => LargeResolution,
                Resolutions.Medium => MediumResolution,
                Resolutions.Small => SmallResolution,
                _ => MaxResolution,
            };
        }

        public static int GetSkipInterval(LOD lod)
        {
            return lod switch
            {
                LOD.LOD0 => LOD0Interval,
                LOD.LOD1 => LOD1Interval,
                LOD.LOD2 => LOD2Interval,
                LOD.LOD3 => LOD3Interval,
                _ => LOD0Interval
            };
        }
    }
}