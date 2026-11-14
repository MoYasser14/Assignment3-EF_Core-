using Assignment3_EF_Core_.Data;
using Assignment3_EF_Core_.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3_EF_Core_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();

            context.Database.EnsureCreated();
            Console.WriteLine("=== Database Initialized Successfully ===\n");

            SeedStudentsAndEnrollments(context);

            Console.WriteLine("\n--- [CREATE OPERATIONS] ---");
            AddInstructor(context, "Moustafa Yasser", "moustafa@example.com", "Software Architect");
            AddCourse(context, "Advanced Microservices", 1, 3, 149.99m, CourseLevel.Advanced);
            AddMultipleStudents(context);
            EnrollStudent(context, studentId: 1, courseId: 1);
            EnrollStudent(context, studentId: 1, courseId: 4);

            Console.WriteLine("\n--- [READ OPERATIONS] ---");
            FindCourse(context, 1, "Advanced Microservices");
            FilterCoursesByPrice(context, 40.0m, 100.0m);
            DisplayCategoriesHierarchy(context, 1);
            DisplayCourseRoster(context, 1);

            Console.WriteLine("\n--- [UPDATE OPERATIONS] ---");
            ChangeCoursePrice(context, 1, 55.00m);
            UpdateEnrollmentProgress(context, 1, 1, 100);
            PublishCourse(context, 4);

            Console.WriteLine("\n--- [DELETE OPERATIONS] ---");
            UnenrollStudent(context, 1, 1);
            TryDeleteCategoryWithSubCategories(context, 1);
            DeleteCategorySafely(context, 5);
            DeleteCourseWithEnrollments(context, 3);

            Console.WriteLine("\n--- [BULK OPERATIONS] ---");
            BulkPriceRaise(context, CourseLevel.Beginner, 0.10m);
            BulkDeleteDroppedEnrollments(context);

            Console.WriteLine("\n=== All Operations Executed Successfully ===");
        }

        private static void SeedStudentsAndEnrollments(AppDbContext context)
        {
            if (!context.Students.Any())
            {
                var students = new List<Student>
                {
                    new Student { FullName = "Ali Mohamed", Email = "ali@example.com", DateOfBirth = new DateTime(2002, 5, 12) },
                    new Student { FullName = "Mona Ahmed", Email = "mona@example.com", DateOfBirth = new DateTime(2003, 8, 22) },
                    new Student { FullName = "Tarek Ziad", Email = "tarek@example.com", DateOfBirth = new DateTime(2001, 11, 3) },
                    new Student { FullName = "Nourhan Ezzat", Email = "nour@example.com", DateOfBirth = new DateTime(2004, 1, 19) },
                    new Student { FullName = "Youssef Hassan", Email = "youssef@example.com", DateOfBirth = new DateTime(2000, 7, 30) }
                };
                context.Students.AddRange(students);
                context.SaveChanges();

                var enrollments = new List<Enrollment>
                {
                    new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = DateTime.Now.AddDays(-10), ProgressPercentage = 45, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = 2, CourseId = 2, EnrollmentDate = DateTime.Now.AddDays(-20), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 95.5m },
                    new Enrollment { StudentId = 3, CourseId = 3, EnrollmentDate = DateTime.Now.AddDays(-5), ProgressPercentage = 15, Status = EnrollmentStatus.Dropped }
                };
                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
                Console.WriteLine("-> Students and initial Enrollments seeded successfully.");
            }
        }

        private static void AddInstructor(AppDbContext context, string name, string email, string bio)
        {
            var instructor = new Instructor { FullName = name, Email = email, Bio = bio, JoinedDate = DateTime.Now };
            context.Instructors.Add(instructor);
            context.SaveChanges();
            Console.WriteLine($"[Add Instructor] Success! Generated Id: {instructor.Id}");
        }

        private static void AddCourse(AppDbContext context, string title, int instructorId, int categoryId, decimal price, CourseLevel level)
        {
            var instructor = context.Instructors.Find(instructorId);
            var category = context.Categories.Find(categoryId);

            if (instructor == null || category == null)
            {
                Console.WriteLine("[Add Course] Failed: Instructor or Category does not exist.");
                return;
            }

            var course = new Course
            {
                Title = title,
                InstructorId = instructorId,
                CategoryId = categoryId,
                Price = price,
                Level = level,
                IsPublished = false
            };

            context.Courses.Add(course);
            context.SaveChanges();
            Console.WriteLine($"[Add Course] Success! Course '{title}' added with Id: {course.Id}");
        }

        private static void AddMultipleStudents(AppDbContext context)
        {
            var newStudents = new List<Student>
            {
                new Student { FullName = "Hoda Kamel", Email = "hoda@example.com", DateOfBirth = new DateTime(2003, 3, 14) },
                new Student { FullName = "Karim Raafat", Email = "karim@example.com", DateOfBirth = new DateTime(2002, 9, 19) },
                new Student { FullName = "Dina Samir", Email = "dina@example.com", DateOfBirth = new DateTime(2005, 12, 5) }
            };

            context.Students.AddRange(newStudents);
            context.SaveChanges();
            Console.WriteLine($"[Add Multiple Students] Success! Added IDs: {newStudents[0].Id}, {newStudents[1].Id}, {newStudents[2].Id}");
        }

        private static void EnrollStudent(AppDbContext context, int studentId, int courseId)
        {
            var student = context.Students.Find(studentId);
            var course = context.Courses.Find(courseId);

            if (student == null || course == null)
            {
                Console.WriteLine($"[Enrollment] Refused: Student (Id: {studentId}) or Course (Id: {courseId}) not found.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"[Enrollment] Refused: Course '{course.Title}' is not published yet.");
                return;
            }

            bool alreadyEnrolled = context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
            if (alreadyEnrolled)
            {
                Console.WriteLine($"[Enrollment] Refused: Student is already enrolled in this course.");
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active
            };

            context.Enrollments.Add(enrollment);
            context.SaveChanges();
            Console.WriteLine($"[Enrollment] Success! Student {student.FullName} enrolled in {course.Title}.");
        }

        private static void FindCourse(AppDbContext context, int id, string title)
        {
            var courseById = context.Courses.Find(id);
            var courseByTitle = context.Courses.FirstOrDefault(c => c.Title == title);

            Console.WriteLine($"[Find Course by Id {id}]: {(courseById != null ? courseById.Title : "not found")}");
            Console.WriteLine($"[Find Course by Title '{title}']: {(courseByTitle != null ? $"Found with ID {courseByTitle.Id}" : "not found")}");
        }

        private static void FilterCoursesByPrice(AppDbContext context, decimal minPrice, decimal maxPrice)
        {
            var courses = context.Courses
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"\n[Filter Courses between {minPrice} and {maxPrice}]:");
            foreach (var c in courses)
            {
                Console.WriteLine($"- Title: {c.Title} | Level: {c.Level} | Price: ${c.Price}");
            }
        }

        private static void DisplayCategoriesHierarchy(AppDbContext context, int categoryId)
        {
            var category = context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                Console.WriteLine("[Categories] Category not found.");
                return;
            }

            Console.WriteLine($"\n[Sub-categories of '{category.Name}']:");
            foreach (var sub in category.SubCategories)
            {
                Console.WriteLine($" -> {sub.Name}: {sub.Description}");
            }

            var topLevels = context.Categories.Where(c => c.ParentId == null).ToList();
            Console.WriteLine("\n[Top-Level Categories]:");
            foreach (var top in topLevels)
            {
                Console.WriteLine($" -> {top.Name}");
            }
        }

        private static void DisplayCourseRoster(AppDbContext context, int courseId)
        {
            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            var students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            Console.WriteLine($"\n[Course Roster for Course ID {courseId}]: Total Enrolled = {students.Count}");
            foreach (var s in students)
            {
                Console.WriteLine($" - Student: {s.FullName} ({s.Email})");
            }
        }

        private static void ChangeCoursePrice(AppDbContext context, int courseId, decimal newPrice)
        {
            var course = context.Courses.Find(courseId);
            if (course != null)
            {
                course.Price = newPrice;
                context.SaveChanges();
                Console.WriteLine($"[Update Price] Course ID {courseId} price changed to ${newPrice}");
            }
            else
            {
                Console.WriteLine("[Update Price] Course not found.");
            }
        }

        private static void UpdateEnrollmentProgress(AppDbContext context, int studentId, int courseId, int progress)
        {
            if (progress < 0 || progress > 100)
            {
                Console.WriteLine("[Update Progress] Rejected: Progress must be between 0 and 100.");
                return;
            }

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine("[Update Progress] Enrollment not found.");
                return;
            }

            enrollment.ProgressPercentage = progress;
            if (progress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.FinalGrade = 90.0m;
            }

            context.SaveChanges();
            Console.WriteLine($"[Update Progress] Success! Progress set to {progress}%. Status: {enrollment.Status}");
        }

        private static void PublishCourse(AppDbContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course != null)
            {
                course.IsPublished = true;
                course.PublishedDate = DateTime.Now;
                context.SaveChanges();
                Console.WriteLine($"[Publish Course] Course '{course.Title}' is now published.");
            }
            else
            {
                Console.WriteLine("[Publish Course] Course not found.");
            }
        }

        private static void UnenrollStudent(AppDbContext context, int studentId, int courseId)
        {
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment != null)
            {
                context.Enrollments.Remove(enrollment);
                context.SaveChanges();
                Console.WriteLine($"[Unenroll] Student ID {studentId} successfully unenrolled from course ID {courseId}.");
            }
            else
            {
                Console.WriteLine("[Unenroll] Enrollment not found.");
            }
        }

        private static void TryDeleteCategoryWithSubCategories(AppDbContext context, int categoryId)
        {
            try
            {
                var category = context.Categories.Find(categoryId);
                if (category != null)
                {
                    context.Categories.Remove(category);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Delete Category Exception Handled]: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private static void DeleteCategorySafely(AppDbContext context, int categoryId)
        {
            var category = context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                Console.WriteLine("[Safe Delete Category] Category not found.");
                return;
            }

            if (category.SubCategories.Any())
            {
                Console.WriteLine($"[Safe Delete Category] Rejected: Category '{category.Name}' still has sub-categories and cannot be deleted.");
                return;
            }

            if (category.Courses.Any())
            {
                Console.WriteLine($"[Safe Delete Category] Rejected: Category '{category.Name}' still contains {category.Courses.Count} active courses and cannot be deleted.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();
            Console.WriteLine($"[Safe Delete Category] Category '{category.Name}' deleted successfully.");
        }

        private static void DeleteCourseWithEnrollments(AppDbContext context, int courseId)
        {
            int countBefore = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"[Delete Course] Enrollments count before deleting course: {countBefore}");

            context.Enrollments.Where(e => e.CourseId == courseId).ExecuteDelete();
            context.Courses.Where(c => c.Id == courseId).ExecuteDelete();

            int countAfter = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"[Delete Course] Enrollments count after deleting course: {countAfter} (Removed via ExecuteDelete).");
        }

        private static void BulkPriceRaise(AppDbContext context, CourseLevel level, decimal percentage)
        {
            int affectedRows = context.Courses
                .Where(c => c.Level == level && c.IsPublished)
                .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * (1 + percentage)));

            Console.WriteLine($"[Bulk Price Raise] Affected rows: {affectedRows}");
        }

        private static void BulkDeleteDroppedEnrollments(AppDbContext context)
        {
            int affectedRows = context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDelete();

            Console.WriteLine($"[Bulk Delete Dropped] Affected rows: {affectedRows}");
        }
    }
}