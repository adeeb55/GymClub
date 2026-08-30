namespace GymClub.Models
{
    public class Schedule
    {
        public int ScheduleId { set; get; }

        // 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday
        // (نفس ترقيم System.DayOfWeek بالـ C# حتى نقارنها مباشرة مع DateTime.Now.DayOfWeek)
        public int DayOfWeek { set; get; }

        public int MemberId { set; get; }
        public Member Member { set; get; }

        public int ExerciseId { set; get; }
        public Exercise Exercise { set; get; }
    }
}
