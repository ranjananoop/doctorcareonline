using System.Collections.Generic;
using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class DoctorNotesServiceContext : BaseContext<DoctorNotesServiceContext>
    {
        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }
        public DbSet<CANCELLEDAPPOINTMENT> CancelledAppointments { get; set; }
        public DbSet<APPOINTMENTRATING> AppointmentRatings { get; set; }
        public DbSet<APPOINTMENT> Appointments { get; set; }
        public DbSet<DOCTORNOTES> DoctorNotes { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

