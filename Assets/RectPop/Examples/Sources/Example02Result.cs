using UnityEngine;
using UnityEngine.UI;

namespace RectPop
{
    public class Example02Result : MonoBehaviour
    {
        [SerializeField] private RectTransform _floatingRect;
        [SerializeField] private Canvas _floatingCanvas;
        [SerializeField] private Button _transparentButton;

        private readonly PopController _controller = new();

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

            if (ev?.Source is not PopController)
            {
                Debug.LogError($"{nameof(ev.Source)} is not {nameof(PopController)}.");
                return;
            }

            if (ev.Result is null)
            {
                Debug.LogError($"{nameof(ev.Result)} is null.");
                return;
            }

            _controller.Apply(ev.Result, _floatingRect, _floatingCanvas);

            SetActive(true);
        }

        private void SetActive(bool active)
        {
            _floatingRect.gameObject.SetActive(active);
            _transparentButton.gameObject.SetActive(active);
        }
    }
}