using UnityEngine;

namespace RectPop
{
    public readonly struct LayoutRequest
    {
        /// <summary> "RectTransform" that defines the target area. </summary>
        public readonly RectTransform TargetRectTransform;

        /// <summary> offset for positioning the layout. </summary>
        /// <remarks> x -> horizontal offset, y -> vertical offset </remarks>
        public readonly Vector2 Offset;

        /// <summary> axis to determine the direction of the layout. </summary>
        public readonly RectTransform.Axis Axis;

        /// <summary> threshold to determine if the layout is near the center of the screen. </summary>
        public readonly float CenterThreshold;

        /// <summary> "Camera" used to render the target "RectTransform". </summary>
        public readonly Camera TargetCamera;

        public LayoutRequest(
            RectTransform targetRectTransform,
            Vector2 offset,
            RectTransform.Axis axis = RectTransform.Axis.Horizontal,
            Camera targetCamera = null,
            float centerThreshold = 50f
        )
        {
            TargetCamera = targetCamera;
            TargetRectTransform = targetRectTransform;
            Offset = offset;
            Axis = axis;
            CenterThreshold = centerThreshold;
        }
    }
}