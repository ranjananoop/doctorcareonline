using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.HealthHistoryServiceBoundedContext
{
   public class MedicationServiceContext: BaseContext<MedicationServiceContext>
    {
        public DbSet<MEDICATION> Medications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
