using UnityEngine;

namespace SharpShadowsToolkit
{
    [System.Serializable]
    public struct SingleLayer
    {
        [SerializeField] private int layerIndex;

        public int LayerIndex
        {
            get { return layerIndex; }
            set
            {
                if (value > 0 && value < 32)
                {
                    layerIndex = value;
                }
            }
        }

        public LayerMask Mask
        {
            get { return 1 << layerIndex; }
        }

        public static implicit operator SingleLayer(int value)
        {
            var singleLayer = new SingleLayer();
            singleLayer.LayerIndex = value;
            return singleLayer;
        }

        public static implicit operator int(SingleLayer value)
        {
            return value.LayerIndex;
        }
    }
}
