using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.PatientInformationServiceBoundedContext
{
    public class PatientRatingInformationServiceContext : BaseContext<PatientRatingInformationServiceContext>
    {
        public DbSet<USERSINFORMATION> UsersInformations { get; set; }
        public DbSet<DOCTORSTATUS> DocRatings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}