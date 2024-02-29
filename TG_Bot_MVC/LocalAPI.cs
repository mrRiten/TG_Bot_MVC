using Microsoft.EntityFrameworkCore;

namespace TG_Bot_MVC
{
    internal class LocalAPI
    {
        private readonly LibraryContext _context;

        public LocalAPI(LibraryContext context)
        {
            _context = context;
        }

        public void AddStatus(string statusName)
        {
            var status = new Status
            {
                StatusName = statusName,
            };

            _context.Statuses.Add(status);
            _context.SaveChanges();
        }

        public void AddUser(string username, long userTGId)
        {
            var user = new User
            {
                UserName = username,
                UserTGId = userTGId,
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? GetUser(int userId)
        {
            return _context.Users
                .Include(u => u.Status)
                .FirstOrDefault(u => u.IdUser == userId);
        }

        public int TryGetGroupId(string groupName)
        {
            var group = _context.Groups.FirstOrDefault(g => g.GroupName == groupName);
            if (group is null)
            {
                AddGroup(groupName);
            }
            return _context.Groups.FirstOrDefault(g => g.GroupName == groupName).IdGroup;
        }

        public void AddGroup(string groupName)
        {
            var newGroup = new Group
            {
                GroupName = groupName,
            };

            _context.Groups.Add(newGroup);
            _context.SaveChanges();
        }

        public int GetWeekOfScheduleId(string weekOfScheduleName)
        {
            return _context.WeekOfSchedules.FirstOrDefault(w => w.WeekOfScheduleName == weekOfScheduleName).IdWeekOfSchedule;
        }

        public ReplasementLesson? GetReplasementLesson(int groupId, int weekday)
        {
            return _context.ReplasementLessons
                .Include(r => r.Group)
                .Include(r => r.WeekOfSchedule)
                .FirstOrDefault(r => r.GroupId == groupId && r.Weekday == weekday);
        }

        public void AddReplasementLesson(int groupId, int weekOfScheduleId, string serializeDataLesson, DateTime dateTime)
        {
            var replasementLesson = new ReplasementLesson
            {
                GroupId = groupId,
                WeekOfScheduleId = weekOfScheduleId,
                Weekday = (int)dateTime.DayOfWeek,
                SerializeDataLessons = serializeDataLesson
            };

            _context.ReplasementLessons.Add(replasementLesson);
            _context.SaveChanges();
        }

        public void DelReplasementLessons(DateTime dateTime)
        {
            var schedule = _context.ReplasementLessons.Where(rl => rl.Weekday == (int)dateTime.DayOfWeek);

            if (schedule.Any())
            {
                _context.ReplasementLessons.RemoveRange(schedule);
                _context.SaveChanges();
            }
        }

        public DefaultSchedule? GetDefaultSchedule(int IdGroup, int weekday)
        {
            return _context.DefaultSchedules.FirstOrDefault(ds => ds.GroupId == IdGroup && ds.Weekday == weekday);
        }

        public CorrectSchedule? GetCorrectSchedule(int IdGroup, int weekday)
        {
            return _context.CorrectSchedules.FirstOrDefault(cs => cs.GroupId == IdGroup && cs.Weekday == weekday);
        }

        public void SetCorrectSchedule(int IdGoup, int weekOfScheduleId, string serializeDataLesson, DateTime dateTime)
        {
            var correctSchedule = new CorrectSchedule
            {
                GroupId = IdGoup,
                WeekOfScheduleId = weekOfScheduleId,
                Weekday = (int)dateTime.DayOfWeek,
                SerializeDataLessons = serializeDataLesson
            };

            _context.CorrectSchedules.Add(correctSchedule);
            _context.SaveChanges();
        }

        public void DelCorrectSchedules(DateTime dateTime)
        {
            var schedules = _context.CorrectSchedules.Where(cs => cs.Weekday == (int)dateTime.DayOfWeek);

            if (schedules.Any())
            {
                _context.CorrectSchedules.RemoveRange(schedules);
                _context.SaveChanges();
            }
        }

        public User? GetFullUser(int IdUser)
        {
            return _context.Users
                .Include(u => u.Status)
                .Include(u => u.Setting)
                .FirstOrDefault(u => u.IdUser == IdUser);
        }

        public User? GetFullInfoUser(int IdUser)
        {
            return _context.Users
                .Include(u => u.Status)
                .Include(u => u.Setting)
                .ThenInclude(u => u.Group)
                .ThenInclude(g => g.Department)
                .FirstOrDefault(u => u.IdUser == IdUser);
        }

        public void AddUserSetting(int IdUser, int IdGroup)
        {
            var setting = new Setting
            {
                isMailing = true,
                TimeOfLessons = true,
                UserId = IdUser,
                GroupId = IdGroup
            };

            _context.Settings.Add(setting);
            _context.SaveChanges();
        }

        public void SetMailingSetting(int IdUser, bool isMailing)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.UserId == IdUser);
            setting.isMailing = isMailing;

            _context.SaveChanges();
        }

        public void SetTimeOfLessonsSetting(int IdUser, bool isTimeOfLessons)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.UserId == IdUser);
            setting.TimeOfLessons = isTimeOfLessons;

            _context.SaveChanges();
        }

        public void SetGroupSetting(int IdUser, int idGroup)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.UserId == IdUser);
            setting.GroupId = idGroup;

            _context.SaveChanges();
        }
    }
}
