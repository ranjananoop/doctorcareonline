using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;


namespace DOCVIDEO.HealthHistoryServiceBoundedContext
{
   public class ImmunizationServiceContext :BaseContext<ImmunizationServiceContext>
    {
       public DbSet<IMMUNIZATION> Immunizations { get; set; }
     
       protected override void OnModelCreating(DbModelBuilder modelBuilder)
       {

       }
    
    }
}
