using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Api.App.Token.Models;
using Genrev.Domain.Accounts;


namespace Genrev.Api.App.Token
{
    public class TokenService
    {
        internal TokenResponseModel GetToken(TokenRequestModel request) {

            var account = _context.Accounts.Where(x => x.ApiKey == request.ApiKey).SingleOrDefault();

            if (!validateTokenRequest(account, request)) {
                throw new NotAuthorizedException();
            }

            var token = new ApiToken();

            token.GenerationDate = DateTime.Now;
            token.ExpirationDate = DateTime.Now.AddMinutes(60);
            token.Value = generateTokenValue();

            account.ApiTokens.Add(token);

            _context.SaveChanges();

            var model = new TokenResponseModel();
            model.Token = new Models.Token();
            model.Token.ExpiresOn = token.ExpirationDate;
            model.Token.GeneratedOn = token.GenerationDate;
            model.Token.Value = token.Value;

            return model;
        }



        private string generateTokenValue() {

            Random random = new Random();
            int length = 255;

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }

        private bool validateTokenRequest(Account account, TokenRequestModel request) {
            
            if (account == null) {
                return false;
            }

            if (!account.ApiEnabled) {
                return false;
            }

            if (!account.AllowApiIpBypass) {
                if (!verifyIpInRange(account, getClientIP())) {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(account.ApiPassword)) {
                return false;
            }

            if (account.ApiPassword != request.Password) {
                return false;
            }
            
            return true;

        }


        private bool verifyIpInRange(Account account, string clientIP) {

            foreach (var item in account.ApiIpWhitelistItems) {
                if (item.IsInRange(clientIP)) {
                    return true;
                }
            }

            return false;
        }


        private string getClientIP() {
            string ip = HttpContext.Current.Request.ServerVariables["HTTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip)) {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            } else {
                ip = ip.Split(',')[0];
            }
            return ip;
        }



        private Data.GenrevContext _context;
        public TokenService() {
            _context = AppService.Current.DataContext;
        }

    }



}