using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Token.Models
{
    [XmlRoot("Response")]
    public class TokenResponseModel
    {
        public Token Token { get; set; }
    }
    public class Token
    {
        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.AddMinutes(60);
        public string Value { get; set; }
    }



}