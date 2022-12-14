namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_Params
    {
        [Key]
        public int ParamId { get; set; }

        public int ParamNr { get; set; }
        [Required]
        [StringLength(150)]
        public string ParamName { get; set; }

        public bool ParamValue { get; set; }
    }
}
