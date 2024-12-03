using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;


namespace Genrev.Api.App.MonthlyData.Models
{
    [XmlRoot(ElementName = "Response")]
    public class MonthlyDataPostResponseModel : ResponseModelBase
    {

        public MonthlyDataPostResponseModel() : base() { }
        public MonthlyDataPostResponseModel(System.Net.HttpStatusCode status) : base(status) { }
        public MonthlyDataPostResponseModel(int code) : base(code) { }
        public MonthlyDataPostResponseModel(int code, string message) : base(code, message) { }


        public string Message { get; set; }

    }
}