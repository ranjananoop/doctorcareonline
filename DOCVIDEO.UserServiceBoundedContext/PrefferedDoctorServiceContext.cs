using System.Collections.Generic;
using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;
namespace DOCVIDEO.UserServiceBoundedContext
{
    public class PrefferedDoctorServiceContext : BaseContext<PrefferedDoctorServiceContext>
    {
        public DbSet<PREFERREDDOCTOR> PrefferedDoctors { get; set; }
        public DbSet<USERSINFORMATION> UserInformations { get; set; }

        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }        
        public DbSet<USERPASSWORD> Userpasswords { get; set; }
        public DbSet<USERSLANGUAGE> UserLanguages { get; set; }
        public DbSet<DOCTORSPECIALITY> Doctorspecialities { get; set; }
        public DbSet<DOCTORPAYRATE> DoctorPayRates { get; set; }      
        public DbSet<APPOINTMENTRATING> AppointmentRatings { get; set; }      
        public DbSet<DOCTORWORKINGINSTITUION> DocWorkInstitutions { get; set; }   

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
