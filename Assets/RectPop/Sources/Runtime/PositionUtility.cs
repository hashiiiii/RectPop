using UnityEngine;

namespace RectPop
{
    public static class PositionUtility
    {
        public static Vector2 GetPivot(Position position)
        {
            return position switch
            {
                Position.TopLeft => new Vector2(0.0f, 1.0f),
                Position.TopCenter => new Vector2(0.5f, 1.0f),
                Position.TopRight => new Vector2(1.0f, 1.0f),
                Position.MiddleLeft => new Vector2(0.0f, 0.5f),
                Position.MiddleCenter => new Vector2(0.5f, 0.5f),
                Position.MiddleRight => new Vector2(1.0f, 0.5f),
                Position.BottomLeft => new Vector2(0.0f, 0.0f),
                Position.BottomCenter => new Vector2(0.5f, 0.0f),
                Position.BottomRight => new Vector2(1.0f, 0.0f),
                _ => new Vector2(0.5f, 0.5f),
            };
        }

        public static Position GetPosition(Vector2 pivot)
        {
            if (pivot == new Vector2(0.0f, 1.0f)) return Position.TopLeft;
            if (pivot == new Vector2(0.5f, 1.0f)) return Position.TopCenter;
            if (pivot == new Vector2(1.0f, 1.0f)) return Position.TopRight;
            if (pivot == new Vector2(0.0f, 0.5f)) return Position.MiddleLeft;
            if (pivot == new Vector2(0.5f, 0.5f)) return Position.MiddleCenter;
            if (pivot == new Vector2(1.0f, 0.5f)) return Position.MiddleRight;
            if (pivot == new Vector2(0.0f, 0.0f)) return Position.BottomLeft;
            if (pivot == new Vector2(0.5f, 0.0f)) return Position.BottomCenter;
            if (pivot == new Vector2(1.0f, 0.0f)) return Position.BottomRight;
            return Position.MiddleCenter;
        }
    }
}