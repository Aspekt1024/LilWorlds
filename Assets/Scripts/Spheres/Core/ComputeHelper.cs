using UnityEngine;

namespace Aspekt.Spheres
{
    public class ComputeHelper
    {
        
        public static void Run (ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
        {
            Vector3Int threadGroupSizes = GetThreadGroupSizes (cs, kernelIndex);
            int numGroupsX = Mathf.CeilToInt (numIterationsX / (float) threadGroupSizes.x);
            int numGroupsY = Mathf.CeilToInt (numIterationsY / (float) threadGroupSizes.y);
            int numGroupsZ = Mathf.CeilToInt (numIterationsZ / (float) threadGroupSizes.y);
            cs.Dispatch (kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
        }
        
        public static Vector3Int GetThreadGroupSizes (ComputeShader compute, int kernelIndex = 0)
        {
            compute.GetKernelThreadGroupSizes (kernelIndex, out var x, out var y, out var z);
            return new Vector3Int ((int) x, (int) y, (int) z);
        }
        
        public static void CreateStructuredBuffer<T> (ref ComputeBuffer buffer, T[] data)
        {
            CreateStructuredBuffer<T> (ref buffer, data.Length);
            buffer.SetData (data);
        }
        
        public static void CreateStructuredBuffer<T> (ref ComputeBuffer buffer, int count)
        {
            int stride = System.Runtime.InteropServices.Marshal.SizeOf (typeof (T));
            bool createNewBuffer = buffer == null || !buffer.IsValid () || buffer.count != count || buffer.stride != stride;
            if (createNewBuffer)
            {
                Release (buffer);
                buffer = new ComputeBuffer (count, stride);
            }
        }
        
        public static ComputeBuffer CreateAndSetBuffer<T> (T[] data, ComputeShader cs, string nameID, int kernelIndex = 0) {
            ComputeBuffer buffer = null;
            CreateAndSetBuffer<T> (ref buffer, data, cs, nameID, kernelIndex);
            return buffer;
        }

        public static void CreateAndSetBuffer<T> (ref ComputeBuffer buffer, T[] data, ComputeShader cs, string nameID, int kernelIndex = 0) {
            int stride = System.Runtime.InteropServices.Marshal.SizeOf (typeof (T));
            CreateStructuredBuffer<T> (ref buffer, data.Length);
            buffer.SetData (data);
            cs.SetBuffer (kernelIndex, nameID, buffer);
        }

        public static ComputeBuffer CreateAndSetBuffer<T> (int length, ComputeShader cs, string nameID, int kernelIndex = 0) {
            ComputeBuffer buffer = null;
            CreateAndSetBuffer<T> (ref buffer, length, cs, nameID, kernelIndex);
            return buffer;
        }

        public static void CreateAndSetBuffer<T> (ref ComputeBuffer buffer, int length, ComputeShader cs, string nameID, int kernelIndex = 0) {
            CreateStructuredBuffer<T> (ref buffer, length);
            cs.SetBuffer (kernelIndex, nameID, buffer);
        }
        
        public static void Release (params ComputeBuffer[] buffers)
        {
            for (int i = 0; i < buffers.Length; i++)
            {
                if (buffers[i] != null)
                {
                    buffers[i].Release ();
                }
            }
        }
        
    }
}