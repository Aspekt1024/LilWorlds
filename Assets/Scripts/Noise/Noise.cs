using UnityEngine;

namespace Aspekt.Noise
{
    public enum NormalizeMode
    {
        Local = 0,
        Global = 1,
    }
    
    public static class NoiseUtil
    {
        public static float[,] GeneratePerlinNoise(int width, int height, Vector2 centre, PerlinNoiseSettings settings)
        {
            var maxPossibleHeight = 0f;
            var amplitude = 1f;
            
            var r = new System.Random(settings.seed.GetHashCode());
            var octaveOffsets = new Vector2[settings.octaves];
            for (int i = 0; i < settings.octaves; i++)
            {
                var xOffset = r.Next(-100000, 100000) + (settings.offset.x + centre.x);
                var yOffset = r.Next(-100000, 100000) + (settings.offset.y + centre.y);
                octaveOffsets[i] = new Vector2(xOffset, yOffset);

                maxPossibleHeight += amplitude;
                amplitude *= settings.persistence;
            }
            
            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            var noiseMap = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f;

                    for (int i = 0; i < settings.octaves; i++)
                    {
                        var xPos = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                        var yPos = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                        
                        var noiseValue = Mathf.PerlinNoise(xPos, yPos);
                        noiseHeight += noiseValue * amplitude;

                        amplitude *= settings.persistence;
                        frequency *= settings.lacunarity;
                    }

                    maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, noiseHeight);
                    minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, noiseHeight);

                    noiseMap[x, y] = noiseHeight;

                    if (settings.normalizeMode == NormalizeMode.Global)
                    {
                        var normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (settings.normalizeMode == NormalizeMode.Local)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                }
            }

            return noiseMap;
        }
    }
}