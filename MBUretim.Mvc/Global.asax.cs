using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using MBUretim.Mvc.Models;

namespace MBUretim.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                HttpCookie cok = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cok == null)
                    return;
                FormsAuthenticationTicket bilet = FormsAuthentication.Decrypt(cok.Value);
                string[] roller = bilet.UserData.Split(';');
                FormsIdentity id = new FormsIdentity(bilet);
                GenericPrincipal priReis = new GenericPrincipal(id, roller);
                Context.User = priReis;

            }
            catch (CryptographicException cex)
            {
                FormsAuthentication.SignOut();
            }
            using (var context = new LOGOProductOrderContext())
            {
                var prodOrderFiche = context.MBGOP_ProdOrderFiche.FirstOrDefault();
                if (prodOrderFiche==null)
                {
                    context.MBGOP_ProdOrderFiche.Add(new MBGOP_ProdOrderFiche
                    {
                        ProdOrderFicheNo = "URT00000001"
                    });
                    context.SaveChanges();
                }

            }
        }

        protected void Application_End()
        {
            //Marshal.ReleaseComObject((object)GlobalParameters.UnityApp);
            //FormsAuthentication.SignOut();
            if (GlobalParameters.UnityApp != null)
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
        }


    }
}
