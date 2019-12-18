using System.Collections.Generic;
using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;
namespace DOCVIDEO.UserServiceBoundedContext
{
    public class MessageServiceContext : BaseContext<MessageServiceContext>
    {
        public DbSet<MESSAGE> Messages { get; set; }
        public DbSet<USERSINFORMATION> Userinformations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
