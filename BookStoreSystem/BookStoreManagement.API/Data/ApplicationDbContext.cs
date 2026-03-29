using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportDetail> ImportDetails { get; set; }
        public DbSet<Employee> Employees { get; set;  }
        public DbSet<Shift> Shifts { get; set;  }
        public DbSet<EmployeeShift> EmployeeShifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new {bc.BookId, bc.CategoryId});

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName("Employees_UserId");

            modelBuilder.Entity<User>().HasData (
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    // remember hash password by brcypt
                    PasswordHash = "$2a$12$D1vG.G0.iA22bZ3hU.Z8/e2xK.6kX4A1X.N/H.8H.J.K.U.V.Q.C.q",
                    FullName = "Adminstrator",
                    Email = "truongnhattien222@gmail.com",
                    RoleId = "admin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.User)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}