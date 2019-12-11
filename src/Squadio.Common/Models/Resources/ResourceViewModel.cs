using System;
using Squadio.Domain.Models.Resources;

namespace Squadio.Common.Models.Resources
{
    public class ResourceViewModel
    {
        private string _group;
        public string Group
        {
            get => _group;
            set { _group = value; SetUrls(); }
        }
        
        private string _filename;
        public string Filename
        {
            get => _filename;
            set { _filename = value; SetUrls(); }
        }

        private string _templateUrl;
        /// <summary>
        /// Template for URLs to Files with all allowed variables
        /// <para>Example: http://127.0.0.1/{Group}/{Resolution}/{Filename}</para>
        /// </summary>
        public string TemplateUrl
        {
            get => _templateUrl;
            set { _templateUrl = value; SetTemplate(_templateUrl); }
        }
        private const string _defaultTemplateUrl = "https://localhost:5005/{Group}/{Resolution}/{Filename}";

        public ResourceViewModel()
        {
            SetTemplate(null);
        }

        /// <summary>
        /// Constructor with setting URLs by specified template
        /// <para>Example: http://127.0.0.1/{Group}/{Bucket}/{Resolution}/{Filename}</para>
        /// </summary>
        public ResourceViewModel(string templateUrl, ResourceModel resource = null)
        {
            if (resource != null)
            {
                _group = resource.Group;
                _filename = resource.FileName;
            }
            SetTemplate(templateUrl);
        }

        public string OriginalUrl { get; private set; }
        public string Url140 { get; private set; }
        public string Url360 { get; private set; }
        public string Url480 { get; private set; }
        public string Url720 { get; private set; }
        public string Url1080 { get; private set; }

        /// <summary>
        /// Set URLs by specified template
        /// <para>Example: http://127.0.0.1/{Group}/{Date}/{Resolution}/{Filename}</para>
        /// </summary>
        public void SetTemplate(string templateUrl)
        {
            _templateUrl = string.IsNullOrEmpty(templateUrl) 
                ? _defaultTemplateUrl
                : templateUrl;
            SetUrls();
        }

        private void SetUrls()
        {
            if (_templateUrl == null)
            {
                _templateUrl = _defaultTemplateUrl;
            }
            if (_group != null && _filename != null)
            {
                var templateGroup = _templateUrl.Replace("{Group}", _group);
                var templateFileName = templateGroup.Replace("{Filename}", _filename);

                OriginalUrl = templateFileName.Replace("{Resolution}", "original");
                Url140 = templateFileName.Replace("{Resolution}", "140");
                Url360 = templateFileName.Replace("{Resolution}", "360");
                Url480 = templateFileName.Replace("{Resolution}", "480");
                Url720 = templateFileName.Replace("{Resolution}", "720");
                Url1080 = templateFileName.Replace("{Resolution}", "1080");
            }
            else
            {
                if (_templateUrl == _defaultTemplateUrl)
                    _templateUrl = null;
            }
        }
    }
}