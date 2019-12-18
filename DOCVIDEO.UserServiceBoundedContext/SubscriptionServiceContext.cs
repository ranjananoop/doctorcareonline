using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class SubscriptionServiceContext : BaseContext<SubscriptionServiceContext>
    {
        public DbSet<USERSINFORMATION> UserInformations { get; set; }
        public DbSet<SUBSCRIPTION> SUBSCRIPTIONS { get; set; }
        public DbSet<SUBSCRIPTIONINSTALLMENT> SUBSCRIPTIONINSTALLMENTS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

