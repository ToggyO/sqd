namespace Squadio.Common.Settings
{
    public class CropSizesModel
    {
        private string _CropSizes;
        public string CropSizes 
        { 
            get => _CropSizes;
            set { _CropSizes = value; ParseSizes(); } 
        }
        
        private int[] _Sizes;
        public int[] Sizes { get; }

        private void ParseSizes()
        {
            var sizes = _CropSizes.Split(',');
            if(sizes == null || sizes?.Length == 0)
                return;
            
            _Sizes = new int[sizes.Length];
            for (int i = 0; i < sizes.Length; i++)
            {
                _Sizes[i] = int.Parse(sizes[i]);
            }
        }
    }
}