using UnityEngine;

namespace RectPop
{
    public class PopRequest
    {
        public readonly RectTransform BaseRectTransform;
        public readonly Canvas BaseCanvas;
        public readonly PopType PopType;
        public readonly Vector2 Offset;
        public readonly float CenterThreshold;
        public readonly object Context;

        public PopRequest(
            RectTransform baseRectTransform,
            Canvas baseCanvas,
            PopType popType = PopType.Inside,
            Vector2 offset = default,
            float centerThreshold = 0f,
            object context = null
        )
        {
            BaseRectTransform = baseRectTransform;
            BaseCanvas = baseCanvas;
            PopType = popType;
            Offset = offset;
            CenterThreshold = centerThreshold;
            Context = context;
        }
    }
}