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

        public List<User> GetUsers()
        {
            var users = _context.Users
                .Include(u => u.Status)
                .ToList();
            return users;
        }

    }
}
