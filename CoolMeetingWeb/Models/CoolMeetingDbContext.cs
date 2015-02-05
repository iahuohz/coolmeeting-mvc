using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace CoolMeetingWeb.Models
{
    public class CoolMeetingDbContext : DbContext
    {
        public CoolMeetingDbContext() : base("DefaultConnection")
        {

        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Meeting>().HasMany(m => m.Participants)
                .WithMany(e => e.MeetingsParticipated)
                .Map(m =>
                {
                    m.ToTable("MeetingParticipant");
                    m.MapLeftKey("MeetingID");
                    m.MapRightKey("EmployeeID");
                }
                );
        }
    }
}