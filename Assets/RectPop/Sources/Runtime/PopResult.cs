using UnityEngine;

namespace RectPop
{
    public class PopResult
    {
        public readonly Position Pivot;
        public readonly Vector2 ScreenPoint;
        public readonly object Context;

        public PopResult(Position pivot, Vector2 screenPoint, object context = null)
        {
            Pivot = pivot;
            ScreenPoint = screenPoint;
            Context = context;
        }
    }
}