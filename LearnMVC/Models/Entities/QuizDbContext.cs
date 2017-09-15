using System;
using LearnMVC.Models.View_Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LearnMVC.Models.Entities
{
    public partial class QuizDbContext : DbContext
    {
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Progress> Progress { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuizUnit> QuizUnit { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=parskyserver.database.windows.net;Initial Catalog=Parsky;Integrated Security=False;User ID=Parsky;Password=Johan1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("Answer", "Quiz");

                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.AnswerText).IsRequired();

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__Answer__Question__14270015");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "Quiz");

                entity.HasIndex(e => e.Order)
                    .HasName("UQ__Category__67A3D86C55BAA82F")
                    .IsUnique();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member", "Quiz");

                entity.Property(e => e.MemberId)
                    .HasColumnName("MemberID")
                    .ValueGeneratedNever();

                entity.Property(e => e.IsAdmin).HasDefaultValueSql("('false')");
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.ToTable("Progress", "Quiz");

                entity.Property(e => e.ProgressId).HasColumnName("ProgressID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MemberId)
                    .HasColumnName("MemberID")
                    .HasMaxLength(450);

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Progress)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__Progress__Questi__1DB06A4F");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question", "Quiz");

                entity.HasIndex(e => new { e.Order, e.QuizUnitId })
                    .HasName("UniqueOrder")
                    .IsUnique();

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.QuestionText).IsRequired();

                entity.Property(e => e.QuizUnitId).HasColumnName("QuizUnitID");

                entity.HasOne(d => d.QuizUnit)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.QuizUnitId)
                    .HasConstraintName("FK__Question__QuizUn__114A936A");
            });

            modelBuilder.Entity<QuizUnit>(entity =>
            {
                entity.ToTable("QuizUnit", "Quiz");

                entity.HasIndex(e => new { e.Order, e.CategoryId })
                    .HasName("UniqueQuizUnitOrder")
                    .IsUnique();

                entity.Property(e => e.QuizUnitId).HasColumnName("QuizUnitID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.QuizUnit)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__QuizUnit__Catego__0E6E26BF");
            });
        }

    }
}
