using UnityEngine;

namespace RectPop
{
    public static class RectPopUtility
    {
        private static LayoutProvider _layoutProvider = new();

        /// <summary>
        /// sets the layout provider.
        /// </summary>
        /// <param name="layoutProvider"> layout provider to set. </param>
        public static void SetLayoutProvider(LayoutProvider layoutProvider)
        {
            _layoutProvider = layoutProvider ?? new LayoutProvider();
        }

        /// <summary>
        /// resets the layout provider to a new instance.
        /// </summary>
        public static void ResetLayoutProvider()
        {
            _layoutProvider = new LayoutProvider();
        }

        /// <summary>
        /// provides a layout result based on the given layout request.
        /// </summary>
        /// <param name="request"> layout request containing the necessary parameters. </param>
        /// <returns> LayoutResult object containing the layout information. </returns>
        public static LayoutResult Provide(LayoutRequest request)
        {
            return _layoutProvider.Provide(request);
        }

        /// <summary>
        /// A simple approach to provide and apply layout to the specified RectTransform and Canvas. 
        /// This method forcefully changes the pivot/anchor settings of both the target RectTransform 
        /// (<paramref name="layoutRect"/>) and its parent RectTransform, so it may overwrite any existing layout settings.
        /// </summary>
        /// <param name="request">Layout request containing all necessary parameters.</param>
        /// <param name="layoutRect">
        ///   The RectTransform that will be positioned as a popup. 
        ///   Note that its pivot and anchorMin/anchorMax will be overwritten.
        /// </param>
        /// <param name="layoutCanvas">
        ///   The Canvas on which the popup is displayed. 
        ///   If <see cref="Canvas.renderMode"/> is ScreenSpaceOverlay, the camera is ignored;
        ///   otherwise, <see cref="Canvas.worldCamera"/> is used.
        /// </param>
        public static void ProvideAndApply(LayoutRequest request, RectTransform layoutRect, Canvas layoutCanvas)
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

            var result = Provide(request);

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
    }
}