namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_ProdOrderFiche
    {
        public int Id { get; set; }

        public int ProdOrderRef { get; set; }

        public string ProdOrderFicheNo { get; set; }

        public int ItemRef { get; set; }

        public int BomRef { get; set; }

        public int RevRef { get; set; }

        public int FactoryNr { get; set; }

        public double PlnAmount { get; set; }

        public int UomRef { get; set; }
    }
}
