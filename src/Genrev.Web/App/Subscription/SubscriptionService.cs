using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Web.App.Subscription.Models;
using Genrev.Domain.Accounts;

namespace Genrev.Web.App.Subscription
{
    public class SubscriptionService
    {


        internal ApiVM GetApiVM() {

            var model = new ApiVM();

            var account = AppService.Current.Account;

            model.ApiEnabled = account.ApiEnabled;
            model.ApiKey = account.ApiKey;
            model.ApiPassword = account.ApiPassword;
            
            return model;

        }




        private Genrev.Data.GenrevContext _context;

        public SubscriptionService() {
            _context = AppService.Current.DataContext;
        }

        internal void SaveApiSettings(bool apiEnabled, string apiKey, string apiPassword) {

            var account = _context.Accounts.Find(AppService.Current.Account.ID);
            
            account.ApiEnabled = apiEnabled;
            account.ApiKey = apiKey;
            account.ApiPassword = apiPassword;

            _context.SaveChanges();
        }

        internal string GetNewApiKey() {

            var key = randomString(30);
            return key;
        }


        private string randomString(int length) {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}