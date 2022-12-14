using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using MBUretim.Mvc.Extensions;
using MBUretim.Mvc.Models;
using MBUretim.Mvc.Models.LogoService;
using MBUretim.Mvc.Notification;
using MBUretim.Mvc.ViewInModel;
using PagedList;

namespace MBUretim.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static List<MBGOP_ProductOrder> _excelList;

        public ActionResult Index()
        {

            //using (var context = new LOGOProductOrderContext())
            //{
            //    var branchs = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
            //    ViewData["branchs"] = branchs;
            //    //var sql = context.Database.SqlQuery(LogoService.ProductOrder.ToConvertFirms().ToString(), new SqlParameter("@DATE", new DateTime(DateTime.Now.Year, 10, 19)), new SqlParameter("@BRANCHNO", "3452"));
            //    var sql = context.Database.SqlQuery<MBGOP_ProductOrder>(LogoService.ProductOrder.ToConvertFirms(), new SqlParameter("@DATE", new DateTime(DateTime.Now.Year, 10, 19)), new SqlParameter("@BRANCHNO", "3452"));
            //}

            return View();
        }
        [HttpPost]
        public ActionResult Index(string startDate, string endDate, IndexViewInModel model, string[] branch)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var branchs = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["branchs"] = branchs;

            }

            var sDate = DateTime.Parse(startDate);
            var eDate = endDate == "" ? sDate : DateTime.Parse(endDate);



            var result = LogoService.GetProductOrderByDateAndByBranch(sDate, eDate, model.CapiDivNr);
            _excelList = result;
            var viewModel = new IndexViewInModel
            {
                //ProductOrders = result
            };
            //ExportData();
            return View(viewModel);
        }
        public ActionResult ExportData()
        {

            var grid = new GridView();
            var result = _excelList;
            if (result == null) return RedirectToAction("Index");
            var fileName = (from fn in result select new { fn.StficheFicheNo, fn.StficheDate }).FirstOrDefault();
            grid.DataSource = result;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", @"attachment; filename=" + fileName.StficheDate.Value.ToString("dd MM yyyy") + "_" + fileName.StficheFicheNo.Trim() + ".xls");
            Response.ContentType = "application/ms-excel";

            using (var sw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    // render the GridView to the HtmlTextWriter
                    grid.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Generate()
        {
            //var belgekod = item.Belgekod;
            //var listBelgeKod = orderList.Where(x => x.Belgekod == belgekod).ToList();

            //if (!IsThereOrder(belgekod)) continue;


            //foreach (var item in _excelList)
            //{
            //   var thereIs = LogoService.GetProductorderByFicheNo(item.STFICHE_FICHENO);
            //    if (thereIs==null)
            //    {
            //LogoService.SaveProductorders(_excelList);
            //    }

            //}


            return RedirectToAction("Index");
        }

        public ActionResult ProducedProductionOrders(int? page)
        {
            using (var context = new LOGOProductOrderContext())
            {
                return View(context.MBGOP_ProductOrder.Where(x => x.IsThere == true).OrderByDescending(x => x.StficheDate).ToList().ToPagedList(page ?? 1, 20));
            }


        }

        public ActionResult ProductPairing(int? page)
        {

            using (var context = new LOGOProductOrderContext())
            {
                ViewData["Items"] = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["BomMaster"] = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["CapiDiv"] = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var result = context.MBGOP_ProductPairing.ToList();
                var viewModel = new PairingViewModel
                {
                    MbProductpairings = result.ToList().ToPagedList(page ?? 1, 20)
                };
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult ProductPairing(PairingViewModel model, string itemCode, string bomCode)
        {

            using (var context = new LOGOProductOrderContext())
            {
                ViewData["Items"] = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["BomMaster"] = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["CapiDiv"] = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                if (!ModelState.IsValid) return RedirectToAction("ProductPairing");
                context.MBGOP_ProductPairing.Add(new MBGOP_ProductPairing
                {
                    ItemCode = itemCode,
                    BomCode = bomCode
                });
                context.SaveChanges();

                return RedirectToAction("ProductPairing");
            }
        }


        public ActionResult DeleteProducedProductionOrders(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var deleteFirm = context.MBGOP_ProductOrder.FirstOrDefault(x => x.ProductOrderId == id);
                context.MBGOP_ProductOrder.Remove(deleteFirm);
                var result = context.SaveChanges();

                if (result > 0)
                {
                    this.AddToastMessage("Başarılı", "İrsaliye Kaydı Silindi", ToastType.Success);
                }
                else
                {
                    this.AddToastMessage("Hata", "İrsaliye Kaydı Silinemedi", toastType: ToastType.Error);

                }

                return RedirectToAction("ProducedProductionOrders");
            }
        }

        public ActionResult OrderedFiche(int? page)
        {
            return View(LogoService.GetOrderedFicheList().ToList().ToPagedList(page ?? 1, 20));
        }
        public ActionResult UnOrderedFiche()
        {

            return View();

        }
        [HttpPost]
        public ActionResult UnOrderedFiche(int? page, string Date)
        {
            DateTime tarih = Convert.ToDateTime(Date);
            using (var context = new LOGOProductOrderContext())
            {
                List<MBGOP_ProductOrder> getUnOrder = context.MBGOP_ProductOrder.Where(x => x.IsThere == false && x.StficheDate == tarih).ToList(); ;
                var listUniq = getUnOrder.GroupBy(x => new { x.ItemCode, x.StficheDate,x.StficheFicheNo }).Select(group => group.First()).ToList();
                var result = listUniq;

                return View(result.ToList().ToPagedList(page ?? 1, 20));
            }
        }

        public ActionResult ReadLog()
        {
            var sb = new StringBuilder();

            try
            {
                var fileStream = new FileStream(Server.MapPath("~/log-file.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                        sb.AppendLine("<br/>");
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
            ViewBag.Log = sb;
            return PartialView("_ReadLog", ViewBag.Log);
        }
        public ActionResult ReadErrorLog()
        {
            var sb = new StringBuilder();

            try
            {
                DateTime now = DateTime.Now;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                var fileStream = new FileStream(Server.MapPath("~/UretimLogs\\Uretim_ErrorLog_" + now.ToString("dd-MM-yyyy") + ".txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                        sb.AppendLine("<br/>");
                    }
                }
            }
            catch (Exception)
            {

            }

            ViewBag.Log = sb;
            return PartialView("_ReadLog", ViewBag.Log);
        }

        public StringBuilder ErrorLog()
        {
            var sb = new StringBuilder();

            try
            {
                DateTime now = DateTime.Now;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                var fileStream = new FileStream(Server.MapPath("~/UretimLogs\\Uretim_ErrorLog_" + now.ToString("dd-MM-yyyy") + ".txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                        sb.AppendLine("<br/>");
                    }
                }
            }
            catch (Exception)
            {

            }
            return sb;
        }

    }
}