namespace RectPop
{
    public interface IPopDispatcher
    {
        void Dispatch(IPopHandler handler, PopResult result);
    }
}