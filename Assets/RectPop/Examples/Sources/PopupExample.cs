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
        [SerializeField] private Canvas _baseCanvas;
        [SerializeField] private List<Button> _baseButtons;
        [SerializeField] private RectTransform _popupRect;
        [SerializeField] private Canvas _popupCanvas;
        [SerializeField] private Button _transparentButton;

        private readonly RectPopController _controller = new();

        private void Awake()
        {
            SetActive(false);

            _transparentButton.onClick.AddListener(() =>
            {
                SetActive(false);
            });

            foreach (var button in _baseButtons)
            {
                button.onClick.AddListener(() =>
                {
                    var baseRectTransform = button.GetComponent<RectTransform>();

                    var request = new LayoutRequest(
                        baseRectTransform: baseRectTransform,
                        baseCanvas: _baseCanvas,
                        layoutType: _layoutType,
                        offset: _offset,
                        centerThreshold: _centerThreshold
                    );

                    _controller.ProvideAndApply(request, _popupRect, _popupCanvas);

                    SetActive(true);
                });
            }

            return;

            void SetActive(bool active)
            {
                _popupRect.gameObject.SetActive(active);
                _transparentButton.gameObject.SetActive(active);
            }
        }
    }
}