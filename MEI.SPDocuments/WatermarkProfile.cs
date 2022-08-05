namespace MEI.SPDocuments
{
    public class WatermarkProfile
    {
        public WatermarkProfile(string waterMarkMethod,
                                int? repeatX,
                                int? repeatY,
                                int? size,
                                WatermarkTextDrawStyle watermarkStyle,
                                string watermarkText)
        {
            WaterMarkMethod = waterMarkMethod;
            RepeatX = repeatX;
            RepeatY = repeatY;
            Size = size;
            WaterMarkStyle = watermarkStyle;
            WaterMarkText = watermarkText;
        }

        public string WaterMarkMethod { get; }

        public int? RepeatX { get; }

        public int? RepeatY { get; }

        public int? Size { get; }

        public WatermarkTextDrawStyle WaterMarkStyle { get; }

        public string WaterMarkText { get; }
    }
}
