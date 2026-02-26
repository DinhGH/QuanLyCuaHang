using System.Data.Entity;
using QuanLyCuaHang.Models;

namespace QuanLyCuaHang.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=QLCHDB")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToTable("employees")
                .HasKey(x => x.id);

            // Map t?t c? columns
            modelBuilder.Entity<User>()
                .Property(x => x.id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.full_name)
                .HasColumnName("full_name")
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(x => x.phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(x => x.email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(x => x.role)
                .HasColumnName("role")
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(x => x.salary)
                .HasColumnName("salary");

            modelBuilder.Entity<User>()
                .Property(x => x.password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(x => x.created_at)
                .HasColumnName("created_at");
        }
    }
}