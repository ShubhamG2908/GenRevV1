using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Products.Models
{
    public class ProductListItem
    {

        public int ID { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int? ProductGroupID { get; set; }
        public string Group { get; set; }

    }
}