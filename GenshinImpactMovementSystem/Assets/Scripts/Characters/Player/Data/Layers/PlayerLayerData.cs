using System;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
        public bool ContainsLayer(LayerMask layerMask, int Layer)
        {
            return (1 << Layer & layerMask) != 0;  
        }

        public bool IsGroundLayer(int Layer)
        {
            return ContainsLayer(GroundLayer, Layer);
        }
    }
}