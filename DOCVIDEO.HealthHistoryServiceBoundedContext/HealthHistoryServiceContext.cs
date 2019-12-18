using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.HealthHistoryServiceBoundedContext
{
   public class HealthHistoryServiceContext : BaseContext<HealthHistoryServiceContext>
    {
       public DbSet<PATIENTREPORT> PatientReports { get; set; }
       public DbSet<APPOINTMENT> Appointments { get; set; }
     
       protected override void OnModelCreating(DbModelBuilder modelBuilder)
       {

       }
    }
}
