using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RectPop
{
    public class Demo : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Vector2 _offset;
        [SerializeField] PopType _popType;
        [SerializeField] float _centerThreshold;

        [Header("References")]
        [SerializeField] private Canvas _baseCanvas;
        [SerializeField] private List<Button> _buttons;
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

            foreach (var button in _buttons)
            {
                button.onClick.AddListener(() =>
                {
                    var baseRectTransform = button.GetComponent<RectTransform>();

                    var request = new PopRequest(
                        baseRectTransform: baseRectTransform,
                        baseCanvas: _baseCanvas,
                        popType: _popType,
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