using UnityEngine;

namespace RectPop
{
    public readonly struct LayoutRequest
    {
        /// <summary> "RectTransform" that defines the target area. </summary>
        public readonly RectTransform LayoutRectTransform;

        /// <summary> type of layout to apply. </summary>
        public readonly LayoutType LayoutType;

        /// <summary> offset for positioning the layout. </summary>
        /// <remarks> x -> horizontal offset, y -> vertical offset </remarks>
        public readonly Vector2 Offset;

        /// <summary> threshold to determine if the layout is near the center of the screen. </summary>
        public readonly float CenterThreshold;

        /// <summary> "Camera" used to render the target "RectTransform". </summary>
        public readonly Camera LayoutCamera;

        public LayoutRequest(
            RectTransform layoutRectTransform,
            LayoutType layoutType,
            Vector2 offset = default,
            float centerThreshold = 0f,
            Camera layoutCamera = null
        )
        {
            LayoutRectTransform = layoutRectTransform;
            LayoutType = layoutType;
            Offset = offset;
            CenterThreshold = centerThreshold;
            LayoutCamera = layoutCamera;
        }
    }
}