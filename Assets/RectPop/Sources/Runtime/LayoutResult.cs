using UnityEngine;

namespace RectPop
{
    public readonly struct LayoutResult
    {
        /// <summary> pivot of the layout. </summary>
        public readonly Position Pivot;

        /// <summary> screen point of the layout. </summary>
        public readonly Vector2 ScreenPoint;

        public LayoutResult(Position pivot, Vector2 screenPoint)
        {
            Pivot = pivot;
            ScreenPoint = screenPoint;
        }
    }
}