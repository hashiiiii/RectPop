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
            "RectPop is a Unity library for floating UIs, including popovers, tooltips, and context menus.",
            "RectPop is a powerful Unity library for creating popovers, tooltips, and context menus with floating UIs. It automatically adjusts UI placement within screen bounds and supports all Canvas render modes.",
            "RectPop is a Unity tool for popovers, tooltips, and context menus. By requiring only a RectTransform and Canvas, it streamlines floating UI creation and keeps content visible on any screen. Custom offsets and multiple render modes are supported, making it ideal for dynamic, device-agnostic setups.",
            "RectPop is a Unity solution for floating UIs like popovers, tooltips, and context menus. It only requires a RectTransform and Canvas, automatically calculating pivots, anchors to keep UIs visible. You can unify multiple floating UIs under one system, regardless of how many objects request them. It also supports all canvas render modes and dynamic resolution changes, ensuring consistency across devices."
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
            if (ev.Result.Context is not int index)
            {
                Debug.LogError($"{nameof(ev.Result.Context)} is not int.");
                return;
            }

            _handler.Apply(ev.Result, _floatingRect, _floatingCanvas);

            _floatingText.text = _textList[index];

            SetActive(true);
        }

        private void SetActive(bool active)
        {
            _floatingRect.gameObject.SetActive(active);
            _transparentButton.gameObject.SetActive(active);
        }
    }
}