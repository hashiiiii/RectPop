using UnityEngine;

namespace RectPop
{
    public class LayoutProvider : ILayoutProvider
    {
        /// <remarks> Use class variable to avoid creating new Vector3 instances every time. </remarks>
        private readonly Vector3[] _corners = new Vector3[4];

        /// <summary>
        /// calculates and provides the layout positioning data based on the given request.
        /// </summary>
        /// <param name="request">
        /// layout request parameters containing the target RectTransform, camera, offset, axis, and center threshold.
        /// </param>
        /// <returns>
        /// <see cref="LayoutResult"/> struct containing the calculated pivot and screen position for the layout.
        /// returns default if the <see cref="LayoutRequest.TargetRectTransform"/> is null.
        /// </returns>
        /// <remarks>
        /// This method determines the optimal pivot and screen position for a UI element relative to the target RectTransform.
        /// It considers the target's position on the screen to decide whether the layout should appear above, below, left, or right of the target.
        /// An offset is applied based on the specified axis and calculated position to ensure proper spacing.
        /// </remarks>
        public LayoutResult Provide(LayoutRequest request)
        {
            if (request.TargetRectTransform is null) return default;

            // 1. get the four corners of the bounding box
            // ---------------------------------------------
            // corners[0] = bottom left, corners[1] = top left,
            // corners[2] = top right, corners[3] = bottom right
            request.TargetRectTransform.GetWorldCorners(_corners);

            // 2. get the center of the bounding box (world coordinates)
            var boundingBoxCenter = (_corners[0] + _corners[1] + _corners[2] + _corners[3]) * 0.25f;

            // 3. convert to screen coordinates
            var boundingBoxCenterScreenPos = RectTransformUtility.WorldToScreenPoint(request.TargetCamera, boundingBoxCenter);

            // 4. compare the center of the screen with the center of the bounding box
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            var isAbove = boundingBoxCenterScreenPos.y > screenCenter.y;
            var isRight = boundingBoxCenterScreenPos.x > screenCenter.x;
            var isNearCenterX = Mathf.Abs(boundingBoxCenterScreenPos.x - screenCenter.x) <= request.CenterThreshold;
            var isNearCenterY = Mathf.Abs(boundingBoxCenterScreenPos.y - screenCenter.y) <= request.CenterThreshold;

            // 5. determine which "anchor" to use and which "pivot" to set
            // -------------------------------------------------------------
            // isAbove: above / below
            // isRight: right / left
            // isNearCenterX: near the horizontal center
            // isNearCenterY: near the vertical center
            Vector3 anchorWorldPos;
            Vector2 pivot;

            // Horizontal
            if (request.Axis == RectTransform.Axis.Horizontal)
            {
                if (isRight)
                {
                    if (isNearCenterY)
                    {
                        // the layout is displayed on the left
                        anchorWorldPos = (_corners[0] + _corners[1]) * 0.5f;
                        pivot = new Vector2(0.0f, 0.5f);
                    }
                    else if (isAbove)
                    {
                        // the layout is displayed on the left top
                        anchorWorldPos = _corners[1];
                        pivot = new Vector2(1.0f, 1.0f);
                    }
                    else
                    {
                        // the layout is displayed on the left bottom
                        anchorWorldPos = _corners[0];
                        pivot = new Vector2(1.0f, 0.0f);
                    }
                }
                else
                {
                    // NOTE: this logic applies when it is exactly in the center of the screen
                    if (isNearCenterY)
                    {
                        // the layout is displayed on the right
                        anchorWorldPos = (_corners[2] + _corners[3]) * 0.5f;
                        pivot = new Vector2(1.0f, 0.5f);
                    }
                    else if (isAbove)
                    {
                        // the layout is displayed on the right top
                        anchorWorldPos = _corners[2];
                        pivot = new Vector2(0.0f, 1.0f);
                    }
                    else
                    {
                        // the layout is displayed on the right bottom
                        anchorWorldPos = _corners[3];
                        pivot = new Vector2(0.0f, 0.0f);
                    }
                }
            }
            // Vertical
            else
            {
                if (isAbove)
                {
                    if (isNearCenterX)
                    {
                        // the layout is displayed on the bottom
                        anchorWorldPos = (_corners[0] + _corners[3]) * 0.5f;
                        pivot = new Vector2(0.5f, 1.0f);
                    }
                    else if (isRight)
                    {
                        // the layout is displayed on the bottom right
                        anchorWorldPos = _corners[3];
                        pivot = new Vector2(1.0f, 1.0f);
                    }
                    else
                    {
                        // the layout is displayed on the bottom left
                        anchorWorldPos = _corners[0];
                        pivot = new Vector2(0.0f, 1.0f);
                    }
                }
                else
                {
                    // NOTE: this logic applies when it is exactly in the center of the screen
                    if (isNearCenterX)
                    {
                        // the layout is displayed on the top
                        anchorWorldPos = (_corners[1] + _corners[2]) * 0.5f;
                        pivot = new Vector2(0.5f, 0.0f);
                    }
                    else if (isRight)
                    {
                        // the layout is displayed on the top right
                        anchorWorldPos = _corners[2];
                        pivot = new Vector2(1.0f, 0.0f);
                    }
                    else
                    {
                        // the layout is displayed on the top left
                        anchorWorldPos = _corners[1];
                        pivot = new Vector2(0.0f, 0.0f);
                    }
                }
            }

            // 6. convert to screen coordinates
            var anchorScreenPos = RectTransformUtility.WorldToScreenPoint(request.TargetCamera, anchorWorldPos);

            // 7. add offset
            var offset = request.Offset;
            if (request.Axis == RectTransform.Axis.Horizontal)
            {
                anchorScreenPos.x += isRight ? -offset.x : offset.x;
                anchorScreenPos.y += isNearCenterY ? 0f : isAbove ? -offset.y : offset.y;
            }
            else
            {
                anchorScreenPos.x += isNearCenterX ? 0f : isRight ? -offset.x : offset.x;
                anchorScreenPos.y += isAbove ? -offset.y : offset.y;
            }

            // 8. return the result
            return new LayoutResult(pivot: pivot, screenPosition: anchorScreenPos);
        }
    }
}
