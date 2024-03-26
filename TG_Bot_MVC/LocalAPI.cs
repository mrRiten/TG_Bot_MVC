using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace TG_Bot_MVC
{
    public class LocalAPI
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

        public void SetStatus(long userIdTg, int statusId)
        {
            var user = _context.Users.Where(u => u.UserTGId == userIdTg).FirstOrDefault();
            user.StatusId = statusId;

            _context.Users.Update(user);
            _context.SaveChanges(); 
        }

        public void SetBan(long userTgId, bool banned)
        {
            var user = _context.Users.Where(u => u.UserTGId == userTgId).FirstOrDefault();

            user.IsBanned = banned;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void AddUser(string userName, long userTGId)
        {
            var user = new User
            {
                UserName = userName,
                UserTGId = userTGId,
                IsBanned = false
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

        public User? TryGetUser(long userIdTg, string userName)
        {
            var user = _context.Users.Where(u => u.UserTGId == userIdTg).FirstOrDefault();
            if (user is null)
            {
                AddUser(userName, userIdTg);
            }
            return _context.Users.Where(u => u.UserTGId == userIdTg).Include(u => u.Status).FirstOrDefault();
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

        public void AddReplasementLesson(int groupId, int weekOfScheduleId, string serializeDataLesson, int weekDay)
        {
            var replasementLesson = new ReplasementLesson
            {
                GroupId = groupId,
                WeekOfScheduleId = weekOfScheduleId,
                SerializeDataLessons = serializeDataLesson,
                Weekday = weekDay
            };

            _context.ReplasementLessons.Add(replasementLesson);
            _context.SaveChanges();
        }

        public void DelReplasementLessons(int weekDay)
        {
            var schedule = _context.ReplasementLessons.Where(rl => rl.Weekday == weekDay);

            if (schedule.Any())
            {
                _context.ReplasementLessons.RemoveRange(schedule);
                _context.SaveChanges();
            }
        }

        public void AddDefaultSchedule(int IdGroup, int weekOfScheduleId, string serializeDataLesson, int weekDay)
        {
            var schedule = new DefaultSchedule
            {
                GroupId = IdGroup,
                WeekOfScheduleId = weekOfScheduleId,
                Weekday = weekDay,
                SerializeDataLessons = serializeDataLesson
            };

            _context.DefaultSchedules.Add(schedule);
            _context.SaveChanges();
        }

        public DefaultSchedule? GetDefaultSchedule(int IdGroup, int weekday)
        {
            return _context.DefaultSchedules.FirstOrDefault(ds => ds.GroupId == IdGroup && ds.Weekday == weekday);
        }

        public CorrectSchedule? GetCorrectSchedule(int IdGroup, int weekday)
        {
            return _context.CorrectSchedules.FirstOrDefault(cs => cs.GroupId == IdGroup && cs.Weekday == weekday);
        }

        public void AddCorrectSchedule(int IdGoup, int weekOfScheduleId, string serializeDataLesson, int weekDay)
        {
            var correctSchedule = new CorrectSchedule
            {
                GroupId = IdGoup,
                WeekOfScheduleId = weekOfScheduleId,
                Weekday = weekDay,
                SerializeDataLessons = serializeDataLesson
            };

            _context.CorrectSchedules.Add(correctSchedule);
            _context.SaveChanges();
        }

        public void DelCorrectSchedules(int weekDay)
        {
            var schedules = _context.CorrectSchedules.Where(cs => cs.Weekday == weekDay);

            if (schedules.Any())
            {
                _context.CorrectSchedules.RemoveRange(schedules);
                _context.SaveChanges();
            }
        }

        public User? GetFullUser(long userTgId)
        {
            return _context.Users
                .Include(u => u.Status)
                .Include(u => u.Setting)
                .FirstOrDefault(u => u.UserTGId == userTgId);
        }

        public User? GetFullInfoUser(long userTgId)
        {
            return _context.Users
                .Include(u => u.Status)
                .Include(u => u.Setting)
                .ThenInclude(u => u.Group)
                .ThenInclude(g => g.Department)
                .FirstOrDefault(u => u.UserTGId == userTgId);
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

        public int GetMaxIdGroup()
        {
            return _context.Groups
                .OrderByDescending(g => g.IdGroup)
                .FirstOrDefault().IdGroup;
        }
    }
}
