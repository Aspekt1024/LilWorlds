using System;
using UnityEngine;

namespace Aspekt
{
    public class SettingsComponent : ScriptableObject
    {
        public Action OnSettingsChanged;
        
        protected virtual void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}