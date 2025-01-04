using System;
using UnityEngine;

namespace RectPop
{
    public class PopController : IPopSource
    {
        private static readonly IPopProvider Default = new DefaultPopProvider();
        private readonly IPopProvider _provider;
        private readonly Lazy<string> _sourceName;

        public PopController(IPopProvider provider)
        {
            _provider = provider;
            _sourceName = new Lazy<string>(GetSourceName);
        }

        public PopController() : this(provider: Default)
        {
        }

        public string SourceName => _sourceName.Value;

        public virtual PopResult Request(PopRequest request)
        {
            if (request is null)
            {
                Debug.LogError($"{nameof(request)} is null.");
                return null;
            }

            var result = _provider.Provide(request);
            if (result is null)
            {
                Debug.LogError($"Failed to get {nameof(PopResult)}");
                return null;
            }

            Dispatch(result);

            return result;
        }

        public virtual void Apply(PopResult result, RectTransform baseRectTransform, Canvas baseCanvas)
        {
            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

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

        public virtual void RequestAndApply(PopRequest request, RectTransform floatingUIRectTransform, Canvas floatingUICanvas)
        {
            if (request is null)
            {
                Debug.LogError($"{nameof(request)} is null.");
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

            var result = Request(request);
            if (result is null)
            {
                Debug.LogError($"Failed to get {nameof(PopResult)}");
                return;
            }

            Apply(result, floatingUIRectTransform, floatingUICanvas);
        }

        protected virtual string GetSourceName()
        {
            return $"{GetType().Name}_{Guid.NewGuid().ToString()}";
        }

        protected virtual void Dispatch(PopResult result)
        {
            PopDispatcher.Dispatch(source: this, result: result);
        }

        // default
        private class DefaultPopProvider : PopProviderBase
        {
        }
    }
}