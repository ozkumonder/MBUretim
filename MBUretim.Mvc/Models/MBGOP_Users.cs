namespace MBUretim.Mvc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MBGOP_Users
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string LogoUserName { get; set; }

        public string LogoPassword { get; set; }
    }
}
