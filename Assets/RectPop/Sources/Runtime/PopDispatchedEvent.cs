namespace RectPop
{
    public class PopDispatchedEvent
    {
        public PopResult Result { get; }

        public PopDispatchedEvent(PopResult result)
        {
            Result = result;
        }
    }
}