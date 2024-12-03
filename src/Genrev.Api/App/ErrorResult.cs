using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App
{

    [XmlRoot(ElementName = "Response")]
    public class ErrorResult
    {

        public ErrorResult() {
            Errors = new List<Error>();
        }

        public ErrorResult(string code, string message) {
            Errors = new List<Error>();
            AddError(code, message);
        }

        public List<Error> Errors { get; set; }

        public void AddError(string code, string message) {
            if (Errors == null) {
                Errors = new List<Error>();
            }
            Errors.Add(new Error() { Code = code, Message = message, Timestamp = DateTime.UtcNow });
        }



        public class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }

    }
    

    

}