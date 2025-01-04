using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RectPop
{
    public class Example02Request : MonoBehaviour
    {
        [SerializeField] private Canvas _baseCanvas;
        [SerializeField] private List<Button> _buttons;

        private readonly PopController _controller = new();

        private void Awake()
        {
            foreach (var button in _buttons)
            {
                button.onClick.AddListener(() =>
                {
                    var baseRectTransform = button.GetComponent<RectTransform>();

                    var request = new PopRequest(
                        baseRectTransform: baseRectTransform,
                        baseCanvas: _baseCanvas
                    );

                    _controller.Request(request);
                });
            }
        }
    }
}