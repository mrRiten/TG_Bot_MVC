using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return _context.WeekOfSchedules.FirstOrDefault(w => w.WeekOfScheduleName ==  weekOfScheduleName).IdWeekOfSchedule;
        }

        public ReplasementLesson? GetReplasementLesson(int groupId)
        {
            return _context.ReplasementLessons
                .Include(r => r.Group)
                .Include(r => r.WeekOfSchedule)
                .FirstOrDefault(r => r.GroupId == groupId);
        }

        public void AddReplasementLesson(int groupId, int weekOfScheduleId, string serializeDataLesson)
        {
            var replasementLesson = new ReplasementLesson
            {
                GroupId = groupId,
                WeekOfScheduleId = weekOfScheduleId,
                Weekday = (int)DateTime.Today.DayOfWeek,
                SerializeDataLessons = serializeDataLesson
            };

            _context.ReplasementLessons.Add(replasementLesson);
            _context.SaveChanges();
        }

    }
}
