using System.Collections.Generic;
using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class DoctorAppointmentStatusServiceContext : BaseContext<DoctorAppointmentStatusServiceContext>
    {
        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }
        public DbSet<CANCELLEDAPPOINTMENT> CancelledAppointments { get; set; }
        public DbSet<APPOINTMENTRATING> AppointmentRatings { get; set; }
        public DbSet<APPOINTMENT> Appointments { get; set; }
        public DbSet<DOCTORNOTES> DoctorNotes { get; set; }
        public DbSet<CHATMESSAGE> Chatmessages { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

