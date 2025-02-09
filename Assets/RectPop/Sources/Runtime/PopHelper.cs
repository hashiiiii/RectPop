using UnityEngine;

namespace RectPop
{
    public static class PopHelper
    {
        public static void Apply(PopResult result, RectTransform floatingUIRectTransform, Canvas floatingUICanvas)
        {
            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

            if (floatingUIRectTransform is null)
            {
                Debug.LogError($"{nameof(floatingUIRectTransform)} is null.");
                return;
            }

            if (floatingUICanvas is null)
            {
                Debug.LogError($"{nameof(floatingUICanvas)} is null.");
                return;
            }

            if (floatingUIRectTransform.parent is not RectTransform floatingUIParentRect)
            {
                Debug.LogError($"{nameof(floatingUIRectTransform)} does not have a parent RectTransform.");
                return;
            }

            floatingUIRectTransform.pivot = PositionUtility.GetPivot(result.Pivot);
            floatingUIRectTransform.anchorMin = floatingUIRectTransform.anchorMax = floatingUIParentRect.pivot = PositionUtility.GetPivot(Position.MiddleCenter);

            var camera = floatingUICanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : floatingUICanvas.worldCamera;
            
            var isConverted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                floatingUIParentRect,
                result.ScreenPoint,
                camera,
                out var localPoint
            );

            if (!isConverted)
            {
                Debug.LogError("Failed to convert screen to local point.");
                return;
            }

            floatingUIRectTransform.anchoredPosition = localPoint;
        }
    }
}