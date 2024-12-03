using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Classifications.Models
{

    [XmlRoot(ElementName = "Response")]
    public class CustomerTypesPostResponseModel : ResponseModelBase
    {

        public CustomerTypesPostResponseModel() : base() { }
        public CustomerTypesPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public CustomerTypesPostResponseModel(int code) : base(code) { }
        public CustomerTypesPostResponseModel(int code, string message) : base(code, message) { }
        
        public string Message { get; set; }

    }
}