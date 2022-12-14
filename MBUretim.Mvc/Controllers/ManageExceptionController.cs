using System.Web.Mvc;

namespace MBUretim.Mvc.Controllers
{
    public class ManageExceptionController : Controller
    {
        // GET: ManageException
        public ActionResult NoPageFound()
        {
            return View();
        }

        public ActionResult UnExpected()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}