using System.Collections.Generic;
using MBUretim.Mvc.Models;
using PagedList;

namespace MBUretim.Mvc.ViewInModel
{
    public class IndexViewInModel
    {
        public List<MBGOP_ProductOrder> ProductOrders { get; set; }
        public CapiDiv CapiDiv { get; set; }
        public string[] CapiDivNr { get; set; }
    }
}