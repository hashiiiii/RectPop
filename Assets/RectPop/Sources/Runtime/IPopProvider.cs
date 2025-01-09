namespace RectPop
{
    public interface IPopProvider
    {
        PopResult Provide(PopRequest request);
    }
}