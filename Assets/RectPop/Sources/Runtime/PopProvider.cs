using UnityEngine;

namespace RectPop
{
    public class PopProvider : IPopProvider
    {
        public virtual PopResult Provide(PopRequest request)
        {
            if (request is null)
            {
                Debug.LogError($"{nameof(request)} is null.");
                return null;
            }

            if (request.BaseRectTransform is null)
            {
                Debug.LogError($"{nameof(request.BaseRectTransform)} is null.");
                return null;
            }

            if (request.BaseCanvas is null)
            {
                Debug.LogError($"{nameof(request.BaseCanvas)} is null.");
                return null;
            }

            // 1. get the four corners of the bounding box
            // ---------------------------------------------
            // corners[0] = bottom left, corners[1] = top left,
            // corners[2] = top right, corners[3] = bottom right
            var corners = new Vector3[4];
            request.BaseRectTransform.GetWorldCorners(corners);

            // 2. get the center of the bounding box (world coordinates)
            var boundingBoxCenter = (corners[0] + corners[1] + corners[2] + corners[3]) * 0.25f;

            // 3. determine the camera
            var camera = request.BaseCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : request.BaseCanvas.worldCamera;

            // 4. convert to screen coordinates
            var boundingBoxCenterScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, boundingBoxCenter);

            // 5. determine the screen center point
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            // 6. determine the Position based on boundingBoxCenterScreenPoint and screenCenterPoint
            var boundingBoxPosition = GetBoundingBoxPosition(
                boundingBoxCenterScreenPoint: boundingBoxCenterScreenPoint,
                screenCenter: screenCenterPoint,
                threshold: request.CenterThreshold
            );

            // 7. determine the pop anchor position based on the Position and PopType
            var popAnchorWorldPoint = GetPopAnchorWorldPoint(boundingBoxPosition, request.PopType, corners);

            // 8. convert the pop anchor point to screen point
            var popAnchorScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, popAnchorWorldPoint);

            // 9. determine the pivot
            var pivotPosition = GetPopPivotPosition(boundingBoxPosition, request.PopType);

            // 10. apply offset
            ApplyOffset(
                popAnchorScreenPoint: ref popAnchorScreenPoint,
                pivotPosition: pivotPosition,
                offset: request.Offset
            );

            // 11. return the result
            return new PopResult(pivot: pivotPosition, screenPoint: popAnchorScreenPoint, context: request.Context);
        }

        protected virtual Vector3 GetPopAnchorWorldPoint(Position position, PopType popType, Vector3[] corners)
        {
            return position switch
            {
                Position.TopLeft => popType switch
                {
                    PopType.OutsideHorizontal => corners[2],
                    PopType.OutsideVertical => corners[0],
                    _ => GetBoundingBoxCenter(),
                },
                Position.TopCenter => popType switch
                {
                    PopType.OutsideHorizontal => corners[2],
                    PopType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                    _ => GetBoundingBoxCenter(),
                },
                Position.TopRight => popType switch
                {
                    PopType.OutsideHorizontal => corners[1],
                    PopType.OutsideVertical => corners[3],
                    _ => GetBoundingBoxCenter(),
                },
                Position.MiddleLeft => popType switch
                {
                    PopType.OutsideHorizontal => (corners[2] + corners[3]) * 0.5f,
                    PopType.OutsideVertical => corners[0],
                    _ => GetBoundingBoxCenter(),
                },
                Position.MiddleCenter => popType switch
                {
                    PopType.OutsideHorizontal => (corners[2] + corners[3]) * 0.5f,
                    PopType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                    _ => GetBoundingBoxCenter(),
                },
                Position.MiddleRight => popType switch
                {
                    PopType.OutsideHorizontal => (corners[0] + corners[1]) * 0.5f,
                    PopType.OutsideVertical => corners[3],
                    _ => GetBoundingBoxCenter(),
                },
                Position.BottomLeft => popType switch
                {
                    PopType.OutsideHorizontal => corners[3],
                    PopType.OutsideVertical => corners[1],
                    _ => GetBoundingBoxCenter(),
                },
                Position.BottomCenter => popType switch
                {
                    PopType.OutsideHorizontal => corners[3],
                    PopType.OutsideVertical => (corners[1] + corners[2]) * 0.5f,
                    _ => GetBoundingBoxCenter(),
                },
                Position.BottomRight => popType switch
                {
                    PopType.OutsideHorizontal => corners[0],
                    PopType.OutsideVertical => corners[2],
                    _ => GetBoundingBoxCenter(),
                },
                _ => GetBoundingBoxCenter(),
            };

            Vector3 GetBoundingBoxCenter()
            {
                return (corners[0] + corners[1] + corners[2] + corners[3]) * 0.25f;
            }
        }

        protected virtual void ApplyOffset(ref Vector2 popAnchorScreenPoint, Position pivotPosition, Vector2 offset)
        {
            // apply vertical offset
            switch (pivotPosition)
            {
                case Position.TopLeft:
                case Position.TopCenter:
                case Position.TopRight:
                    popAnchorScreenPoint.y -= offset.y;
                    break;
                case Position.BottomLeft:
                case Position.BottomCenter:
                case Position.BottomRight:
                    popAnchorScreenPoint.y += offset.y;
                    break;
            }

            // apply horizontal offset
            switch (pivotPosition)
            {
                case Position.TopLeft:
                case Position.MiddleLeft:
                case Position.BottomLeft:
                    popAnchorScreenPoint.x += offset.x;
                    break;
                case Position.TopRight:
                case Position.MiddleRight:
                case Position.BottomRight:
                    popAnchorScreenPoint.x -= offset.x;
                    break;
            }
        }

        protected virtual Position GetPopPivotPosition(Position position, PopType popType)
        {
            return position switch
            {
                Position.TopLeft => popType switch
                {
                    _ => Position.TopLeft,
                },
                Position.TopCenter => popType switch
                {
                    PopType.OutsideHorizontal => Position.TopLeft,
                    _ => Position.TopCenter,
                },
                Position.TopRight => popType switch
                {
                    _ => Position.TopRight,
                },
                Position.MiddleLeft => popType switch
                {
                    PopType.OutsideVertical => Position.TopLeft,
                    _ => Position.MiddleLeft,
                },
                Position.MiddleCenter => popType switch
                {
                    PopType.OutsideHorizontal => Position.MiddleLeft,
                    PopType.OutsideVertical => Position.TopCenter,
                    _ => Position.MiddleCenter,
                },
                Position.MiddleRight => popType switch
                {
                    PopType.OutsideVertical => Position.TopRight,
                    _ => Position.MiddleRight,
                },
                Position.BottomLeft => popType switch
                {
                    _ => Position.BottomLeft,
                },
                Position.BottomCenter => popType switch
                {
                    PopType.OutsideHorizontal => Position.BottomLeft,
                    _ => Position.BottomCenter,
                },
                Position.BottomRight => popType switch
                {
                    _ => Position.BottomRight,
                },
                _ => Position.MiddleCenter,
            };
        }

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        private Position GetBoundingBoxPosition(Vector2 boundingBoxCenterScreenPoint, Vector2 screenCenter, float threshold)
        {
            var isAbove = boundingBoxCenterScreenPoint.y > screenCenter.y + threshold;
            var isBelow = boundingBoxCenterScreenPoint.y < screenCenter.y - threshold;
            var isRight = boundingBoxCenterScreenPoint.x > screenCenter.x + threshold;
            var isLeft = boundingBoxCenterScreenPoint.x < screenCenter.x - threshold;
            var isYAxis = !isLeft && !isRight;
            var isXAxis = !isAbove && !isBelow;

            if (isAbove && isLeft) return Position.TopLeft;
            if (isAbove && isYAxis) return Position.TopCenter;
            if (isAbove && isRight) return Position.TopRight;
            if (isXAxis && isLeft) return Position.MiddleLeft;
            if (isXAxis && isYAxis) return Position.MiddleCenter;
            if (isXAxis && isRight) return Position.MiddleRight;
            if (isBelow && isLeft) return Position.BottomLeft;
            if (isBelow && isYAxis) return Position.BottomCenter;
            if (isBelow && isRight) return Position.BottomRight;

            return Position.MiddleCenter;
        }
    }
}
