using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Web.App.Navigation.Models;
using System.Runtime.Caching;

namespace Genrev.Web.App.Navigation.Services
{
    public class NavBarService
    {

        public static NavBarMV GetNavBarVM() {


            string username = HttpContext.Current.User.Identity.Name;
            var nav = setFromCache(username);
            if (nav != null) {
                return nav;
            }

            nav = new NavBarMV();

            // build navbar here based on user permissions



            var groupHome = new NavBarGroup();
            groupHome.ID = 0;
            groupHome.Text = "Home";
            groupHome.Expanded = true;
            groupHome.Items = new List<NavBarGroupItem>();

            groupHome.Items.Add(new NavBarGroupItem()
            {
                ID = 0,
                Text = "Dashboard",
                Url = "/Home/Dashboard",
                Title = "Home - Dashboard"
                

            });





            var groupAnalysis = new NavBarGroup();
            groupAnalysis.ID = 1;
            groupAnalysis.Text = "Analysis";
            groupAnalysis.Expanded = true;
            groupAnalysis.Items = new List<NavBarGroupItem>();

            groupAnalysis.Items.Add(new NavBarGroupItem()
            {
                ID = 0,
                Text = "Actual vs. Forecast",
                Url = "/Analysis/ActualVsForecast",
                Title = "Analysis - Actual Vs Forecast"
            });

            groupAnalysis.Items.Add(new NavBarGroupItem()
            {
                ID = 1,
                Text = "Historic",
                Url = "/Analysis/Historic",
                Title = "Analysis - Historic"
            });


            groupAnalysis.Items.Add(new NavBarGroupItem()
            {
                ID = 2,
                Text = "Opportunities",
                Url = "/Analysis/Opportunities",
                Title = "Analysis - Opportunities"
            });

            groupAnalysis.Items.Add(new NavBarGroupItem()
            {
                ID = 3,
                Text = "Sales Calls",
                Url = "/Analysis/SalesCalls",
                Title = "Analysis - Sales Calls"
            });

            groupAnalysis.Items.Add(new NavBarGroupItem()
            {
                ID = 4,
                Text = "Drilldown",
                Url = "/Analysis/Drilldown",
                Title = "Analyis - Drilldown"
            });







            var groupData = new NavBarGroup();
            groupData.ID = 2;
            groupData.Text = "Data";
            groupData.Expanded = false;
            groupData.Items = new List<NavBarGroupItem>();

            if (AppService.Current.IsSysAdmin())
            {
                groupData.Items.Add(new NavBarGroupItem()
                {
                    ID = 0,
                    Text = "Data Management",
                    Url = "/Data/Management",
                    Title = "Data - Management"
                });

                groupData.Items.Add(new NavBarGroupItem()
                {
                    ID = 0,
                    Text = "Forecast Lock",
                    Url = "/Data/ForecastLock",
                    Title = "Data - Forecast Lock"
                });
            }

            groupData.Items.Add(new NavBarGroupItem()
            {
                ID = 1,
                Text = "Forecast Data",
                Url = "/Data/Forecast",
                Title = "Data - Forecast"
            });

            if (AppService.Current.IsSysAdmin())
            {
                groupData.Items.Add(new NavBarGroupItem()
                {
                    ID = 2,
                    Text = "Import Data",
                    Url = "/Data/Import",
                    Title = "Data - Import"
                });
            }





            var groupSubscription = new NavBarGroup();
            groupSubscription.ID = 3;
            groupSubscription.Text = "Account";
            groupSubscription.Expanded = false;
            groupSubscription.Items = new List<NavBarGroupItem>();


            groupSubscription.Items.Add(new NavBarGroupItem()
            {
                ID = 0,
                Text = "Your Account",
                Url = "/Subscription",
                Title = "Account - General"
            });

            groupSubscription.Items.Add(new NavBarGroupItem()
            {
                ID = 1,
                Text = "Customers",
                Url = "/Customers",
                Title = "Customer Management"
            });

            if (AppService.Current.Settings.ProductFeatureEnabled)
            {
                groupSubscription.Items.Add(new NavBarGroupItem()
                {
                    ID = 2,
                    Text = "Products",
                    Url = "/Products",
                    Title = "Product Management"
                });
            }

            groupSubscription.Items.Add(new NavBarGroupItem()
            {
                ID = 2,
                Text = "Personnel",
                Url = "/Personnel",
                Title = "Personnel Management"
            });




            nav.Groups = new List<NavBarGroup>();
            nav.Groups.Add(groupHome);
            nav.Groups.Add(groupAnalysis);
            nav.Groups.Add(groupData);

            if (AppService.Current.IsSysAdmin())
            {
                nav.Groups.Add(groupSubscription);
            }



            setNavBarCache(nav, username);
            return nav;

        }

        private static NavBarMV setFromCache(string username) {
            string key = "WebApp_NavBar_UserName_" + username;
            return MemoryCache.Default.Get(key) as NavBarMV;
        }

        private static void setNavBarCache(NavBarMV navbar, string username) {
            string key = "WebApp_NavBar_UserName_" + username;
            MemoryCache.Default.Set(
                key,
                navbar,
                new DateTimeOffset(DateTime.Now.AddMinutes(20))
                );
        }

        public static void InvalidateNavBarCache(string username) {
            string key = "WebApp_NavBar_UserName_" + username;
            MemoryCache.Default.Remove(key);
        }

    }
}