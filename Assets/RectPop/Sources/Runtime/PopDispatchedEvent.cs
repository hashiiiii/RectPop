namespace RectPop
{
    public class PopDispatchedEvent
    {
        public IPopSource Source { get; }
        public PopResult Result { get; }

        public PopDispatchedEvent(IPopSource source, PopResult result)
        {
            Source = source;
            Result = result;
        }
    }
}