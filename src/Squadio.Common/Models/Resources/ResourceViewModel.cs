﻿using System;
using Squadio.Domain.Models.Resources;

namespace Squadio.Common.Models.Resources
{
    public class ResourceViewModel
    {
        public Guid Id { get; set; }
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
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
        /// </summary>
        public string TemplateUrl
        {
            get => _templateUrl;
            set { _templateUrl = value; SetTemplate(_templateUrl); }
        }
        private const string _defaultTemplateUrl = "http://localhost:5005/api/files/{Group}/{Filename}";

        public ResourceViewModel()
        {
            SetTemplate(null);
        }

        /// <summary>
        /// Constructor with setting URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
        /// </summary>
        public ResourceViewModel(ResourceModel resource, string templateUrl = null)
        {
            if (resource != null)
            {
                Id = resource.Id;
                _group = resource.Group;
                _filename = resource.FileName;
            }
            SetTemplate(templateUrl);
        }

        public string OriginalUrl { get; private set; }

        /// <summary>
        /// Set URLs by specified template
        /// <para>Example: https://127.0.0.1/api/files/{Group}/{Filename}</para>
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
                OriginalUrl = templateGroup.Replace("{Filename}", _filename);
            }
            else
            {
                if (_templateUrl == _defaultTemplateUrl)
                    _templateUrl = null;
            }
        }
    }
}