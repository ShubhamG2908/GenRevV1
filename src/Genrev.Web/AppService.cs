using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;


namespace Genrev.Web
{

    
    public class AppService
    {

        public Types.ViewContext ViewContext { get; set; }
        public Types.CurrentUser User { get; set; }
        public Types.CurrentPerson Person { get; set; }
        public Types.Settings Settings { get; set; }
        
        public AppService() {
            ViewContext = new Types.ViewContext(this);
            User = new Types.CurrentUser(this);
            Person = new Types.CurrentPerson(this);
            Settings = new Types.Settings(this);
        }


        /// <summary>
        ///  Get the current user's AppService instance
        ///  Pull from cache or create new and place in cache
        /// </summary>
        public static AppService Current {
            get
            {
                var username = HttpContext.Current.User.Identity.Name;
                var cacheKey = "AppService_" + username;
                AppService current = AppCache.GetItem(cacheKey) as AppService;
                if (current == null) {
                    current = new AppService();
                    AppCache.AddItem(cacheKey, current);
                }
                return current;
            }
        }

        /// <summary>
        /// Removes the current user's AppService from the cache
        /// </summary>
        public void Invalidate() {
            var username = HttpContext.Current.User.Identity.Name;
            var cacheKey = "AppService_" + username;
            AppCache.InvalidateItem(cacheKey);
        }
                
        public Data.GenrevContext DataContext {
            get
            {
                // data context, new context per request
                if (!HttpContext.Current.Items.Contains("DataContext")) {
                    HttpContext.Current.Items.Add(
                        "DataContext",
                        new Data.GenrevContext());
                }

                return HttpContext.Current.Items["DataContext"] as Data.GenrevContext;
                
            }
        }

        public Domain.Accounts.Account Account {
            get
            {
                return User.User.Account;
            }
        }

        public bool IsSysAdmin()
        {
            return Person.Person.Roles.Any(x => x.IsSysAdministrator);
        }


        public class Types
        {

            public class ViewContext
            {
                
                AppService appService;
                int? _personID = null;

                public ViewContext(AppService appService) {
                    this.appService = appService;
                }

                public int PersonID {
                    get
                    {
                        if (_personID == null) {
                            PersonID = appService.Person.PersonID;
                        }
                        return _personID.Value;
                    }
                    set
                    {
                        _personID = value;
                        // invalidate cache, recalc whatever needs to be recalc'd
                    }
                }

                public Domain.Companies.Person Person {
                    get
                    {
                        return AppService.Current.DataContext.Personnel.Find(PersonID);
                    }
                }

                public List<Domain.Companies.Person> Personnel {
                    get
                    {
                        return AppService.Current.DataContext.GetDownstreamPersonnel(AppService.Current.ViewContext.PersonID).ToList();
                    }
                }

                public int[] PersonnelIDs {
                    get
                    {
                        return AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                    }
                }




                public int[] CustomerIDs {
                    get
                    {
                        return AppService.Current.DataContext.GetDownstreamCustomerIDs(PersonID);
                    }
                }

                public System.Data.DataTable CustomerIDsTable {
                    get
                    {
                        return Data.DTOs.CustomerIDsTable.ToDataTable(AppService.Current.ViewContext.PersonnelIDs);
                    }
                }
            }
            
            public class CurrentUser
            {

                private AppService appService;

                public CurrentUser(AppService appService) {
                    this.appService = appService;
                }

                public bool HasRole(string roleName) {
                    return HttpContext.Current.User.IsInRole(roleName);
                }

                public string UserName {
                    get
                    {
                        return HttpContext.Current.User.Identity.Name;
                    }
                }

                public int UserID {
                    get
                    {
                        return User.ID;
                    }
                }

                public Domain.Users.User User {
                    get
                    {
                        try {
                            return AppService.Current.DataContext.Users.Where(x => x.Email == UserName).Single();
                        }
                        catch {
                            return new Data.GenrevContext().Users.Where(x => x.Email == UserName).Single();
                        }

                    }
                }

            }

            public class CurrentPerson
            {

                private AppService appService;

                public CurrentPerson(AppService appService) {
                    this.appService = appService;
                }

                public int PersonID {
                    get
                    {
                        return appService.User.User.PersonnelID.Value;
                    }
                }

                public Domain.Companies.Person Person {
                    get
                    {
                        return AppService.Current.DataContext.Personnel.Find(appService.User.User.PersonnelID.Value);
                    }
                }

                public List<Domain.Companies.Person> Personnel {
                    get
                    {
                        return AppService.Current.DataContext.GetDownstreamPersonnel(PersonID).ToList();
                    }
                }

            }

            public class Settings
            {
                private AppService appService;

                public Settings(AppService appService)
                {
                    this.appService = appService;
                    TargetFeatureEnabled = bool.Parse(ConfigurationManager.AppSettings["TargetFeatureEnabled"]);
                    ProductFeatureEnabled = bool.Parse(ConfigurationManager.AppSettings["ProductFeatureEnabled"]);
                    FeedbackEmails = ConfigurationManager.AppSettings["FeedbackEmails"].Split(new String[1]{", "}, StringSplitOptions.None);

                    SMTPAccount = new Domain.Email.SMTPAccount()
                    {
                        Username = ConfigurationManager.AppSettings["SMTPAccountUserName"],
                        Password = ConfigurationManager.AppSettings["SMTPAccountPassword"],
                        Server = ConfigurationManager.AppSettings["SMTPAccountServer"],
                        Port = int.Parse(ConfigurationManager.AppSettings["SMTPAccountPort"]),
                        UseSSL = bool.Parse(ConfigurationManager.AppSettings["SMTPAccountUseSSL"]),
                        FromAddress = ConfigurationManager.AppSettings["SMTPAccountFromAddress"]
                    };

                }

                public bool TargetFeatureEnabled { get; set; }
                public bool ProductFeatureEnabled { get; set; }
                public string[] FeedbackEmails { get; set; }
                public Domain.Email.SMTPAccount SMTPAccount { get; set; }

                public bool DisplayDetailedErrors {  get
                    {

#if DEBUG
                        return true;
#else
                        var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/Web.config");
                        var customErrorsSection = (System.Web.Configuration.CustomErrorsSection)config.GetSection("system.web/customErrors");
                        if (customErrorsSection.Mode == System.Web.Configuration.CustomErrorsMode.Off) {
                            return true;
                        } else {
                            return false;
                        }
#endif


                    }
                }

                public string FileUploadDirectory {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["FileUploadDirectory"];
                    }
                }

            }
            

        }

        

        internal void LogException(Exception e) {
            throw new NotImplementedException();
        }
    }
}