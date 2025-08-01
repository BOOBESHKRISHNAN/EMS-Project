//heart of your database configuration - defines how your data models behave and interact when mapped to the database.
using EventManagementCL.Models;
using System.Text;                        //string encoding
using Microsoft.EntityFrameworkCore;     //Core library for using EF Core (the ORM that maps C# classes to database tables).
using System.Security.Cryptography;      //hash passwords securely with SHA256.

namespace EventManagementCL.Data
{
    public class AppDbContext : DbContext  //inherits from dbcontext
    {
        public DbSet<User> Users { get; set; }  //represent tables in the database.
        public DbSet<Event> Events { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)     //Accepts options like connection strings, used when setting up the database in your application
            : base(options)
        {
        }

        //overrides a method from EF Core's DbContext class - gives access to ModelBuilder object, which is used to configure your entity models and their relationships
        //why override? Entity Framework Core comes with default conventions to map your C# classes to database tables. But when you want to customize things like cascading, enum conversion, relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();                  //enum to string in db instead of int

            //Event → Location relationship
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Location)                   //One-to-many relationship between Location and Event
                .WithMany(l => l.Events)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Cascade);         //Cascade means deleting a location will delete associated events.

            modelBuilder.Entity<Event>()
           .HasOne(e => e.Organizer)
           .WithMany()
           .HasForeignKey(e => e.OrganizerId)
           .OnDelete(DeleteBehavior.Restrict);            //Restrict prevents deletion of an event if feedback exists

            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.Event)
               .WithMany()
               .HasForeignKey(t => t.EventID)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Event)
            .WithMany(e => e.Feedbacks)
            .HasForeignKey(f => f.EventId)
            .OnDelete(DeleteBehavior.Restrict); // prevent cycle conflict

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Ticket)
            .WithMany(t => t.Feedbacks)
            .HasForeignKey(f => f.TicketID)
            .OnDelete(DeleteBehavior.Restrict); // only one cascade allowed

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // safe user reference

            //Seeding SuperAdmin Data into db
            //Replacing dynamic DateTime with static seed value to avoid EF Core warning
            var staticCreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var passwordBytes = Encoding.UTF8.GetBytes("Admin@123456789");
            var passwordHash = Convert.ToBase64String(SHA256.HashData(passwordBytes));

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1001,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@event.com",
                ContactNumber = "9999999999",
                PasswordHash = passwordHash,
                Role = UserRole.SuperAdmin,
                CreatedAt = staticCreatedAt
            });
            modelBuilder.Entity<Location>().HasData(
            new Location
            {
            Id = 1001,
            Name = "Chennai Convention Center",
            Address = "123 Marina Road",
            City = "Chennai",
            State = "Tamil Nadu",
            ZipCode = "600001",
            VenueFee = 30000
            },
            new Location
            {
            Id = 1002,
            Name = "Hyderabad Expo Hall",
            Address = "456 Jubilee Hills",
            City = "Hyderabad",
            State = "Telangana",
            ZipCode = "500033",
                VenueFee = 50000
            }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
