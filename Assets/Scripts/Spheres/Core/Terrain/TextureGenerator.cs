using UnityEngine;
using Aspekt.Noise;

namespace Aspekt.Spheres
{
    public static class TextureGenerator
    {
        public static Texture GenerateTexture(float[,] heightMap, TerrainColorSettings colorSettings, float colourWeight)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);

            var texture = new Texture2D(width, height)
            {
                filterMode = colorSettings.textureFilterMode,
                wrapMode = TextureWrapMode.Clamp,
            };

            var colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                var x = i % width;
                var y = Mathf.FloorToInt(i / height);
                var color = colorSettings.grad.Evaluate(heightMap[x, y]);
                colors[i] = Color.Lerp(Color.white * heightMap[x, y], color, colourWeight);
            }

            texture.SetPixels(colors);
            texture.Apply();
            
            return texture;
        }
    }
}