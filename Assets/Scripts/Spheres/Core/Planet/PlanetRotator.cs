using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class PlanetRotator : MonoBehaviour
    {
        private Rigidbody body;
        
        private void Awake()
        {
            body = GetComponent<Rigidbody>();

        }

        private void Update()
        {
            
            body.angularVelocity = Vector3.up * 0.2f;
        }
    }
    
}