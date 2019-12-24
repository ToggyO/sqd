namespace Squadio.Common.Settings
{
    public class CropSizesModel
    {
        private string _sizesStr;
        public string SizesStr 
        { 
            get => _sizesStr;
            set { _sizesStr = value; ParseSizes(); } 
        }
        
        private int[] _sizes;
        public int[] Sizes
        {
            get => _sizes;
        }

        private void ParseSizes()
        {
            var sizes = _sizesStr.Split(',');
            if(sizes == null || sizes?.Length == 0)
                return;
            
            _sizes = new int[sizes.Length];
            for (int i = 0; i < sizes.Length; i++)
            {
                _sizes[i] = int.Parse(sizes[i]);
            }
        }
    }
}