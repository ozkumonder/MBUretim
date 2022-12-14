namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_ProductPairing
    {
        [Key]
        public int ProductPairingId { get; set; }

        public int? ItemId { get; set; }

        [StringLength(25)]
        public string ItemCode { get; set; }

        [StringLength(50)]
        public string ItemName { get; set; }

        public int? BomId { get; set; }

        [StringLength(25)]
        public string BomCode { get; set; }

        [StringLength(50)]
        public string BomName { get; set; }

        public int BomRevId { get; set; }

        public string BomRevCode { get; set; }

        public int? DivisionId { get; set; }

        public short? DivisionCode { get; set; }

        [StringLength(50)]
        public string DivisionName { get; set; }
    }
}
