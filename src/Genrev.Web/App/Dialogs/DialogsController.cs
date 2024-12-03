using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Genrev.Web.App.Dialogs
{
    public class DialogsController : Controller
    {


        [HttpGet]
        public ActionResult AlertPopup() {
            return PartialView("AlertPopup");
        }

        [HttpGet]
        public ActionResult AlertPopupContent() {
            return PartialView("AlertPopupContent");
        }

        [HttpGet]
        public ActionResult ConfirmPopup() {
            return PartialView("ConfirmPopup");
        }

        [HttpGet]
        public ActionResult ConfirmPopupContent() {
            return PartialView("ConfirmPopupContent");
        }

        [HttpGet]
        public ActionResult Popup(string id, int width, int height, string title, bool allowDrag, bool allowResize) {

            var model = new Models.PopupOptionsVM();

            model.AllowDrag = allowDrag;
            model.AllowResize = allowResize;
            model.Height = height;
            model.ID = id;
            model.Title = title;
            model.Width = width;

            return PartialView("Popup", model);

        }

    }
}