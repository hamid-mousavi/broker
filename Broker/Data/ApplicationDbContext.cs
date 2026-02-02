using Broker.Models;
using Microsoft.EntityFrameworkCore;

namespace Broker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ClearanceAgent> ClearanceAgents { get; set; }
        public DbSet<AgentSpecialization> AgentSpecializations { get; set; }
        public DbSet<CargoOwner> CargoOwners { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<VerificationRequest> VerificationRequests { get; set; }
        public DbSet<ReviewReply> ReviewReplies { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber);
                entity.Property(e => e.Role).HasConversion<int>();
            });

            // ClearanceAgent Configuration
            modelBuilder.Entity<ClearanceAgent>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.LicenseNumber);
                entity.HasOne(e => e.User)
                    .WithOne(e => e.ClearanceAgent)
                    .HasForeignKey<ClearanceAgent>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AgentSpecialization Configuration
            modelBuilder.Entity<AgentSpecialization>(entity =>
            {
                entity.HasOne(e => e.Agent)
                    .WithMany(e => e.Specializations)
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CargoOwner Configuration
            modelBuilder.Entity<CargoOwner>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User)
                    .WithOne(e => e.CargoOwner)
                    .HasForeignKey<CargoOwner>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Request Configuration
            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasIndex(e => e.CargoOwnerId);
                entity.HasIndex(e => e.AgentId);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasOne(e => e.CargoOwner)
                    .WithMany(e => e.Requests)
                    .HasForeignKey(e => e.CargoOwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Agent)
                    .WithMany(e => e.Requests)
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Appointment Configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasIndex(e => e.RequestId);
                entity.HasIndex(e => e.AppointmentDate);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasOne(e => e.Request)
                    .WithMany(e => e.Appointments)
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Message Configuration
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.RequestId);
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.ReceiverId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasOne(e => e.Request)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Sender)
                    .WithMany(e => e.SentMessages)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Receiver)
                    .WithMany(e => e.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Rating Configuration
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasIndex(e => e.AgentId);
                entity.HasIndex(e => e.RaterId);
                entity.HasOne(e => e.Agent)
                    .WithMany(e => e.Ratings)
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Rater)
                    .WithMany(e => e.GivenRatings)
                    .HasForeignKey(e => e.RaterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Document Configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DocumentType);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Favorite Configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasIndex(e => e.CargoOwnerId);
                entity.HasIndex(e => e.AgentId);
                entity.HasIndex(e => new { e.CargoOwnerId, e.AgentId }).IsUnique();
                entity.HasOne(e => e.CargoOwner)
                    .WithMany()
                    .HasForeignKey(e => e.CargoOwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Agent)
                    .WithMany()
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Service Configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(e => e.AgentId);
                entity.HasOne(e => e.Agent)
                    .WithMany()
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // VerificationRequest Configuration
            modelBuilder.Entity<VerificationRequest>(entity =>
            {
                entity.HasIndex(e => e.AgentId);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasOne(e => e.Agent)
                    .WithMany()
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ReviewedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ReviewReply Configuration
            modelBuilder.Entity<ReviewReply>(entity =>
            {
                entity.HasIndex(e => e.RatingId);
                entity.HasOne(e => e.Rating)
                    .WithMany(e => e.Replies)
                    .HasForeignKey(e => e.RatingId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.RepliedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RepliedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivityLog Configuration
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Action);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Announcement Configuration
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

