namespace Squadio.Common.Settings.Static
{
    public static class CropSizesSettings
    {
        public static string SizesStr { get; private set; }

        public static int[] Sizes { get; private set; }

        public static void SetSizes(string sizesStr)
        {
            if (SizesStr == null)
                SizesStr = sizesStr;
            
            ParseSizes();
        }

        private static void ParseSizes()
        {
            var sizes = SizesStr.Split(',');
            if(sizes?.Length == 0)
                return;
            
            Sizes = new int[sizes.Length];
            for (var i = 0; i < sizes.Length; i++)
            {
                Sizes[i] = int.Parse(sizes[i]);
            }
        }
    }
}