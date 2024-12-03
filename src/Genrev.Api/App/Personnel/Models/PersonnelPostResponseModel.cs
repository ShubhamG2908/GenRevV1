using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Personnel.Models
{

    [XmlRoot(ElementName = "Response")]
    public class PersonnelPostResponseModel : ResponseModelBase
    {

        public PersonnelPostResponseModel() : base() { }
        public PersonnelPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public PersonnelPostResponseModel(int code) : base(code) { }
        public PersonnelPostResponseModel(int code, string message) : base(code, message) { }


        public string Message { get; set; }

    }
}