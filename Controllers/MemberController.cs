using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymClub.Models;

namespace GymClub.Controllers
{
    public class MemberController : Controller
    {
        private GymContext context { set; get; }

        public MemberController(GymContext context)
        {
            this.context = context;
        }

        // بيرجع اسم تمرين اليوم لمشترك معين (حسب يوم الأسبوع الحالي)
        private string GetTodayExercise(Member m)
        {
            int today = (int)DateTime.Now.DayOfWeek;
            Schedule? sc = m.Schedules?.FirstOrDefault(s => s.DayOfWeek == today);
            return sc != null ? sc.Exercise.Name : "لا يوجد جدول";
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(int mid, string mpass)
        {
            Member? m = context.Members.Where(m => m.MemberId == mid && m.Password == mpass).FirstOrDefault();
            if (m != null)
            {
                HttpContext.Session.SetInt32("mid", m.MemberId);

                if (m.IsAdmin)
                    return RedirectToAction("ShowMembers");
                else
                    return RedirectToAction("ShowMyProfile");
            }
            else
                return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        // -------- صفحة المشترك: يشوف حالو بس (اشتراكه + شو رح يلعب اليوم) --------
        public IActionResult ShowMyProfile()
        {
            int? mid = HttpContext.Session.GetInt32("mid");
            if (mid == null)
                return RedirectToAction("Login");

            Member? m = context.Members.Include(mm => mm.Schedules).ThenInclude(s => s.Exercise)
                                        .FirstOrDefault(mm => mm.MemberId == mid);

            if (m == null)
                return RedirectToAction("Login");

            ViewBag.TodayExercise = GetTodayExercise(m);
            return View("ShowMyProfile", m);
        }

        // -------- صفحة الأدمن (صاحب النادي): يشوف كل المشتركين --------
        public IActionResult ShowMembers()
        {
            int? mid = HttpContext.Session.GetInt32("mid");
            Member? admin = context.Members.Find(mid);

            if (admin == null || !admin.IsAdmin)
                return RedirectToAction("Login");

            List<Member> members = context.Members
                                           .Where(m => !m.IsAdmin)
                                           .Include(m => m.Schedules).ThenInclude(s => s.Exercise)
                                           .ToList();

            Dictionary<int, string> todayMap = new Dictionary<int, string>();
            foreach (Member m in members)
                todayMap[m.MemberId] = GetTodayExercise(m);

            ViewBag.TodayMap = todayMap;
            return View("ShowMembers", members);
        }

        public IActionResult Search(string key1)
        {
            int? mid = HttpContext.Session.GetInt32("mid");
            Member? admin = context.Members.Find(mid);
            if (admin == null || !admin.IsAdmin)
                return RedirectToAction("Login");

            List<Member> members = context.Members
                                           .Where(m => !m.IsAdmin && m.Name.Contains(key1))
                                           .Include(m => m.Schedules).ThenInclude(s => s.Exercise)
                                           .ToList();

            Dictionary<int, string> todayMap = new Dictionary<int, string>();
            foreach (Member m in members)
                todayMap[m.MemberId] = GetTodayExercise(m);

            ViewBag.TodayMap = todayMap;
            return View("ShowMembers", members);
        }

        // -------- إضافة مشترك جديد (أدمن فقط) --------
        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpPost]
        public IActionResult Add(Member m)
        {
            context.Members.Add(m);
            context.SaveChanges();

            // ننشئ جدول أسبوعي كامل (7 أيام) لهذا المشترك الجديد، افتراضياً "Rest Day"
            for (int day = 0; day < 7; day++)
            {
                context.Schedules.Add(new Schedule
                {
                    MemberId = m.MemberId,
                    DayOfWeek = day,
                    ExerciseId = 7 // Rest Day
                });
            }
            context.SaveChanges();

            return RedirectToAction("ShowMembers");
        }

        // -------- تعديل بيانات مشترك (أدمن فقط) --------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Member? m = context.Members.Find(id);
            return View("Edit", m);
        }

        [HttpPost]
        public IActionResult Edit(Member m)
        {
            context.Members.Update(m);
            context.SaveChanges();
            return RedirectToAction("ShowMembers");
        }

        public IActionResult delete(int id)
        {
            Member? m = context.Members.Find(id);
            if (m != null)
            {
                context.Members.Remove(m);
                context.SaveChanges();
            }
            return RedirectToAction("ShowMembers");
        }

        // -------- تعديل الجدول الأسبوعي لمشترك معين (أدمن فقط) --------
        [HttpGet]
        public IActionResult EditSchedule(int id)
        {
            Member? m = context.Members.Include(mm => mm.Schedules).ThenInclude(s => s.Exercise)
                                        .FirstOrDefault(mm => mm.MemberId == id);
            ViewBag.exercises = context.Exercises.OrderBy(e => e.ExerciseId).ToList();
            return View("EditSchedule", m);
        }

        [HttpPost]
        public IActionResult EditSchedule(int memberId, int[] exerciseId)
        {
            // بنعدل أي صف موجود، وبننشئ أي صف ناقص (لأي مشترك اتضاف قبل هذا الإصلاح)
            for (int day = 0; day < exerciseId.Length; day++)
            {
                Schedule? sc = context.Schedules.FirstOrDefault(s => s.MemberId == memberId && s.DayOfWeek == day);
                if (sc != null)
                {
                    sc.ExerciseId = exerciseId[day];
                    context.Schedules.Update(sc);
                }
                else
                {
                    context.Schedules.Add(new Schedule
                    {
                        MemberId = memberId,
                        DayOfWeek = day,
                        ExerciseId = exerciseId[day]
                    });
                }
            }
            context.SaveChanges();
            return RedirectToAction("ShowMembers");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("mid") == null)
                return RedirectToAction("Login");
            return View("ChangePassword");
        }

        [HttpPost]
        public IActionResult ChangePassword(string newPass)
        {
            int? mid = HttpContext.Session.GetInt32("mid");
            Member m = context.Members.Find(mid);

            m.Password = newPass;

            context.Members.Update(m);
            context.SaveChanges();

            return RedirectToAction("ShowMyProfile");
        }
    }
}