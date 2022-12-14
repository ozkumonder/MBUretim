using MBUretim.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBUretim.Mvc.Controllers
{
    [Authorize]
    public class ManageParamsController : Controller
    {
        // GET: ManageParams
        public ActionResult Params()
        {
            using (var context = new LOGOProductOrderContext())
            {
                var result = context.MBGOP_Params.ToList();
                return View(result);
            }
        }

        public ActionResult EditParams()
        {
            using (var context = new LOGOProductOrderContext())
            {
                return View();
            }
            
        }

        public ActionResult EditParams(List<MBGOP_Params> model,MBGOP_Params param)
        {
            return View();
        }

    }
}