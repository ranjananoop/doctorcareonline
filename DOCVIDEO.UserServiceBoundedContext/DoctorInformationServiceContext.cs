using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class DoctorInformationServiceContext : BaseContext<DoctorInformationServiceContext>
    {
        public DbSet<DOCTORSINFORMATION> DoctorsInformations { get; set; }
        public DbSet<DOCTORWORKINGINSTITUION> DoctorsWorkInstitutions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

