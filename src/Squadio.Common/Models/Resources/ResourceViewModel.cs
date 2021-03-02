using System;
using Squadio.Common.Settings;
using Squadio.Common.Settings.Static;
using Squadio.Domain.Models.Resources;

namespace Squadio.Common.Models.Resources
{
    public class ResourceViewModel
    {
        public Guid ResourceId { get; set; }
        protected string _group;
        public string Group
        {
            get => _group;
            set { _group = value; SetUrls(); }
        }
        
        protected string _filename;
        public string Filename
        {
            get => _filename;
            set { _filename = value; SetUrls(); }
        }

        protected string _templateUrl;
        /// <summary>
        /// Template for URLs to Files with all allowed variables
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
        /// </summary>
        public string TemplateUrl
        {
            get => _templateUrl;
            set { _templateUrl = value; SetTemplate(_templateUrl); }
        }

        public ResourceViewModel()
        {
            SetTemplate(PathTemplates.FilePathTemplate);
        }

        /// <summary>
        /// Constructor with setting URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
        /// </summary>
        public ResourceViewModel(ResourceModel resource)
        {
            if (resource != null)
            {
                ResourceId = resource.Id;
                _group = resource.Group;
                _filename = resource.FileName;
            }
            SetTemplate(PathTemplates.FilePathTemplate);
        }

        public string OriginalUrl { get; protected set; }

        /// <summary>
        /// Set URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
        /// </summary>
        public void SetTemplate(string templateUrl)
        {
            _templateUrl = templateUrl;
            SetUrls();
        }

        protected void SetUrls()
        {
            if(_templateUrl == null)
                return;
            
            if (_group != null && _filename != null)
            {
                var templateGroup = _templateUrl.Replace("{Group}", _group);
                OriginalUrl = templateGroup.Replace("{Filename}", _filename);
            }
        }
    }
}