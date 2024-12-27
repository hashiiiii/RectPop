namespace RectPop
{
    /// <summary>
    /// defines the layout type for the UI element relative to the target RectTransform.
    /// </summary>
    public enum LayoutType
    {
        /// <summary> places the layout inside the target's bounding box. </summary>
        Inside,

        /// <summary> places the layout outside - vertical the target's bounding box. </summary>
        /// <remarks> recommended for "Portrait" display. </remarks>
        OutsideVertical,

        /// <summary> places the layout outside - horizontal the target's bounding box. </summary>
        /// <remarks> recommended for "Landscape" display. </remarks>
        OutsideHorizontal,
    }
}