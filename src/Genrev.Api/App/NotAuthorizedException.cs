using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Api
{
    public class NotAuthorizedException : Exception
    {

        public NotAuthorizedException() : base() { }

    }
}