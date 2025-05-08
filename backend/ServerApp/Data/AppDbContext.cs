using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserData> UserData { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAprrove> UserAprrove { get; set; }
        public DbSet<Subject> Subjects { get; set; } // Добавлено
        public DbSet<Grade> Grades { get; set; }     // Добавлено
        public DbSet<TeacherData> TeacherData { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherData>()
                .HasOne(td => td.User)
                .WithMany(u => u.SubjectsTaught)
                .HasForeignKey(td => td.UserId);

            modelBuilder.Entity<TeacherData>()
                .HasOne(td => td.Subject)
                .WithMany(s => s.Teachers)
                .HasForeignKey(td => td.SubjectId);


            // UserData <-> User
            modelBuilder.Entity<UserData>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserApprove <-> User
            modelBuilder.Entity<UserAprrove>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Grade -> Student
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade -> Teacher
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany()
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade -> Subject
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany()
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
