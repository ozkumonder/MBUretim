using System.Collections.Generic;
using MBUretim.Mvc.Models;
using PagedList;

namespace MBUretim.Mvc.ViewInModel
{
    public class PairingViewModel
    {
        public IPagedList<MBGOP_ProductPairing> MbProductpairings { get; set; }

        public MBGOP_ProductPairing MbProductpairing { get; set; }
        public Item Item { get; set; }
        public BomMaster BomMaster { get; set; }
        public CapiDiv CapiDiv { get; set; }
    }
}