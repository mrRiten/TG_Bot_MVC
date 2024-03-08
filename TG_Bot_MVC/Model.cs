using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TG_Bot_MVC
{
    public class LibraryContext(bool isDebug) : DbContext
    {
        public readonly bool isDebug = isDebug;

        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<DefaultSchedule> DefaultSchedules { get; set; }
        public DbSet<ReplasementLesson> ReplasementLessons { get; set; }
        public DbSet<CorrectSchedule> CorrectSchedules { get; set; }
        public DbSet<WeekOfSchedule> WeekOfSchedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigWorker configWorker = new();
            if (isDebug)
            {
                optionsBuilder.UseMySQL(configWorker.GetDebugConnectionString());
            }
            else
            {
                optionsBuilder.UseMySQL(configWorker.GetReleaseConnectionString());
            }

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Одно к одному между User и Setting
            modelBuilder.Entity<User>()
                .HasOne(u => u.Setting)
                .WithOne(s => s.User)
                .HasForeignKey<Setting>(s => s.UserId);

            // Многие к одному между User и Status
            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StatusId);

            // Многие к одному между Setting и Group
            modelBuilder.Entity<Setting>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Settings)
                .HasForeignKey(s => s.GroupId);

            // Многие к одному между Group и Department
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Department)
                .WithMany(d => d.Groups)
                .HasForeignKey(g => g.DepartmentId);

            // Многие к одному между DefaultSchedule и Group
            modelBuilder.Entity<DefaultSchedule>()
                .HasOne(d => d.Group)
                .WithMany(g => g.DefaultSchedules)
                .HasForeignKey(d => d.GroupId);

            // Многие к одному между ReplasementLesson и Group
            modelBuilder.Entity<ReplasementLesson>()
                .HasOne(d => d.Group)
                .WithMany(g => g.ReplasementLessons)
                .HasForeignKey(d => d.GroupId);

            // Многие к одному между CorrectSchedule и Group
            modelBuilder.Entity<CorrectSchedule>()
                .HasOne(d => d.Group)
                .WithMany(g => g.CorrectSchedules)
                .HasForeignKey(d => d.GroupId);

            // Многие к одному между DefaultSchedule и WeekOfSchedule
            modelBuilder.Entity<DefaultSchedule>()
                .HasOne(d => d.WeekOfSchedule)
                .WithMany(w => w.DefaultSchedules)
                .HasForeignKey(d => d.WeekOfScheduleId);

            // Многие к одному между ReplasementLesson и WeekOfSchedule
            modelBuilder.Entity<ReplasementLesson>()
                .HasOne(d => d.WeekOfSchedule)
                .WithMany(w => w.ReplasementLessons)
                .HasForeignKey(d => d.WeekOfScheduleId);

            // Многие к одному между CorrectSchedule и WeekOfSchedule
            modelBuilder.Entity<CorrectSchedule>()
                .HasOne(d => d.WeekOfSchedule)
                .WithMany(w => w.CorrectSchedules)
                .HasForeignKey(d => d.WeekOfScheduleId);
        }
    }

    public class DefaultSchedule
    {
        [Key]
        public int IdDefSchedule { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int WeekOfScheduleId { get; set; }
        public WeekOfSchedule WeekOfSchedule { get; set; }

        public int Weekday { get; set; }
        public string SerializeDataLessons { get; set; }
    }

    public class ReplasementLesson
    {
        [Key]
        public int IdRepLesson { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int WeekOfScheduleId { get; set; }
        public WeekOfSchedule WeekOfSchedule { get; set; }

        public int Weekday { get; set; }
        public string SerializeDataLessons { get; set; }
    }

    public class CorrectSchedule
    {
        [Key]
        public int IdCorSchedule { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int WeekOfScheduleId { get; set; }
        public WeekOfSchedule WeekOfSchedule { get; set; }

        public int Weekday { get; set; }
        public string SerializeDataLessons { get; set; }
    }

    public class WeekOfSchedule
    {
        [Key]
        public int IdWeekOfSchedule { get; set; }
        public string WeekOfScheduleName { get; set; }

        public List<DefaultSchedule> DefaultSchedules { get; set; }
        public List<ReplasementLesson> ReplasementLessons { get; set; }
        public List<CorrectSchedule> CorrectSchedules { get; set; }
    }

    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string UserName { get; set; }

        public long UserTGId { get; set; }

        public int StatusId { get; set; } = 1;
        public Status Status { get; set; }

        public Setting Setting { get; set; }

        public bool IsBanned { get; set; }

    }

    public class Group
    {
        [Key]
        public int IdGroup { get; set; }
        public string GroupName { get; set; }

        public List<Setting> Settings { get; set; }
        public List<DefaultSchedule> DefaultSchedules { get; set; }
        public List<ReplasementLesson> ReplasementLessons { get; set; }
        public List<CorrectSchedule> CorrectSchedules { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
    }

    public class Department
    {
        [Key]
        public int IdDepartment { get; set; }
        public string DepartmentName { get; set; }

        public List<Group> Groups { get; set; }
    }

    public class Setting
    {
        [Key]
        public int IdSetting { get; set; }
        public bool isMailing { get; set; }
        public bool TimeOfLessons { get; set; }

        public int UserId { set; get; }
        public User User { set; get; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }

    public class Status
    {
        [Key]
        public int IdStatus { get; set; }
        public string StatusName { get; set; }

        public List<User> Users { get; set; }
    }
}
