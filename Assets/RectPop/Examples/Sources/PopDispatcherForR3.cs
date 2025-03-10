﻿#if RECTPOP_R3
using R3;
using UnityEngine;

namespace RectPop
{
    public class PopDispatcherForR3 : IPopDispatcher
    {
        private static readonly Subject<PopDispatchedEvent> R3Subject = new();
        public static Observable<PopDispatchedEvent> OnDispatchedByR3AsObservable => R3Subject.AsObservable();

        public void Dispatch(PopResult result)
        {
            if (result is null)
            {
                Debug.LogError($"{nameof(result)} is null.");
                return;
            }

            var ev = new PopDispatchedEvent(result);

            R3Subject.OnNext(ev);
        }
    }
}
#endif