using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class DoctorWorkInstitutionServiceContext : BaseContext<DoctorWorkInstitutionServiceContext>
    {
        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }
        public DbSet<DOCTORWORKINGINSTITUION> DoctorsWorkInstitutions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

