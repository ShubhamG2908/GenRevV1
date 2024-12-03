using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;


namespace Genrev.Api.App.Customers.Models
{

    [XmlRoot(ElementName = "Response")]
    public class CustomerPostResponseModel : ResponseModelBase
    {

        public CustomerPostResponseModel() : base() { }
        public CustomerPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public CustomerPostResponseModel(int code) : base(code) { }
        public CustomerPostResponseModel(int code, string message) : base(code, message) { }


        public string Message { get; set; }

    }

}