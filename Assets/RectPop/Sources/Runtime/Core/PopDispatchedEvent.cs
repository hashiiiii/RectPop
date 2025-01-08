namespace RectPop
{
    public class PopDispatchedEvent
    {
        public IPopHandler Handler { get; }
        public PopResult Result { get; }

        public PopDispatchedEvent(IPopHandler handler, PopResult result)
        {
            Handler = handler;
            Result = result;
        }
    }
}