using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RectPop
{
    public class Example02Result : MonoBehaviour
    {
        [SerializeField] private RectTransform _floatingRect;
        [SerializeField] private Text _floatingText;
        [SerializeField] private Canvas _floatingCanvas;
        [SerializeField] private Button _transparentButton;

        private readonly PopHandler _handler = new();
        private readonly List<string> _textList = new()
        {
            "RectPop enables effortless creation of floating UIs like tooltips and menus, ensuring dynamic positioning, screen visibility, and compatibility across devices and resolutions.",
            "RectPop simplifies the implementation of floating UIs, such as tooltips, context menus, and popovers. Its dynamic positioning ensures that UI elements remain visible on screen, even on devices with varying resolutions. By handling requests from multiple objects seamlessly, RectPop unifies UI management, offering developers a robust and flexible solution for interactive applications",
        }; 

        private void Awake()
        {
            PopDispatcher.OnDispatched += OnPopDispatched;

            SetActive(false);

            _transparentButton.onClick.AddListener(() => SetActive(false));
        }

        private void OnDestroy()
        {
            PopDispatcher.OnDispatched -= OnPopDispatched;
        }

        private void OnPopDispatched(PopDispatchedEvent ev)
        {
            if (_floatingRect is null)
            {
                Debug.LogWarning($"{nameof(_floatingRect)} is not set.");
                return;
            }

            if (_floatingCanvas is null)
            {
                Debug.LogWarning($"{nameof(_floatingCanvas)} is not set.");
                return;
            }

            if (ev?.Handler is not PopHandler)
            {
                Debug.LogError($"{nameof(ev.Handler)} is not {nameof(PopHandler)}.");
                return;
            }

            if (ev.Result is null)
            {
                Debug.LogError($"{nameof(ev.Result)} is null.");
                return;
            }

            if (ev.Result.Context is not int index)
            {
                Debug.LogError($"{nameof(ev.Result.Context)} is not int.");
                return;
            }

            _handler.Apply(ev.Result, _floatingRect, _floatingCanvas);

            var text = _textList[index];
            _floatingText.text = $"Button {index + 1} clicked. {text}";

            SetActive(true);
        }

        private void SetActive(bool active)
        {
            _floatingRect.gameObject.SetActive(active);
            _transparentButton.gameObject.SetActive(active);
        }
    }
}