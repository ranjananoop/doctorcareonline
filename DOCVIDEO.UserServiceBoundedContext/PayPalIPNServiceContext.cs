using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class PayPalIPNServiceContext : BaseContext<PayPalIPNServiceContext>
    {
        public DbSet<PAYPALIPN> PAYPALIPNS{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

