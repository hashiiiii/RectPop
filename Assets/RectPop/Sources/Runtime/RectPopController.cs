using UnityEngine;

namespace RectPop
{
    public class RectPopController
    {
        // constant
        private static readonly IPopProvider Default = new DefaultPopProvider();

        // dependency
        private readonly IPopProvider _popProvider;

        // constructor
        public RectPopController(IPopProvider popProvider)
        {
            _popProvider = popProvider;
        }

        public RectPopController() : this(Default)
        {
        }

        public PopResult Provide(PopRequest request)
        {
            return _popProvider.Provide(request);
        }

        public void Apply(PopResult result, RectTransform baseRectTransform, Canvas baseCanvas)
        {
            if (baseRectTransform is null || baseCanvas is null)
            {
                Debug.LogError($"{nameof(baseRectTransform)} or {nameof(baseCanvas)} is null.");
                return;
            }

            if (baseRectTransform.parent is not RectTransform baseParentRect)
            {
                Debug.LogError($"{nameof(baseRectTransform)} does not have a parent RectTransform.");
                return;
            }

            baseRectTransform.pivot = PositionUtility.GetPivot(result.Pivot);
            baseRectTransform.anchorMin = baseRectTransform.anchorMax = baseParentRect.pivot = PositionUtility.GetPivot(Position.MiddleCenter);

            var camera = baseCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : baseCanvas.worldCamera;
            
            var isConverted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                baseParentRect,
                result.ScreenPoint,
                camera,
                out var localPoint
            );

            if (isConverted)
            {
                baseRectTransform.anchoredPosition = localPoint;
            }
            else
            {
                Debug.LogError("Failed to convert screen to local point.");
            }
        }

        public void ProvideAndApply(PopRequest request, RectTransform popRectTransform, Canvas popCanvas)
        {
            var result = Provide(request);
            if (result.Pivot is Position.None)
            {
                Debug.LogError($"Failed to get {nameof(PopResult)}");
                return;
            }

            Apply(result, popRectTransform, popCanvas);
        }

        // default
        private class DefaultPopProvider : PopProviderBase
        {
        }
    }
}