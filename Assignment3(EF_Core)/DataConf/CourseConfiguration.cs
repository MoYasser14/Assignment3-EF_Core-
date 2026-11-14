using Assignment3_EF_Core_.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment3_EF_Core_.Data
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).HasMaxLength(1000);
            builder.Property(c => c.Price).HasColumnType("decimal(18,2)");

            builder.HasOne(c => c.Instructor)
                   .WithMany(i => i.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Course { Id = 1, Title = "Mastering C#", Description = "Complete C# course", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2023, 2, 1), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 2, Title = "Advanced ASP.NET Core", Description = "Build enterprise APIs", Price = 99.99m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2023, 4, 5), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 3, Title = "Python for Data Science", Description = "Data analysis fundamentals", Price = 79.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2023, 5, 10), InstructorId = 2, CategoryId = 5 },
                new Course { Id = 4, Title = "React Basics", Description = "Frontend UI development", Price = 59.99m, Level = CourseLevel.Beginner, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 4 },
                new Course { Id = 5, Title = "Machine Learning Intro", Description = "AI models and algorithms", Price = 120.00m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2023, 7, 15), InstructorId = 2, CategoryId = 5 },
                new Course { Id = 6, Title = "Entity Framework Core Deep Dive", Description = "ORM mastery", Price = 89.99m, Level = CourseLevel.Intermediate, IsPublished = false, PublishedDate = null, InstructorId = 1, CategoryId = 3 }
            );
        }
    }
}