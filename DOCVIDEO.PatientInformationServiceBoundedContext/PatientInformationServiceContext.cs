using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.PatientInformationServiceBoundedContext
{
    public class PatientInformationServiceContext : BaseContext<PatientInformationServiceContext>
    {
        public DbSet<USERSINFORMATION> UsersInformations { get; set; }
        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}