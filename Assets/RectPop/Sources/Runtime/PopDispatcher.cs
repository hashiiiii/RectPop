﻿using System;
using UnityEngine;

namespace RectPop
{
    public class PopDispatcher : IPopDispatcher
    {
        public static event Action<PopDispatchedEvent> OnDispatched;

        public void Dispatch(IPopHandler handler, PopResult result)
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

            var ev = new PopDispatchedEvent(handler, result);

            OnDispatched?.Invoke(ev);
        }
    }
}