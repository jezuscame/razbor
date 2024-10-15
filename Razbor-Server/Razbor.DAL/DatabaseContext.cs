using Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Razbor.DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<UserInfoTable> UserInfoTable { get; set; }

        public DbSet<ChatTable> ChatTable { get; set; }

        public DbSet<MatchTable> MatchTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfoTable>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.UserGender).IsRequired();
                entity.Property(e => e.Birthday).IsRequired();
            });

            modelBuilder.Entity<MatchTable>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.OriginUserTable)
                    .WithMany(u => u.DestinationMatches)
                    .HasForeignKey(m => m.OriginUser)
                    .HasPrincipalKey(u => u.Username);

                entity.HasOne(e => e.DestinationUserTable)
                    .WithMany(u => u.OriginMatches)
                    .HasForeignKey(m => m.DestinationUser)
                    .HasPrincipalKey(u => u.Username);


                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.OriginUser).IsRequired();
                entity.Property(e => e.OriginMatchStatus).IsRequired();
                entity.Property(e => e.DestinationUser).IsRequired();
                entity.Property(e => e.DestinationMatchStatus).IsRequired();
            });

            modelBuilder.Entity<ChatTable>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Match)
                    .WithMany(m => m.Chat)
                    .HasForeignKey(e => e.MatchId);

                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Sender).IsRequired();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.MatchId).IsRequired();
                entity.Property(e => e.Date).IsRequired();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}