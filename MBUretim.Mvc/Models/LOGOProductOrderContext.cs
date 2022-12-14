using System.Threading;

namespace MBUretim.Mvc.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LOGOProductOrderContext : DbContext
    {
        public LOGOProductOrderContext()
            : base("name=LOGOProductOrderContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<MBGOP_Firms> MBGOP_Firms { get; set; }
        public virtual DbSet<MBGOP_Params> MBGOP_Params { get; set; }
        public virtual DbSet<MBGOP_ProdOrderFiche> MBGOP_ProdOrderFiche { get; set; }
        public virtual DbSet<MBGOP_ProductOrder> MBGOP_ProductOrder { get; set; }
        public virtual DbSet<MBGOP_ProductPairing> MBGOP_ProductPairing { get; set; }
        public virtual DbSet<MBGOP_Users> MBGOP_Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.StficheFicheNo)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.ItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.ItemName)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.UnitSetLCode)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.UnitSetLName)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductOrder>()
                .Property(e => e.CapiDivName)
                .IsUnicode(false);

            modelBuilder.Entity<MBGOP_ProductPairing>()
                .Property(e => e.ItemCode)
                .IsUnicode(false);
        }
    }
}
