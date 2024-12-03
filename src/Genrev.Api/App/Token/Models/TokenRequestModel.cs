using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Token.Models
{

    
    [XmlRoot(ElementName = "Request")]
    public class TokenRequestModel
    {
        public string ApiKey { get; set; }
        public string Password { get; set; }
    }
}