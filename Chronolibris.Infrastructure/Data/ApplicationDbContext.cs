using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Chronolibris.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<Format> Formats { get; set; }
        //public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<SelectionType> SelectionTypes { get; set; }
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<BookFragment> BookFragments { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<ContentParticipation> ContentParticipations { get; set; }
        public DbSet<BookParticipation> BookParticipations { get; set; }

        public DbSet<BookFileStatus> BookFileStatuses { get; set; }

        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonRole> PersonRoles { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewReactions> ReviewReactions { get; set; }
        public DbSet<CommentReactions> CommentReactions { get; set; }
        public DbSet<Selection> Selections { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<ShelfType> ShelfTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagType> TagTypes { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<ReadingProgress> ReadingProgresses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ModerationTask> ModerationTasks { get; set; }
        public DbSet<ReportReasonType> ReportReasons { get; set; }
        public DbSet<ReportStatus> ReportStatuses { get; set; }
        public DbSet<ReportTargetType> ReportTargetTypes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("user_role");
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserToken<long>>().ToTable("user_tokens");

            modelBuilder.Entity<TokenBlacklist>()
                .HasIndex(t => t.Expiry)
                .HasDatabaseName("IX_TokenBlacklist_Expiry");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }
    }
}
