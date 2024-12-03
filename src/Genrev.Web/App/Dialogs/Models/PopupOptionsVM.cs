using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Dialogs.Models
{
    public class PopupOptionsVM
    {

        public string ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        public bool AllowDrag { get; set; }
        public bool AllowResize { get; set; }
        
    }
}