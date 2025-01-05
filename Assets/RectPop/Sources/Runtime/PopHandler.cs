using System;
using UnityEngine;

namespace RectPop
{
    public class PopHandler : IPopHandler
    {
        private static readonly IPopProvider Default = new PopProvider();
        private readonly IPopProvider _provider;
        private readonly Lazy<string> _id;

        public PopHandler(IPopProvider provider)
        {
            _provider = provider;

            // lazy initialization of the ID, which will be generated using the GetId method.
            // GetId is a virtual method, so to ensure the correct implementation in derived classes is called,
            // we use lazy evaluation to delay the ID generation until it is actually needed.
            _id = new Lazy<string>(GetId);
        }

        public PopHandler() : this(provider: Default)
        {
        }

        public string Id => _id.Value;

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

            if (baseRectTransform is null)
            {
                Debug.LogError($"{nameof(baseRectTransform)} is null.");
                return;
            }

            if (baseCanvas is null)
            {
                Debug.LogError($"{nameof(baseCanvas)} is null.");
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

            if (!isConverted)
            {
                Debug.LogError("Failed to convert screen to local point.");
                return;
            }

            baseRectTransform.anchoredPosition = localPoint;
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

        protected virtual string GetId()
        {
            return $"{GetType().Name}_{Guid.NewGuid().ToString()}";
        }

        protected virtual void Dispatch(PopResult result)
        {
            PopDispatcher.Dispatch(handler: this, result: result);
        }
    }
}