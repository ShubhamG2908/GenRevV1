using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Api
{
    public class AppService
    {

        private static AppService _current;

        public static AppService Current {
            get
            {
                if (_current == null) {
                    _current = new AppService();
                }
                return _current;
            }
        }



        
        public int AccountID {
            get
            {
                string auth = HttpContext.Current.Request.Headers["Authorization"];
                auth = auth.ToUpper().Replace("TOKEN ", "");
                auth = auth.Split('.')[0];
                return int.Parse(auth);
            }    
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



    }
}