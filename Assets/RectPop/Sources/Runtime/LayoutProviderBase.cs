using UnityEngine;

namespace RectPop
{
    public abstract class LayoutProviderBase : ILayoutProvider
    {
        private readonly Vector3[] _corners = new Vector3[4];
        private Vector3 _boundingBoxCenter;
        private bool _isBoundingBoxCenterDirty;

        public virtual LayoutResult Provide(LayoutRequest request)
        {
            if (request.BaseRectTransform is null)
            {
                Debug.LogError($"{nameof(request.BaseRectTransform)} is null.");
                return new LayoutResult(pivot: default, screenPoint: default);
            }

            // 1. get the four corners of the bounding box
            // ---------------------------------------------
            // corners[0] = bottom left, corners[1] = top left,
            // corners[2] = top right, corners[3] = bottom right
            request.BaseRectTransform.GetWorldCorners(_corners);
            _isBoundingBoxCenterDirty = true;

            // 2. get the center of the bounding box (world coordinates)
            var boundingBoxCenter = (_corners[0] + _corners[1] + _corners[2] + _corners[3]) * 0.25f;

            // 3. determine the camera
            var camera = request.BaseCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : request.BaseCanvas.worldCamera;

            // 4. convert to screen coordinates
            var boundingBoxCenterScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, boundingBoxCenter);

            // 5. determine the screen center point
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            // 6. determine the Position based on boundingBoxCenterScreenPoint and screenCenterPoint
            var position = GetBoundingBoxPosition(
                boundingBoxCenterScreenPoint: boundingBoxCenterScreenPoint,
                screenCenter: screenCenterPoint,
                threshold: request.CenterThreshold
            );

            // 7. determine the layout anchor position based on the Position and LayoutType
            var layoutAnchorWorldPoint = GetLayoutAnchorWorldPoint(position, request.LayoutType, _corners);

            // 8. convert the layout anchor point to screen point
            var layoutAnchorScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, layoutAnchorWorldPoint);

            // 9. apply offset
            ApplyOffset(
                layoutAnchorScreenPoint: ref layoutAnchorScreenPoint,
                position: position,
                offset: request.Offset
            );

            // 10. return the result
            return new LayoutResult(pivot: position, screenPoint: layoutAnchorScreenPoint);
        }

        protected virtual Vector3 GetLayoutAnchorWorldPoint(Position position, LayoutType layoutType, Vector3[] corners)
        {
            return position switch
            {
                Position.TopLeft => layoutType switch
                {
                    LayoutType.OutsideHorizontal => corners[0],
                    LayoutType.OutsideVertical => corners[2],
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.TopCenter => layoutType switch
                {
                    LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.TopRight => layoutType switch
                {
                    LayoutType.OutsideHorizontal => corners[3],
                    LayoutType.OutsideVertical => corners[1],
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.MiddleLeft => layoutType switch
                {
                    LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[2] + corners[3]) * 0.5f,
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.MiddleCenter => layoutType switch
                {
                    LayoutType.OutsideHorizontal => (corners[2] + corners[3]) * 0.5f,
                    LayoutType.OutsideVertical => (corners[0] + corners[3]) * 0.5f,
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.MiddleRight => layoutType switch
                {
                    LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[0] + corners[1]) * 0.5f,
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.BottomLeft => layoutType switch
                {
                    LayoutType.OutsideHorizontal => corners[1],
                    LayoutType.OutsideVertical => corners[3],
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.BottomCenter => layoutType switch
                {
                    LayoutType.OutsideHorizontal or LayoutType.OutsideVertical => (corners[1] + corners[2]) * 0.5f,
                    _ => GetBoundingBoxCenter(corners),
                },
                Position.BottomRight => layoutType switch
                {
                    LayoutType.OutsideHorizontal => corners[2],
                    LayoutType.OutsideVertical => corners[0],
                    _ => GetBoundingBoxCenter(corners),
                },
                _ => GetBoundingBoxCenter(corners),
            };
        }

        protected virtual void ApplyOffset(ref Vector2 layoutAnchorScreenPoint, Position position, Vector2 offset)
        {
            // apply vertical offset
            switch (position)
            {
                case Position.TopLeft:
                case Position.TopCenter:
                case Position.TopRight:
                    layoutAnchorScreenPoint.y -= offset.y;
                    break;
                case Position.BottomLeft:
                case Position.BottomCenter:
                case Position.BottomRight:
                    layoutAnchorScreenPoint.y += offset.y;
                    break;
            }

            // apply horizontal offset
            switch (position)
            {
                case Position.TopLeft:
                case Position.MiddleLeft:
                case Position.BottomLeft:
                    layoutAnchorScreenPoint.x += offset.x;
                    break;
                case Position.TopRight:
                case Position.MiddleRight:
                case Position.BottomRight:
                    layoutAnchorScreenPoint.x -= offset.x;
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

            // fallback
            return Position.MiddleCenter;
        }
    }
}
