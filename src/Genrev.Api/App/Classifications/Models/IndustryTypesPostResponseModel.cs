using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;


namespace Genrev.Api.App.Classifications.Models
{
    [XmlRoot(ElementName = "Response")]
    public class IndustryTypesPostResponseModel : ResponseModelBase
    {


        public IndustryTypesPostResponseModel() : base() { }
        public IndustryTypesPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public IndustryTypesPostResponseModel(int code) : base(code) { }
        public IndustryTypesPostResponseModel(int code, string message) : base(code, message) { }


        public string Message { get; set; }


    }
}
