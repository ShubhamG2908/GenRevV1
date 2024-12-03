using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Classifications.Models
{
    [XmlRoot(ElementName = "Response")]
    public class AccountTypesPostResponseModel : ResponseModelBase
    {

        public AccountTypesPostResponseModel() : base() { }
        public AccountTypesPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public AccountTypesPostResponseModel(int code) : base(code) { }
        public AccountTypesPostResponseModel(int code, string message) : base(code, message) { }

        public string Message { get; set; }

    }
}