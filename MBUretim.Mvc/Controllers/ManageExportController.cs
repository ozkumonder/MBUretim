using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using MBUretim.Mvc.Models;

namespace MBUretim.Mvc.Controllers
{
    public class ManageExportController : Controller
    {
        private static List<MBGOP_ProductOrder> _excelList;
        // GET: ManageExport

        public ActionResult ExportData(List<MBGOP_ProductOrder> exportList)
        {
            var grid = new GridView();
            var result = _excelList;
            if (result == null) return RedirectToAction("Index", "Home");
            var fileName = (from fn in result select new {fn.StficheFicheNo, fn.StficheDate}).FirstOrDefault();
            grid.DataSource = result;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
                @"attachment; filename=" + fileName.StficheDate.Value.ToString("dd MM yyyy") + "_" +
                fileName.StficheFicheNo.Trim() + ".xls");
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
            return RedirectToAction("Index", "Home");
        }
    }
}