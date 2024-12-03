using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Products.Models
{
    public class ProductGroupListItem
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentGroupID { get; set; }
        public string ParentGroup { get; set; }

    }
}


