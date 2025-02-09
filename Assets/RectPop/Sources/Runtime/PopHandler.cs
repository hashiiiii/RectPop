using UnityEngine;

namespace RectPop
{
    public class PopHandler : IPopHandler
    {
        private static readonly IPopProvider DefaultProvider = new PopProvider();
        private static readonly IPopDispatcher DefaultDispatcher = new PopDispatcher();

        private readonly IPopProvider _provider;
        private readonly IPopDispatcher _dispatcher;

        public PopHandler(IPopProvider provider, IPopDispatcher dispatcher)
        {
            _provider = provider;
            _dispatcher = dispatcher;
        }

        public PopHandler(IPopDispatcher dispatcher) : this(provider: DefaultProvider, dispatcher: dispatcher)
        {
        }

        public PopHandler(IPopProvider provider) : this(provider: provider, dispatcher: DefaultDispatcher)
        {
        }

        public PopHandler() : this(provider: DefaultProvider, dispatcher: DefaultDispatcher)
        {
        }

        public virtual PopResult Request(PopRequest request)
        {
            if (request is null)
            {
                Debug.LogError($"{nameof(request)} is null.");
                return null;
            }

            var result = _provider.Provide(request);
            if (result is null)
            {
                Debug.LogError($"Failed to get {nameof(PopResult)}");
                return null;
            }

            _dispatcher.Dispatch(result);

            return result;
        }
    }
}