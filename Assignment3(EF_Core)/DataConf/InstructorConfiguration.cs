using Assignment3_EF_Core_.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Assignment3_EF_Core_.Models;

namespace Assignment3_EF_Core_.Data
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.FullName).IsRequired().HasMaxLength(150);
            builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Bio).HasMaxLength(1000);
            builder.Property(i => i.JoinedDate).IsRequired();

            builder.HasData(
                new Instructor { Id = 1, FullName = "Ahmed Ali", Email = "ahmed.ali@example.com", Bio = "Senior .NET Developer", JoinedDate = new DateTime(2023, 1, 15) },
                new Instructor { Id = 2, FullName = "Sara Mahmoud", Email = "sara.mahmoud@example.com", Bio = "Data Science Expert", JoinedDate = new DateTime(2023, 3, 10) },
                new Instructor { Id = 3, FullName = "Khaled Omar", Email = "khaled.omar@example.com", Bio = "Frontend Architect", JoinedDate = new DateTime(2023, 6, 20) }
            );
        }
    }
}