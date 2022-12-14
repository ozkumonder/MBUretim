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
    public class ManagePairingController : Controller
    {
        // GET: ManagePairing
        public ActionResult Pairing()
        {
            using (var context = new LOGOProductOrderContext())
            {
                ViewData["Items"] = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["BomMaster"] = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["CapiDiv"] = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var result = context.MBGOP_ProductPairing.ToList();
                var viewModel = new PairingViewModel
                {
                    MbProductpairings = result.ToList().ToPagedList(1,20)
                };
                return View(viewModel);
            }
        }

        public ActionResult AddPairing()
        {
            using (var context = new LOGOProductOrderContext())
            {
                ViewData["Items"] = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["BomMaster"] = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["CapiDiv"] = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                return RedirectToAction("Pairing");
            }
        }
        [HttpPost]
        public ActionResult AddPairing(PairingViewModel model)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var items = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var bomMaster = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var capiDiv = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["Items"] = items;
                ViewData["BomMaster"] = bomMaster;
                ViewData["CapiDiv"] = capiDiv;
                if (!ModelState.IsValid) return RedirectToAction("Pairing");
                context.MBGOP_ProductPairing.Add(new MBGOP_ProductPairing
                {
                    ItemId = model.Item.Id,
                    ItemCode = items.FirstOrDefault(x => x.Id == model.Item.Id).Code,
                    ItemName = items.FirstOrDefault(x => x.Id == model.Item.Id).Name,
                    BomId = model.BomMaster.Id,
                    BomCode = bomMaster.FirstOrDefault(x => x.Id == model.BomMaster.Id).Code,
                    BomName = bomMaster.FirstOrDefault(x => x.Id == model.BomMaster.Id).Name,
                    BomRevId = LogoService.GetBomRevSn(model.BomMaster.Id).BomRevId,
                    BomRevCode = LogoService.GetBomRevSn(model.BomMaster.Id).BomRevCode,
                    DivisionId = model.CapiDiv.CAPIDIV_LOGICALREF,
                    DivisionCode = capiDiv.FirstOrDefault(x => x.CAPIDIV_LOGICALREF == model.CapiDiv.CAPIDIV_LOGICALREF).CAPIDIV_NR,
                    DivisionName = capiDiv.FirstOrDefault(x => x.CAPIDIV_LOGICALREF == model.CapiDiv.CAPIDIV_LOGICALREF).CAPIDIV_NAME
                });
                context.SaveChanges();

                return RedirectToAction("Pairing");
            }
        }

        public ActionResult EditPairing(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {
                ViewData["Items"] = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["BomMaster"] = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                ViewData["CapiDiv"] = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var editPair = context.MBGOP_ProductPairing.FirstOrDefault(x => x.ProductPairingId == id);
                var viewModel = new PairingViewModel
                {
                    MbProductpairing = editPair
                };
                return PartialView("_EditPairing",viewModel);
            }
            
        }
        [HttpPost]
        public ActionResult EditPairing(PairingViewModel model)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var items = LogoService.GetItems(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var bomMaster = LogoService.GetBomMaster(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var capiDiv = LogoService.GetCapiDivs(context.MBGOP_Firms.FirstOrDefault(x => x.IsDefault == true).FirmNr);
                var pairModel = new MBGOP_ProductPairing
                {
                    ProductPairingId = model.MbProductpairing.ProductPairingId,
                    ItemId = model.Item.Id,
                    ItemCode = items.FirstOrDefault(x => x.Id == model.Item.Id).Code,
                    ItemName = items.FirstOrDefault(x => x.Id == model.Item.Id).Name,
                    BomId = model.BomMaster.Id,
                    BomCode = bomMaster.FirstOrDefault(x => x.Id == model.BomMaster.Id).Code,
                    BomName = bomMaster.FirstOrDefault(x => x.Id == model.BomMaster.Id).Name,
                    BomRevId = LogoService.GetBomRevSn(model.BomMaster.Id).BomRevId,
                    BomRevCode = LogoService.GetBomRevSn(model.BomMaster.Id).BomRevCode,
                    DivisionId = model.CapiDiv.CAPIDIV_LOGICALREF,
                    DivisionCode = capiDiv.FirstOrDefault(x => x.CAPIDIV_LOGICALREF == model.CapiDiv.CAPIDIV_LOGICALREF).CAPIDIV_NR,
                    DivisionName = capiDiv.FirstOrDefault(x => x.CAPIDIV_LOGICALREF == model.CapiDiv.CAPIDIV_LOGICALREF).CAPIDIV_NAME
                };
                context.Entry(context.MBGOP_ProductPairing.Find(model.MbProductpairing.ProductPairingId)).CurrentValues.SetValues(pairModel);
                var result = context.SaveChanges();
                if (result > 0)
                {
                    this.AddToastMessage("Bilgi", "Ürün Reçete Bilgileri Güncellendi", ToastType.Success);
                }
                else
                {
                    this.AddToastMessage("Hata", "Ürün Reçete Bilgileri Güncellenemedi", ToastType.Error);
                }

                return RedirectToAction("Pairing");
            }
            
        }

        public ActionResult DeletePairing(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {

                var deleteFirm = context.MBGOP_ProductPairing.FirstOrDefault(x => x.ProductPairingId == id);
                context.MBGOP_ProductPairing.Remove(deleteFirm);
                var result = context.SaveChanges();
                if (result > 0)
                {
                    this.AddToastMessage("Başarılı", "Ürün Reçete İlişkisi Silindi", ToastType.Success);
                }
                else
                {
                    this.AddToastMessage("Hata", "Hata", toastType: ToastType.Error);

                }

                return RedirectToAction("Pairing");
            }
        }
    }
}