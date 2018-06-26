using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using TaxiTicketSystem.Filters;
using TaxiTicketSystem.Models;

namespace TaxiTicketSystem.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.message = "";
            return View("Index");
        }

        [HttpPost]
        public ActionResult Login(string code, string password, string returnUrl)
        {
            returnUrl = "/Home/Index";

            MyMembershipProvider myMembershipProvider = new MyMembershipProvider();
            if (myMembershipProvider.ValidateUser(code, password))
            {
                FormsAuthentication.SetAuthCookie(code, false);

                if (!String.IsNullOrEmpty(returnUrl))
                {
                    dynamic userType = Session["userType"];
                    if (userType != null && userType.x)
                    {
                        if (User.Identity.Name == "sofo")
                        {
                            returnUrl = "/Home/DriverSalaryReport";
                        }
                        else
                        {
                            returnUrl = "/Home/Index";
                        }
                    }      
                    else if(userType != null && !userType.x)
                    {
                        returnUrl = "/Home/Reports";
                    }              
                    else if(userType == null)
                    {
                        returnUrl = "/Account/Index";
                    }

                    return Redirect(returnUrl);
                }
                else return RedirectToAction("Index", "Account");
            }
            else
            {
                ViewBag.ValidateUserMessage = "სახელი ან პაროლი არასწორია!";
                return View("Index");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account");
        }
    }
}
