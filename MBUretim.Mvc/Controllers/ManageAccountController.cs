using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using MBUretim.Mvc.Extensions;
using MBUretim.Mvc.LogoHelper;
using MBUretim.Mvc.Models;
using UnityObjects;

namespace MBUretim.Mvc.Controllers
{
    public class ManageAccountController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(MBGOP_Users model)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var result = context.MBGOP_Users.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
                var firmNr = context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr;
                if (result != null)
                {
                    var settings = new StringBuilder();

                    settings.Append($"sUserName${result.UserName};");
                    settings.Append($"sPassword${result.Password};");
                    settings.Append($"sLogoUserName${result.LogoUserName};");
                    settings.Append($"sLogoPassword${result.LogoPassword};");
                    settings.Append($"sFirm${firmNr};");
                    LoginHelper.LogIn(model.UserName, settings.ToString(), false);
                    GlobalParameters.UnityApp = (UnityApplication)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("72DB412A-6BF5-4920-A002-2AAC679951DF")));
                    if (GlobalParameters.UnityApp.Login(LoginHelper.GetSettings("sLogoUserName"), LoginHelper.GetSettings("sLogoPassword"), LoginHelper.GetSettings("sFirm").ToInt32()))
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.LoginError = "Kullanıcı Bilgilerinizi Kontrol Ediniz...\n" + GlobalParameters.UnityApp.GetLastError() + GlobalParameters.UnityApp.GetLastErrorString();
                        Marshal.ReleaseComObject((object)GlobalParameters.UnityApp);
                        return View("Login");
                    }
                }
                else
                {
                    //this.AddToastMessage("Hata", "Giriş Yapılamadı /r Kullanıcı Bilgilerinizi Kontrol Ediniz...",ToastType.Error);
                    ViewBag.LoginError = "Kullanıcı Bilgilerinizi Kontrol Ediniz...";
                    return View("Login");
                }
            }

        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            if (GlobalParameters.UnityApp!=null)
            {
                if (GlobalParameters.UnityApp.LoggedIn)
                {
                    GlobalParameters.UnityApp.Disconnect();
                }
                Marshal.ReleaseComObject((object)GlobalParameters.UnityApp);
                
            }
            
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}