namespace Squadio.Common.Settings
{
    public static class PathTemplates
    {
        static PathTemplates()
        {
            ImagePathTemplate = null;
            FilePathTemplate = null;
        }
        
        public static string ImagePathTemplate { get; private set; }
        public static string FilePathTemplate { get; private set; }

        public static void SetImagePathTemplate(string newPathTemplate)
        {
            ImagePathTemplate ??= newPathTemplate;
        }
        public static void SetFilePathTemplate(string newPathTemplate)
        {
            FilePathTemplate ??= newPathTemplate;
        }
    }
}