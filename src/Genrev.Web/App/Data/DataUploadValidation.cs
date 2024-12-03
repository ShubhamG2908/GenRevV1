using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Data
{
    public class DataUploadValidation
    {

        public static DevExpress.Web.UploadControlValidationSettings Settings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".csv" },
            MaxFileSize = 4194304            
        };


    }
}