namespace RectPop
{
    public interface ILayoutProvider
    {
        LayoutResult Provide(LayoutRequest request);
    }
}