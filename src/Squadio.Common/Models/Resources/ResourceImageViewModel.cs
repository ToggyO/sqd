using System.Collections.Generic;
using System.Linq;
using Squadio.Common.Settings;
using Squadio.Common.Settings.Static;
using Squadio.Domain.Models.Resources;

namespace Squadio.Common.Models.Resources
{
    public class ResourceImageViewModel : ResourceViewModel
    {
        public ResourceImageViewModel()
        {
            SetTemplate(PathTemplates.ImagePathTemplate);
        }

        /// <summary>
        /// Constructor with setting URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Resolution}/{Filename}</para>
        /// </summary>
        public ResourceImageViewModel(ResourceModel resource)
        {
            if (resource != null)
            {
                ResourceId = resource.Id;
                _group = resource.Group;
                _filename = resource.FileName;
            }
            SetTemplate(PathTemplates.ImagePathTemplate);
        }
        public IDictionary<string, string> FormatUrls { get; private set; }

        /// <summary>
        /// Set URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Resolution}/{Filename}</para>
        /// </summary>
        public new void SetTemplate(string templateUrl)
        {
            _templateUrl = templateUrl;
            SetUrls();
        }

        private new void SetUrls()
        {
            if(_templateUrl == null)
                return;
            
            if (_group != null && _filename != null)
            {
                var templateGroup = _templateUrl.Replace("{Group}", _group);
                var templateFileName = templateGroup.Replace("{Filename}", _filename);

                OriginalUrl = templateFileName.Replace("{Resolution}", "original");
                FormatUrls = CropSizesSettings.Sizes
                    .ToDictionary(
                        size => size.ToString(), 
                        size => templateFileName.Replace("{Resolution}", size.ToString()));;
            }
        }
    }
}