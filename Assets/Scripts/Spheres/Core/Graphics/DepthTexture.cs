using UnityEngine;

namespace Aspekt.Spheres.Graphics
{
    [ExecuteInEditMode]
    public class DepthTexture : MonoBehaviour
    {
        private Camera cam;
        
        private void Start()
        {
            cam = GetComponent<Camera>();
            cam.depthTextureMode = DepthTextureMode.Depth;
        }
    }
}