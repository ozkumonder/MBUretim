using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MBUretim.Mvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "Parametreler",
               url: "tavukdunyasi/parametreler",
               defaults: new { controller = "ManageParams", action = "Params" }
               );
            routes.MapRoute(
               name: "UretimIrsaliyeleri",
               url: "tavukdunyasi/uretimi_gerceklesen_irsaliyeler",
               defaults: new { controller = "Home", action = "ProducedProductionOrders" }
               );
            routes.MapRoute(
               name: "BagliFisler",
               url: "tavukdunyasi/bagli_fisler",
               defaults: new { controller = "Home", action = "OrderedFiche" }
               );
            routes.MapRoute(
                name: "UretilmeyenFisler",
                url: "tavukdunyasi/uretilmeyen_fisler",
                defaults: new { controller = "Home", action = "UnOrderedFiche" }
            );
            routes.MapRoute(
                name: "FirmaTanimlari",
                url: "tavukdunyasi/firma_tanimlari",
                defaults: new { controller = "ManageFirms", action = "Firms" }
                );
            routes.MapRoute(
                name: "FirmaTanimlariDuzenle",
                url: "tavukdunyasi/firma_tanimlari_duzenle",
                defaults: new { controller = "ManageFirms", action = "UpdateFirm" }
                );
            routes.MapRoute(
                name: "FirmaTanimlariEkle",
                url: "tavukdunyasi/firma_tanimlari_ekle",
                defaults: new { controller = "ManageFirms", action = "AddFirms" }
                );
            routes.MapRoute(
                name: "UrunReceteEslestirme",
                url: "tavukdunyasi/urun_recete_eslestirme",
                defaults: new { controller = "ManagePairing", action = "Pairing" }
                );
            routes.MapRoute(
                name: "UrunReceteEslestirmeDuzenle",
                url: "tavukdunyasi/urun_recete_eslestirme_duzenle",
                defaults: new { controller = "ManagePairing", action = "EditPairing" }
                );
            routes.MapRoute(
                name: "UrunReceteEslestirmeEkle",
                url: "tavukdunyasi/urun_recete_eslestirme_ekle",
                defaults: new { controller = "ManagePairing", action = "AddPairing" }
                );
            routes.MapRoute(
                name: "Uretim",
                url: "tavukdunyasi/uretim/{branch}/{begdate}/{enddate}",
                defaults: new { controller = "ManageProduction", action = "Production",branch=UrlParameter.Optional,begdate=UrlParameter.Optional,enddate=UrlParameter.Optional }
                );
            routes.MapRoute(
               name: "Login",
               url: "tavukdunyasi/girisyap",
               defaults: new { controller = "ManageAccount", action = "Login" }
               );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
