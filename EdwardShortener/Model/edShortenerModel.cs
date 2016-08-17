namespace EdwardShortener
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class edShortenerModel : DbContext
    {
        public edShortenerModel()
            : base("name=edShortenerModel")
        {
        }

        public virtual DbSet<Click> Clicks { get; set; }
        public virtual DbSet<ShortedUrl> ShortedUrls { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortedUrl>()
                .HasMany(e => e.Clicks)
                .WithRequired(e => e.ShortedUrl)
                .HasForeignKey(e => e.Fk_idShortedUrl)
                .WillCascadeOnDelete(false);
        }
    }
}