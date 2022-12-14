namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_ProductOrder
    {
        [Key]
        public int ProductOrderId { get; set; }

        public DateTime? StficheDate { get; set; }

        public int? StficheLogicalref { get; set; }

        public short? StficheBranch { get; set; }

        [StringLength(17)]
        public string StficheFicheNo { get; set; }

        public int? ItemLogicalref { get; set; }

        [StringLength(25)]
        public string ItemCode { get; set; }

        [StringLength(51)]
        public string ItemName { get; set; }

        public double? StlineAmount { get; set; }

        public int? BomMasterLogicalRef { get; set; }

        public int? BomRevSnLogicalref { get; set; }

        public int? UnitSetLLogicalref { get; set; }

        [StringLength(11)]
        public string UnitSetLCode { get; set; }

        [StringLength(11)]
        public string UnitSetLName { get; set; }

        [StringLength(61)]
        public string CapiDivName { get; set; }

        public bool IsThere { get; set; }
    }
}
