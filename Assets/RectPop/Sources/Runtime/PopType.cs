namespace RectPop
{
    /// <summary> defines the pop type for the UI element relative to the base RectTransform. </summary>
    public enum PopType
    {
        /// <summary> places the pop inside the base bounding box. </summary>
        Inside,

        /// <summary> places the pop outside - vertical the base bounding box. </summary>
        /// <remarks> recommended for "Portrait" display. </remarks>
        OutsideVertical,

        /// <summary> places the pop outside - horizontal the base bounding box. </summary>
        /// <remarks> recommended for "Landscape" display. </remarks>
        OutsideHorizontal,
    }
}