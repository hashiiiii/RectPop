using System;
using UnityEngine;

namespace RectPop
{
    public static class PopDispatcher
    {
        public static event Action<PopDispatchedEvent> OnDispatched;

        public static void Dispatch(IPopSource source, PopResult result)
        {
            if (source is null)
            {
                Debug.LogError($"{nameof(source)} is null.");
                return;
            }

            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

            OnDispatched?.Invoke(new PopDispatchedEvent(source, result));
        }
    }
}