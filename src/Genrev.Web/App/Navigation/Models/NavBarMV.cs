using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Navigation.Models
{
    public class NavBarMV
    {
        public List<NavBarGroup> Groups { get; set; }

        public string GetControllerName(int groupID, int itemID) {
            var item = Groups.Where(g => g.ID == groupID).First().Items.Where(i => i.ID == itemID).First();
            string action;
            string controller;
            parseUrlToActionAndController(item.Url, out action, out controller);
            return controller;
        }

        public string GetActionName(int groupID, int itemID) {
            var item = Groups.Where(g => g.ID == groupID).First().Items.Where(i => i.ID == itemID).First();
            string action;
            string controller;
            parseUrlToActionAndController(item.Url, out action, out controller);
            return action;
        }

        void parseUrlToActionAndController(string url, out string action, out string controller) {
            string s = url.Substring(1);
            action = s.Split('/')[1];
            controller = s.Split('/')[0];
        }

    }

    public class NavBarGroup
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public bool Expanded { get; set; }
        public List<NavBarGroupItem> Items { get; set; }
        
    }

    public class NavBarGroupItem
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }

}