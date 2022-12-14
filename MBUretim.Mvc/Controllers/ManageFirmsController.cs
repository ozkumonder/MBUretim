using System.Linq;
using System.Web.Mvc;
using MBUretim.Mvc.Extensions;
using MBUretim.Mvc.Models;
using MBUretim.Mvc.Notification;
using MBUretim.Mvc.ViewInModel;

namespace MBUretim.Mvc.Controllers
{
    [Authorize]
    public class ManageFirmsController : Controller
    {
        public ActionResult Firms()
        {
            using (var context = new LOGOProductOrderContext())
            {
                var viewModel = new FirmViewModel
                {
                    Firms = context.MBGOP_Firms.ToList()
                };

                return View(viewModel);
            }
        }
        // GET: ManageFirms
        public ActionResult AddFirms()
        {
            using (var context = new LOGOProductOrderContext())
            {

                var viewModel = new FirmViewModel
                {
                    Firms = context.MBGOP_Firms.ToList()
                };

                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult AddFirms(FirmViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Firm.FirmNr.ToString()))
            {
                using (var context = new LOGOProductOrderContext())
                {
                    if (context.MBGOP_Firms.Any(x => x.IsDefault)&&model.Firm.IsDefault==true)
                    {
                        this.AddToastMessage("Hata", "Birden fazla aktif firma ekleyemezsiniz", ToastType.Error);
                    }
                    else
                    {
                        context.MBGOP_Firms.Add(new MBGOP_Firms
                        {
                            FirmNr = model.Firm.FirmNr,
                            IsDefault = model.Firm.IsDefault
                        });
                        context.SaveChanges();
                        this.AddToastMessage("Başarılı", "Yeni Firma Ekklendi", ToastType.Success);
                        
                    }
                   return RedirectToAction("Firms");
                }
            }
            else
            {
                this.AddToastMessage("Hata", "Firma Numarası ve Aktiflik durum bilgisi girilmelidir.", ToastType.Error);
                return RedirectToAction("Firms");
            }
            
        }

        public ActionResult UpdateFirm(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {
                var editFirm = context.MBGOP_Firms.FirstOrDefault(x => x.Id == id);

                var viewModel = new FirmViewModel
                {
                    Firm = editFirm
                };

                return PartialView("_UpdateFirm",viewModel);
            }
        }
        [HttpPost]
        public ActionResult UpdateFirm(FirmViewModel model)
        {
            using (var context = new LOGOProductOrderContext())
            {
                context.Entry(context.MBGOP_Firms.Find(model.Firm.Id)).CurrentValues.SetValues(model.Firm);
              var result =  context.SaveChanges();
                if (result>0)
                {
                    this.AddToastMessage("Bilgi", "Firma Bilgileri Güncellendi", ToastType.Success);
                }
                else
                {
                    this.AddToastMessage("Hata", "Firma Bilgileri Güncellenemedi", ToastType.Error);
                }
                return RedirectToAction("Firms");
            }
        }
        public ActionResult DeleteFirms(int id)
        {
            using (var context = new LOGOProductOrderContext())
            {
                if (context.MBGOP_Firms.Count() > 1)
                {
                    var deleteFirm = context.MBGOP_Firms.FirstOrDefault(x => x.Id == id);
                    context.MBGOP_Firms.Remove(deleteFirm);
                    context.SaveChanges();
                    this.AddToastMessage("Başarılı", "Firma Silindi", ToastType.Success);
                }
                else
                {
                    this.AddToastMessage("Hata", "En az 1 Adet firma kaydı olamak zorundadır.",toastType:ToastType.Error);
                   
                }
                
                return RedirectToAction("Firms");
            }
        }

    }
}