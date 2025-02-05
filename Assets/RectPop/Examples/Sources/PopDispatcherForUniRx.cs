#if RECTPOP_UNIRX
using System;
using UniRx;
using UnityEngine;

namespace RectPop
{
    public class PopDispatcherForUniRx : IPopDispatcher
    {
        private static readonly Subject<PopDispatchedEvent> UniRxSubject = new();
        public static IObservable<PopDispatchedEvent> OnDispatchedByUniRxAsObservable => UniRxSubject.AsObservable();

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

            UniRxSubject.OnNext(ev);
        }
    }
}
#endif