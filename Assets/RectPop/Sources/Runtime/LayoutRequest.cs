using UnityEngine;

namespace RectPop
{
    public readonly struct LayoutRequest
    {
        public readonly RectTransform BaseRectTransform;
        public readonly Canvas BaseCanvas;
        public readonly LayoutType LayoutType;
        public readonly Vector2 Offset;
        public readonly float CenterThreshold;

        public LayoutRequest(
            RectTransform baseRectTransform,
            Canvas baseCanvas,
            LayoutType layoutType = LayoutType.Inside,
            Vector2 offset = default,
            float centerThreshold = 0f
        )
        {
            BaseRectTransform = baseRectTransform;
            BaseCanvas = baseCanvas;
            LayoutType = layoutType;
            Offset = offset;
            CenterThreshold = centerThreshold;
        }
    }
}