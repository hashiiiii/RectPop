using UnityEngine;

namespace RectPop
{
    public readonly struct LayoutResult
    {
        /// <summary> pivot point of the layout. </summary>
        public readonly Vector2 Pivot;

        /// <summary> screen position of the layout. </summary>
        public readonly Vector2 ScreenPosition;

        public LayoutResult(Vector2 pivot, Vector2 screenPosition)
        {
            Pivot = pivot;
            ScreenPosition = screenPosition;
        }
    }
}