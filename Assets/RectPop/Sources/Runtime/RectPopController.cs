using UnityEngine;

namespace RectPop
{
    public class RectPopController
    {
        // constant
        private static readonly ILayoutProvider Default = new DefaultLayoutProvider();

        // dependency
        private readonly ILayoutProvider _layoutProvider;

        // constructor
        public RectPopController(ILayoutProvider layoutProvider)
        {
            _layoutProvider = layoutProvider;
        }

        public RectPopController() : this(Default)
        {
        }

        public LayoutResult Provide(LayoutRequest request)
        {
            return _layoutProvider.Provide(request);
        }

        public void Apply(LayoutResult result, RectTransform layoutRect, Canvas layoutCanvas)
        {
            if (layoutRect is null || layoutCanvas is null)
            {
                Debug.LogError($"{nameof(layoutRect)} or {nameof(layoutCanvas)} is null.");
                return;
            }

            if (layoutRect.parent is not RectTransform layoutParentRect)
            {
                Debug.LogError($"{nameof(layoutRect)} does not have a parent RectTransform.");
                return;
            }

            layoutRect.pivot = PositionUtility.GetPivot(result.Pivot);
            layoutRect.anchorMin = layoutRect.anchorMax = layoutParentRect.pivot = PositionUtility.GetPivot(Position.MiddleCenter);

            var camera = layoutCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : layoutCanvas.worldCamera;
            
            var isConverted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                layoutParentRect,
                result.ScreenPoint,
                camera,
                out var localPoint
            );

            if (isConverted)
            {
                layoutRect.anchoredPosition = localPoint;
            }
            else
            {
                Debug.LogError("Failed to convert screen to local point.");
            }
        }

        public void ProvideAndApply(LayoutRequest request, RectTransform layoutRect, Canvas layoutCanvas)
        {
            Apply(result: Provide(request), layoutRect, layoutCanvas);
        }

        // default
        private class DefaultLayoutProvider : LayoutProviderBase
        {
        }
    }
}