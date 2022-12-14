using System.Collections.Generic;
using MBUretim.Mvc.Models;

namespace MBUretim.Mvc.ViewInModel
{
    public class FirmViewModel
    {
        public List<MBGOP_Firms> Firms { get; set; }
        public MBGOP_Firms Firm { get; set; }
    }
}