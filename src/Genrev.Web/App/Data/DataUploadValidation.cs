namespace Genrev.Web.App.Data
{
    public class DataUploadValidation
    {
        public static DevExpress.Web.UploadControlValidationSettings Settings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".csv" },
            MaxFileSize = 4194304
        };

        public static DevExpress.Web.UploadControlValidationSettings ForecastSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".csv", ".xls", ".xlsx", ".xlsm" },
            MaxFileSize = 10 * 1024 * 1024 
        };
        public static DevExpress.Web.UploadControlValidationSettings CRMSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".*" },
            MaxFileSize = 4194304
        };
    }
}