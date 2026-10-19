using Microsoft.EntityFrameworkCore;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.Domain_Models;

namespace Student_Info_Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student_DM>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<User_DM>()
                .HasKey(x => x.UserId);
            modelBuilder.Entity<User_DM>()
            .HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<User_DM>()
                .Property(x => x.PasswordHash).IsRequired();
            modelBuilder.Entity<User_DM>()
                .Property(x => x.Roles).IsRequired();
            modelBuilder.Entity<User_DM>()
               .Property(x => x.UserName).IsRequired();

            modelBuilder.Entity<Student_DM>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student_DM>(x => x.UserId);

            modelBuilder.Entity<TokenDataGeneration_DM>()
                .HasOne<User_DM>()
                .WithMany()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<User_DM>()
                .HasIndex(x => x.Email)
                .IsUnique();                
        }

        public DbSet<Student_DM> Students { get; set; }
        public DbSet<Course_DM> Courses { get; set; }
        public DbSet<User_DM> Users { get; set; }
        public DbSet<TokenDataGeneration_DM> PasswordResetToken { get; set; }

    }
}