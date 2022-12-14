using System.Web.Mvc;
using MBUretim.Mvc.Notification;

namespace MBUretim.Mvc.Extensions
{
    public static class ControllerExtensions
    {
        public static ToastMessage AddToastMessage(this Controller controller, string title, string message,
            ToastType toastType = ToastType.Info)
        {
            var toastr = controller.TempData["Toastr"] as Toastr;
            toastr = toastr ?? new Toastr();

            var toastMessage = toastr.AddToastMessage(title, message, toastType);
            controller.TempData["Toastr"] = toastr;
            return toastMessage;
        }
    }
}