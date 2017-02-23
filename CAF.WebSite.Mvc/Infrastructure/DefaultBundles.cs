﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using CAF.WebSite.Application.WebUI.Mvc.Bundles;

namespace CAF.WebSite.Mvc.Infrastructure
{
    public class DefaultBundles : IBundleProvider
    {
        public void RegisterBundles(BundleCollection bundles)
        {


            var MediaRootPath = "~/Content/assets/";
            /* File Upload
            -----------------------------------------------------*/
            bundles.Add(new CustomScriptBundle("~/bundles/fileupload").Include(
                    MediaRootPath + "global/plugins/jquery-file-upload/js/vendor/jquery.ui.widget.js",
                    MediaRootPath + "global/plugins/fileupload/jquery.iframe-transport.js",
                    MediaRootPath + "global/plugins/fileupload/jquery.fileupload.js",
                    MediaRootPath + "global/plugins/fileupload/jquery.fileupload-single-ui.js"));

            bundles.Add(new CustomStyleBundle("~/css/fileupload").Include(
                    MediaRootPath + "global/plugins/fileupload/jquery.fileupload-single-ui.css"));

            /* Image Gallery
        -----------------------------------------------------*/
            bundles.Add(new CustomScriptBundle("~/bundles/image-gallery").Include(
                "~/Content/image-gallery/js/blueimp-gallery.js",
                "~/Content/image-gallery/js/blueimp-gallery-indicator.js",
                "~/Scripts/jquery.elevatezoom.js",
                "~/Scripts/yaofang.smartgallery.js"));

            bundles.Add(new CustomStyleBundle("~/css/image-gallery").Include(
                "~/Content/custom/yaofang.smartgallery.css",
                "~/Content/image-gallery/css/blueimp-gallery.css",
                "~/Content/image-gallery/css/blueimp-gallery-indicator.css",
                "~/Content/image-gallery/css/blueimp-gallery-custom.css"));

        }

        public int Priority
        {
            get { return 0; }
        }
    }
}