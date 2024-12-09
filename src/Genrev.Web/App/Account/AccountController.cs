using Dymeng;
using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;


namespace Genrev.Web.App.Account
{

    [Authorize]
    public class AccountController : Controller
    {
        
        [AllowAnonymous]
        public ActionResult Login() {
            var model = new Models.LoginModel();
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginModel model, string returnUrl) {

            if (ModelState.IsValid) {
                
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe.Value)) {
                    return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "The username or password is incorrect.");
            }
            
            // if we got this far, something failed, redisplay form
            return View(model);
        }





        public ActionResult LogOff() {
            AppService.Current.Invalidate();
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult Register() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.RegisterModel model) {
            if (ModelState.IsValid) {
                try {

                    System.Collections.Generic.Dictionary<string, object> values = new System.Collections.Generic.Dictionary<string, object>();
                    values.Add("Email", model.Email);
                    
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: values);
                    WebSecurity.Login(model.UserName, model.Password);

                    return RedirectToAction("Index", "Home");

                } catch (MembershipCreateUserException e) {
                    e.Handle();
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            // if we got this far something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePassword() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Models.ChangePasswordModel model) {
            if (ModelState.IsValid) {
                bool changePasswordSucceeded;
                try {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                } catch (Exception e) {
                    e.Handle();
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded) {
                    return RedirectToAction("ChangePasswordSuccess");
                } else {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid");
                }
            }
            // if we got this far something's wrong, redisplay form
            return View(model);
        }


        public ActionResult ChangePasswordSuccess() {
            return View();
        }








        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}