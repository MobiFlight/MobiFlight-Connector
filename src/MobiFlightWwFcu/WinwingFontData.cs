namespace MobiFlightWwFcu
{
    internal class WinwingFontData
    {
        public WinwingFontConfig largeFontConfig { get; set; }
        public WinwingFontConfig smallFontConfig { get; set; }
    }

    internal class WinwingFontConfig
    {
        public WinwingFontHeadConfig headConfig { get; set; }      
        public byte[] pixelUint8Array { get; set; }
    }

    internal class WinwingFontHeadConfig
    {
        public int id { get; set; }
        public short matrixW { get; set; }
        public short matrixH { get; set; }
        public int glyphSize { get; set; }
        public int charSize { get; set; }
        public int size { get; set; }
        public int version { get; set; }
        public byte isWriteFlash { get; set; }
    }
}
