using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace Genrev.Api.App.Classifications.Models
{

    [XmlRoot(ElementName = "Request")]
    public class CustomerTypePostRequestModel
    {
        public List<CustomerType> CustomerTypes { get; set; }
    }

    public class CustomerType
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}