using System;
using UnityEngine;

#if RECTPOP_R3
using R3;
#endif

#if RECTPOP_UNIRX
using UniRx;
#endif

namespace RectPop
{
    public static class PopDispatcher
    {
#if RECTPOP_R3
        private static readonly R3.Subject<PopDispatchedEvent> R3Subject = new();
        public static Observable<PopDispatchedEvent> OnDispatchedByR3AsObservable => R3Subject.AsObservable();
#endif

#if RECTPOP_UNIRX
        private static readonly UniRx.Subject<PopDispatchedEvent> UniRxSubject = new();
        public static IObservable<PopDispatchedEvent> OnDispatchedByUniRxAsObservable => UniRxSubject.AsObservable();
#endif

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

            var ev = new PopDispatchedEvent(handler, result);

            OnDispatched?.Invoke(ev);

#if RECTPOP_R3
            R3Subject.OnNext(ev);
#endif

#if RECTPOP_UNIRX
            UniRxSubject.OnNext(ev);
#endif
        }
    }
}