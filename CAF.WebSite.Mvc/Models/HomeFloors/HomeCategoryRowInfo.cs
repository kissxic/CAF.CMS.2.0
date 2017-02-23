using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Runtime.CompilerServices;

namespace CAF.WebSite.Mvc.Models.HomeFloors
{
    public class HomeCategoryRowInfo : ModelBase
    {


        public int Id { get; set; }


        private string image1 { get; set; }


        public string Image1 { get; set; }


        private string image2 { get; set; }

        public string Image2 { get; set; }


        public int RowId { get; set; }

        public string Url1 { get; set; }


        public string Url2 { get; set; }


        public HomeCategoryRowInfo()
        {
        }
    }
}