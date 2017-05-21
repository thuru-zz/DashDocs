using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    [Authorize]
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;

            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}