using UnityEngine;

namespace RectPop
{
    public class LayoutProvider : ILayoutProvider
    {
        /// <remarks> Use class variable to avoid creating new Vector3 instances every time. </remarks>
        private readonly Vector3[] _corners = new Vector3[4];
        private Vector3 _boundingBoxCenter;
        private bool _isBoundingBoxCenterDirty;

        /// <summary>
        /// calculates and provides the layout positioning data based on the given request.
        /// </summary>
        /// <param name="request">
        /// layout request parameters containing the target RectTransform, offset, center threshold, target camera and layout type.
        /// </param>
        /// <returns>
        /// <see cref="LayoutResult"/> struct containing the calculated pivot and screen position for the layout.
        /// returns default if the <see cref="LayoutRequest.TargetRectTransform"/> is null.
        /// </returns>
        /// <remarks>
        /// This method calculates the optimal pivot and screen position for a UI element relative to the target RectTransform.
        /// It evaluates the target's screen position to determine whether the layout should appear above, below, left, or right of the target.
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
            _isBoundingBoxCenterDirty = true;

            // 2. get the center of the bounding box (world coordinates)
            var boundingBoxCenter = (_corners[0] + _corners[1] + _corners[2] + _corners[3]) * 0.25f;

            // 3. convert to screen coordinates
            var boundingBoxCenterScreenPos = RectTransformUtility.WorldToScreenPoint(request.TargetCamera, boundingBoxCenter);

            // 4. determine the screen center.
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            // 5. determine the screen region based on boundingBoxCenterScreenPos
            var region = GetScreenRegion(
                boundingBoxCenterScreenPos: boundingBoxCenterScreenPos,
                screenCenter: screenCenter,
                threshold: request.CenterThreshold
            );

            // 6. determine anchor position and pivot based on the region and layout type
            GetAnchorAndPivot(region, request.LayoutType, _corners, out var anchorWorldPos, out var pivot);

            // 7. convert the anchor position to screen coordinates.
            var anchorScreenPos = RectTransformUtility.WorldToScreenPoint(request.TargetCamera, anchorWorldPos);

            // 8. apply offset
            ApplyOffset(
                anchorScreenPos: ref anchorScreenPos,
                region: region,
                offset: request.Offset
            );

            // 9. return the result
            return new LayoutResult(pivot: pivot, screenPosition: anchorScreenPos);
        }

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        private ScreenRegion GetScreenRegion(Vector2 boundingBoxCenterScreenPos, Vector2 screenCenter, float threshold)
        {
            var isAbove = boundingBoxCenterScreenPos.y > screenCenter.y + threshold;
            var isBelow = boundingBoxCenterScreenPos.y < screenCenter.y - threshold;
            var isRight = boundingBoxCenterScreenPos.x > screenCenter.x + threshold;
            var isLeft = boundingBoxCenterScreenPos.x < screenCenter.x - threshold;
            var isYAxis = !isLeft && !isRight;
            var isXAxis = !isAbove && !isBelow;

            if (isAbove && isLeft) return ScreenRegion.TopLeft;
            if (isAbove && isYAxis) return ScreenRegion.TopCenter;
            if (isAbove && isRight) return ScreenRegion.TopRight;
            if (isXAxis && isLeft) return ScreenRegion.MiddleLeft;
            if (isXAxis && isYAxis) return ScreenRegion.MiddleCenter;
            if (isXAxis && isRight) return ScreenRegion.MiddleRight;
            if (isBelow && isLeft) return ScreenRegion.BottomLeft;
            if (isBelow && isYAxis) return ScreenRegion.BottomCenter;
            if (isBelow && isRight) return ScreenRegion.BottomRight;

            // fallback
            return ScreenRegion.MiddleCenter;
        }

        private void GetAnchorAndPivot(ScreenRegion region, LayoutType layoutType, Vector3[] corners, out Vector3 anchorWorldPos, out Vector2 pivot)
        {
            switch (region)
            {
                case ScreenRegion.TopLeft:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal => corners[0],
                        LayoutType.OutsideVertical => corners[2],
                        _ => GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.0f, 1.0f);
                    break;
                case ScreenRegion.TopCenter:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.5f, 1.0f);
                    break;
                case ScreenRegion.TopRight:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal => corners[3],
                        LayoutType.OutsideVertical => corners[1],
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(1.0f, 1.0f);
                    break;
                case ScreenRegion.MiddleLeft:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[2] + corners[3]) * 0.5f,
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.0f, 0.5f);
                    break;
                case ScreenRegion.MiddleCenter:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal => (corners[2] + corners[3]) * 0.5f,
                        LayoutType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ScreenRegion.MiddleRight:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[0] + corners[1]) * 0.5f,
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(1.0f, 0.5f);
                    break;
                case ScreenRegion.BottomLeft:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal => corners[1],
                        LayoutType.OutsideVertical => corners[3],
                        _ => GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.0f, 0.0f);
                    break;
                case ScreenRegion.BottomCenter:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[1] + corners[2]) * 0.5f,
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ScreenRegion.BottomRight:
                    anchorWorldPos = layoutType switch
                    {
                        LayoutType.OutsideHorizontal => corners[2],
                        LayoutType.OutsideVertical => corners[0],
                        _=> GetBoundingBoxCenter(corners),
                    };
                    pivot = new Vector2(1.0f, 0.0f);
                    break;
                default:
                    anchorWorldPos = GetBoundingBoxCenter(corners);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }

        private Vector3 GetBoundingBoxCenter(Vector3[] corners)
        {
            if (_isBoundingBoxCenterDirty)
            {
                _boundingBoxCenter = (corners[0] + corners[1] + corners[2] + corners[3]) * 0.25f;
                _isBoundingBoxCenterDirty = false;
            }

            return _boundingBoxCenter;
        }

        private void ApplyOffset(ref Vector2 anchorScreenPos, ScreenRegion region, Vector2 offset)
        {
            // apply vertical offset
            switch (region)
            {
                case ScreenRegion.TopLeft:
                case ScreenRegion.TopCenter:
                case ScreenRegion.TopRight:
                    anchorScreenPos.y -= offset.y;
                    break;
                case ScreenRegion.BottomLeft:
                case ScreenRegion.BottomCenter:
                case ScreenRegion.BottomRight:
                    anchorScreenPos.y += offset.y;
                    break;
            }

            // apply horizontal offset
            switch (region)
            {
                case ScreenRegion.TopLeft:
                case ScreenRegion.MiddleLeft:
                case ScreenRegion.BottomLeft:
                    anchorScreenPos.x += offset.x;
                    break;
                case ScreenRegion.TopRight:
                case ScreenRegion.MiddleRight:
                case ScreenRegion.BottomRight:
                    anchorScreenPos.x -= offset.x;
                    break;
            }
        }
    }
}
