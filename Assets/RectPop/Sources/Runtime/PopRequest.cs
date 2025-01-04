using UnityEngine;

namespace RectPop
{
    public class PopRequest
    {
        /// <summary> the base UI RectTransform </summary>
        /// <remarks>
        /// the base RectTransform refers to the UI element, such as a button, that serves as the reference for displaying the pop.
        /// </remarks>
        public readonly RectTransform BaseRectTransform;

        /// <summary> the base Canvas RectTransform </summary>
        /// <remarks> the base Canvas refers to the Canvas in which the base RectTransform exists. </remarks>
        public readonly Canvas BaseCanvas;
        public readonly PopType PopType;
        public readonly Vector2 Offset;
        public readonly float CenterThreshold;

        public PopRequest(
            RectTransform baseRectTransform,
            Canvas baseCanvas,
            PopType popType = PopType.Inside,
            Vector2 offset = default,
            float centerThreshold = 0f
        )
        {
            BaseRectTransform = baseRectTransform;
            BaseCanvas = baseCanvas;
            PopType = popType;
            Offset = offset;
            CenterThreshold = centerThreshold;
        }
    }
}