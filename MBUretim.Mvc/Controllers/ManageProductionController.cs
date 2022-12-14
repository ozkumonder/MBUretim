using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBUretim.Mvc.Extensions;
using MBUretim.Mvc.Models;
using MBUretim.Mvc.Models.LogoService;
using MBUretim.Mvc.Notification;
using MBUretim.Mvc.ViewInModel;
using PagedList;

namespace MBUretim.Mvc.Controllers
{
    [Authorize]
    public class ManageProductionController : Controller
    {
        private static List<MBGOP_ProductOrder> _excelList;

        public ActionResult Production()
        {
            using (var context = new LOGOProductOrderContext())
            {
                var branchs = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault).FirmNr);
                ViewData["branchs"] = branchs;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Production(int? page, string startDate, string endDate, IndexViewInModel model)
        {
            Session["branch"] = model.CapiDivNr;
            Session["startDate"] = startDate;
            Session["endDate"] = endDate;
            bool groupParam;
            using (var context = new LOGOProductOrderContext())
            {
                groupParam = context.MBGOP_Params.FirstOrDefault(x => x.ParamNr == 1).ParamValue;
                var branchs = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault).FirmNr);
                ViewData["branchs"] = branchs;
            }

            var sDate = DateTime.Parse(startDate);
            var eDate = endDate == "" ? sDate : DateTime.Parse(endDate);
            List<MBGOP_ProductOrder> result;
            if (groupParam)
            {
                result = LogoService.GetProductOrderByDateAndByBranchGroup(sDate, eDate, model.CapiDivNr);
                _excelList = result;
            }
            else
            {
                result = LogoService.GetProductOrderByDateAndByBranch(sDate, eDate, model.CapiDivNr);
                _excelList = result;
            }


            var viewModel = new IndexViewInModel
            {
                ProductOrders = result
            };
            if (result.Count <= 0)
            {
                this.AddToastMessage("Bilgilendirme", "Girilen Tarihie Ait Kayıt Bulunamadı!!!", ToastType.Error);
                return View(viewModel);
            }
            //ExportData();
            return View(viewModel);
        }

        [HttpPost]

        public ActionResult Generate()
        {
            var result = new ResultType();
            var brnach = (string[])Session["branch"];
            var begDate = Session["startDate"].ToDateTime();
            var endDate = Session["endDate"].ToString() == "" ? begDate : DateTime.Parse(Session["endDate"].ToString());
            bool groupParam;
            using (var context = new LOGOProductOrderContext())
            {
                groupParam = context.MBGOP_Params.FirstOrDefault(x => x.ParamNr == 1).ParamValue;
            }
            if (groupParam)
            {
                var orderList = LogoService.GetProductOrderByDateAndByBranch((DateTime)begDate, (DateTime)endDate, brnach);
                result = LogoService.AddProdOrders(orderList, true, endDate);


            }
            else
            {
                var orderList = LogoService.GetProductOrderByDateAndByBranch((DateTime)begDate, (DateTime)endDate, brnach);
                result = LogoService.AddProdOrders(orderList, false, begDate);
            }


            //var result = LogoService.AddProdOrders(_excelList);

            if (result.State)
            {
                this.AddToastMessage("Başarılı", "Üretim Emirleri Başarılı Olarak Üretilmiştir.", ToastType.Success);
                _excelList = null;
            }
            else
            {
                this.AddToastMessage("Hata", result.ErrorMessage, ToastType.Error);
            }



            return RedirectToAction("Production");
        }
    }
}