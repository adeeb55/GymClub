using Microsoft.EntityFrameworkCore;

namespace GymClub.Models
{
    public class GymContext : DbContext
    {
        public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -------- Exercises (قائمة التمارين المرجعية) --------
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { ExerciseId = 1, Name = "Chest" },
                new Exercise { ExerciseId = 2, Name = "Back" },
                new Exercise { ExerciseId = 3, Name = "Legs" },
                new Exercise { ExerciseId = 4, Name = "Shoulders" },
                new Exercise { ExerciseId = 5, Name = "Biceps & Triceps" },
                new Exercise { ExerciseId = 6, Name = "Cardio" },
                new Exercise { ExerciseId = 7, Name = "Rest Day" }
            );

            // -------- Members --------
            modelBuilder.Entity<Member>().HasData(
                new Member
                {
                    MemberId = 1,
                    Name = "Ahmad Nassar",
                    Password = "admin123",
                    IsAdmin = true,
                    SubscriptionStart = new DateTime(2026, 1, 1),
                    SubscriptionEnd = new DateTime(2027, 1, 1)
                },
                new Member
                {
                    MemberId = 2,
                    Name = "Khaled Yousef Amer",
                    Password = "123",
                    IsAdmin = false,
                    SubscriptionStart = new DateTime(2026, 6, 1),
                    SubscriptionEnd = new DateTime(2026, 12, 1)
                },
                new Member
                {
                    MemberId = 3,
                    Name = "Sara Adel Hamdan",
                    Password = "456",
                    IsAdmin = false,
                    SubscriptionStart = new DateTime(2026, 8, 1),
                    SubscriptionEnd = new DateTime(2027, 2, 1)
                }
            );

            // -------- Schedules (جدول أسبوعي مختلف لكل مشترك) --------
            modelBuilder.Entity<Schedule>().HasData(
                // Khaled (MemberId = 2)
                new Schedule { ScheduleId = 1, MemberId = 2, DayOfWeek = 0, ExerciseId = 1 }, // Sunday    -> Chest
                new Schedule { ScheduleId = 2, MemberId = 2, DayOfWeek = 1, ExerciseId = 2 }, // Monday    -> Back
                new Schedule { ScheduleId = 3, MemberId = 2, DayOfWeek = 2, ExerciseId = 3 }, // Tuesday   -> Legs
                new Schedule { ScheduleId = 4, MemberId = 2, DayOfWeek = 3, ExerciseId = 4 }, // Wednesday -> Shoulders
                new Schedule { ScheduleId = 5, MemberId = 2, DayOfWeek = 4, ExerciseId = 5 }, // Thursday  -> Biceps & Triceps
                new Schedule { ScheduleId = 6, MemberId = 2, DayOfWeek = 5, ExerciseId = 6 }, // Friday    -> Cardio
                new Schedule { ScheduleId = 7, MemberId = 2, DayOfWeek = 6, ExerciseId = 7 }, // Saturday  -> Rest Day

                // Sara (MemberId = 3) - جدول مختلف تماماً
                new Schedule { ScheduleId = 8, MemberId = 3, DayOfWeek = 0, ExerciseId = 2 },  // Sunday    -> Back
                new Schedule { ScheduleId = 9, MemberId = 3, DayOfWeek = 1, ExerciseId = 1 },  // Monday    -> Chest
                new Schedule { ScheduleId = 10, MemberId = 3, DayOfWeek = 2, ExerciseId = 6 }, // Tuesday   -> Cardio
                new Schedule { ScheduleId = 11, MemberId = 3, DayOfWeek = 3, ExerciseId = 3 }, // Wednesday -> Legs
                new Schedule { ScheduleId = 12, MemberId = 3, DayOfWeek = 4, ExerciseId = 4 }, // Thursday  -> Shoulders
                new Schedule { ScheduleId = 13, MemberId = 3, DayOfWeek = 5, ExerciseId = 5 }, // Friday    -> Biceps & Triceps
                new Schedule { ScheduleId = 14, MemberId = 3, DayOfWeek = 6, ExerciseId = 7 }  // Saturday  -> Rest Day
            );
        }
    }
}
