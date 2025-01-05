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
            for (var i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                var index = i;

                button.onClick.AddListener(() =>
                {
                    var baseRectTransform = button.GetComponent<RectTransform>();

                    var request = new PopRequest(baseRectTransform, _baseCanvas, context: index);

                    _controller.Request(request);
                });
            }
        }
    }
}