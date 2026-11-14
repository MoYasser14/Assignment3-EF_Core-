using Assignment3_EF_Core_.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment3_EF_Core_.Data
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);

            builder.HasOne(c => c.Parent)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Category { Id = 1, Name = "Programming", Description = "General programming topics", ParentId = null }, 
                new Category { Id = 2, Name = "Backend", Description = "Server-side development", ParentId = 1 },          
                new Category { Id = 3, Name = "ASP.NET Core", Description = "Web frameworks with C#", ParentId = 2 },       
                new Category { Id = 4, Name = "Frontend", Description = "Client-side development", ParentId = 1 },        
                new Category { Id = 5, Name = "Data Science", Description = "AI and Data analysis", ParentId = null }   
            );
        }
    }
}