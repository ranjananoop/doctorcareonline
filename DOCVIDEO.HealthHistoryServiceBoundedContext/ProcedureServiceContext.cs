using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.HealthHistoryServiceBoundedContext
{
    public class ProcedureServiceContext : BaseContext<ProcedureServiceContext>
    {
        public DbSet<PROCEDURE> Procedures { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
