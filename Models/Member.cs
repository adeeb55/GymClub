namespace GymClub.Models
{
    public class Member
    {
        public int MemberId { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public bool IsAdmin { set; get; }
        public DateTime SubscriptionStart { set; get; }
        public DateTime SubscriptionEnd { set; get; }
        public List<Schedule> Schedules { set; get; }
    }
}
