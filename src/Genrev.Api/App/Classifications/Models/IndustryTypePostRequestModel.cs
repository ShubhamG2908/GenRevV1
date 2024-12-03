using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Classifications.Models
{

    [XmlRoot(ElementName = "Request")]
    public class IndustryTypePostRequestModel
    {
        public List<IndustryType> IndustryTypes { get; set; }
    }

    public class IndustryType
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}