using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc;

namespace Genrev.Web.App.Admin
{
    public class AdminController : Dymeng.Web.Mvc.ControllerBase
    {



        /***********************
        * 
        * USER ADMINISTRATION
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Users() {
            var model = new Models.Users.UserAdminHomeVM();
            return PartialView("UserAdminHome", model);
        }
        


        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/



        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/





        /***********************
        * 
        * THAT AREA
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/



        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/



        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/





        /***********************
        * 
        * CONTROLLER SETUP
        * 
        ************************/


    }
}