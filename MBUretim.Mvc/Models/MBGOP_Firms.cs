namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_Firms
    {
        public int Id { get; set; }

        public short FirmNr { get; set; }

        public bool IsDefault { get; set; }

        public bool? IsDelete { get; set; }
    }
}
