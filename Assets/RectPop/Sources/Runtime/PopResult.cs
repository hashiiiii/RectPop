using UnityEngine;

namespace RectPop
{
    public class PopResult
    {
        /// <summary> pivot of the pop. </summary>
        /// <remarks> the pop refers to the UI elements such as popups. </remarks>
        public readonly Position Pivot;

        /// <summary> screen point of the pop. </summary>
        /// <remarks> the pop refers to the UI elements such as popups. </remarks>
        public readonly Vector2 ScreenPoint;

        public PopResult(Position pivot, Vector2 screenPoint)
        {
            Pivot = pivot;
            ScreenPoint = screenPoint;
        }
    }
}