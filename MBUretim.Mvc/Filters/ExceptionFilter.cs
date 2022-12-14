using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBUretim.Mvc.Filters
{
    //public class ExceptionFilter : ActionFilterAttribute, IExceptionFilter
    //{
        //public void OnException(ExceptionContext filterContext)
        //{

        //    filterContext.ExceptionHandled = true;

        //    if (filterContext.Exception is NotificationException)
        //    {
        //        filterContext.Result = new ViewResult
        //        {
        //            TempData = new TempDataDictionary
        //            {
        //                { string.Format("notifications.{0}", NotifyType.Error),filterContext.Exception.Message }
        //            }
        //        };
        //    }
        //    else
        //    {



        //        filterContext.Result = new ViewResult()
        //        {
        //            ViewName = "~/Views/Exception/Error.cshtml",
        //            ViewData = new ViewDataDictionary(filterContext.Exception)
        //        };


        //    }

        //}
    //}
}