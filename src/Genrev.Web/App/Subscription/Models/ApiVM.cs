using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Subscription.Models
{
    public class ApiVM
    {
        public bool ApiEnabled { get; set; }
        public string ApiKey { get; set; }
        public string ApiPassword { get; set; }
    }
}