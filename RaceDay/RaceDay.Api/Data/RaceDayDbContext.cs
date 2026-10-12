// start of RaceDayDbContext
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public class RaceDayDbContext : DbContext
    {
  
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {
        }


        public DbSet<User> Users => Set<User>();
        public DbSet<EventType> EventTypes => Set<EventType>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // start of Users configuration
            modelBuilder.Entity<User>(entity =>
            {
                // no two users can share an email
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_Users_Email");

                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

                // Role can only be Organiser or Participant
                entity.ToTable(t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Organiser', 'Participant')"));
            });
            // end of Users configuration

            // start of EventTypes configuration
            modelBuilder.Entity<EventType>(entity =>
            {
                entity.HasIndex(t => t.EventTypeName).IsUnique().HasDatabaseName("UQ_EventTypes_Name");

                // seed the three event types so events can be created straight away
                entity.HasData(
                    new EventType { EventTypeId = 1, EventTypeName = "Run" },
                    new EventType { EventTypeId = 2, EventTypeName = "Walk" },
                    new EventType { EventTypeId = 3, EventTypeName = "Cycle" });
            });
            // end of EventTypes configuration

            // start of Events configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Events_Distance", "[Distance] > 0"));

                // Restrict = no cascade delete
                entity.HasOne(e => e.Organiser)
                      .WithMany(u => u.OrganisedEvents)
                      .HasForeignKey(e => e.OrganiserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EventType)
                      .WithMany(t => t.Events)
                      .HasForeignKey(e => e.EventTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // end of Events configuration

            // start of Categories configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Event)
                      .WithMany(e => e.Categories)
                      .HasForeignKey(c => c.EventId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // end of Categories configuration

            // start of Enrolments configuration
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.Property(e => e.EnrolmentDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.EnrolmentStatus).HasDefaultValue("Confirmed");

                // a participant can only enrol once per event
                entity.HasIndex(e => new { e.ParticipantId, e.EventId })
                      .IsUnique()
                      .HasDatabaseName("UQ_Enrolments_Participant_Event");

                // three foreign keys
                entity.HasOne(e => e.Participant)
                      .WithMany(u => u.Enrolments)
                      .HasForeignKey(e => e.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Event)
                      .WithMany(ev => ev.Enrolments)
                      .HasForeignKey(e => e.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Enrolments)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // end of Enrolments configuration

            // start of Results configuration
            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Results_FinishingPosition", "[FinishingPosition] > 0"));


                entity.HasIndex(r => r.EnrolmentId).IsUnique().HasDatabaseName("UQ_Results_EnrolmentId");

                entity.HasOne(r => r.Enrolment)
                      .WithOne(e => e.Result)
                      .HasForeignKey<Result>(r => r.EnrolmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // end of Results configuration
        }
    }
}
// end of RaceDayDbContext