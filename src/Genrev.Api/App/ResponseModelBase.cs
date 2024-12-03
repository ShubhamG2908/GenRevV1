using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Net;

namespace Genrev.Api.App
{
    
    public abstract class ResponseModelBase
    {

        public ResponseModelBase() { }
        public ResponseModelBase(HttpStatusCode status) { constructViaHttpStatus(status); }
        public ResponseModelBase(int code) { construct(code, null); }
        public ResponseModelBase(int code, string message) { construct(code, message); }

        private void construct(int code, string message) {
            SetMessage(code, message);
        }

        private void constructViaHttpStatus(HttpStatusCode status) {
            SetMessage((int)status, status.ToString());
        }

        public void SetMessage(int code, string message) {
            Status = new ResponseBaseStatus()
            {
                Code = code,
                Message = message
            };
        }
        
        public ResponseBaseStatus Status { get; set; }


    }

    public class ResponseBaseStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

}