using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RectPop.Examples.Sources
{
    public class PopupExample : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Vector2 _offset;
        [SerializeField] LayoutType _layoutType;
        [SerializeField] float _centerThreshold;
        
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private List<Button> _buttons;
        [SerializeField] private RectTransform _popupRect;
        [SerializeField] private Canvas _popupCanvas;
        [SerializeField] private Button _transparentButton;

        private void Awake()
        {
            SetActive(false);

            _transparentButton.onClick.AddListener(() =>
            {
                SetActive(false);
            });

            foreach (var button in _buttons)
            {
                button.onClick.AddListener(() =>
                {
                    var rectTransform = button.GetComponent<RectTransform>();

                    var request = new LayoutRequest(
                        layoutRectTransform: rectTransform,
                        layoutType: _layoutType,
                        offset: _offset,
                        centerThreshold: _centerThreshold,
                        layoutCamera: GetCamera()
                    );

                    RectPopUtility.ProvideAndApply(request: request, layoutRect: _popupRect, layoutCanvas: _popupCanvas);
                    SetActive(true);
                });
            }

            return;

            void SetActive(bool active)
            {
                _popupRect.gameObject.SetActive(active);
                _transparentButton.gameObject.SetActive(active);
            }

            Camera GetCamera()
            {
                return _popupCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera;
            }
        }
    }
}