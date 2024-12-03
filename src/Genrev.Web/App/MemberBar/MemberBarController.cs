using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Genrev.Web.App.MemberBar
{
    public class MemberBarController : Controller
    {

        Genrev.Data.GenrevContext dataContext;

        public MemberBarController()
        {
            dataContext = new Genrev.Data.GenrevContext();
        }


        [HttpGet]
        public ActionResult ViewAs() {
            var model = new Models.ViewAsVM();
            var currentPerson = AppService.Current.Person.Person;
            model.Name = currentPerson.FirstName + " " + currentPerson.LastName;
            return PartialView("ViewAs", model);
        }

        [HttpGet]
        public ActionResult ViewAsPopup() {
            return PartialView("ViewAsPopup");
        }

        [HttpGet]
        public ActionResult ViewAsPopupContent() {
            var model = AppService.Current.Person.Personnel;
            return PartialView("ViewAsPopupContent", model);
        }

        [HttpGet]
        public ActionResult UserPopup() {
            return PartialView("UserPopup");
        }

        [HttpGet]
        public ActionResult UserPopupContent() {
            return PartialView("UserPopupContent");
        }

        [HttpGet]
        public ActionResult AlertPopup() {
            return PartialView("AlertPopup");
        }

        [HttpGet]
        public ActionResult AlertPopupContent() {
            return PartialView("AlertPopupContent");
        }

        [HttpGet]
        public ActionResult HelpPopup() {
            return PartialView("HelpPopup");
        }

        [HttpGet]
        public ActionResult HelpPopupContent() {
            return PartialView("HelpPopupContent");
        }

        [HttpGet]
        public ActionResult FeedbackPopup()
        {
            return PartialView("FeedbackPopup");
        }

        [HttpGet]
        public ActionResult FeedbackPopupContent()
        {
            return PartialView("FeedbackPopupContent");
        }

        [HttpPost]
        public ActionResult FeedbackSubmit(string feedback)
        {
            dataContext.Feedback.Add(new Domain.DataSets.Feedback()
            {
                SubmittedBy = AppService.Current.User.User.ID,
                SubmittedTime = DateTime.UtcNow,
                MessageText = feedback
            });

            dataContext.SaveChanges();

            Domain.Email.SMTPAccount smtpAccount = AppService.Current.Settings.SMTPAccount;

            string message = "Genrev Feedback from User: " + AppService.Current.User.User.Email + "\n\n" + "'" + feedback + "'";

            DomainServices.Email.SMTP.Send(smtpAccount, "Genrev Feedback", message, AppService.Current.Settings.FeedbackEmails);

            return Json("ok");
        }



    }
}