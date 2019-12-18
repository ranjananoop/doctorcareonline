using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.HealthHistoryServiceBoundedContext
{
    public class AllergiesServiceContext : BaseContext<AllergiesServiceContext>
    {
        public DbSet<ALLERGY> Allergies { get; set; }
        public DbSet<APPOINTMENT> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
