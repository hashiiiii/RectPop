using System;
using UnityEngine;

namespace RectPop
{
    public class PopDispatcher : IPopDispatcher
    {
        public static event Action<PopDispatchedEvent> OnDispatched;

        public void Dispatch(PopResult result)
        {
            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

            var ev = new PopDispatchedEvent(result);

            OnDispatched?.Invoke(ev);
        }
    }
}