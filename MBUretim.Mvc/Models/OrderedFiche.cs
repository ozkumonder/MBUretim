using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MBUretim.Mvc.Models
{
    public class OrderedFiche
    {
        public string FicheType { get; set; }
        public string StFicheNo { get; set; }
        public string ProdFicheNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double StlineAmount { get; set; }


    }
}