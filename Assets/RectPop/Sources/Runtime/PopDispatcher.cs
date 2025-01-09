using System;
using UnityEngine;

namespace RectPop
{
    public static class PopDispatcher
    {
        public static event Action<PopDispatchedEvent> OnDispatched;

        public static void Dispatch(IPopHandler handler, PopResult result)
        {
            if (handler is null)
            {
                Debug.LogError($"{nameof(handler)} is null.");
                return;
            }

            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

            OnDispatched?.Invoke(new PopDispatchedEvent(handler, result));
        }
    }
}