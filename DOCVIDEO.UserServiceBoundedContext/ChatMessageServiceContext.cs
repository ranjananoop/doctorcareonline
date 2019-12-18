using System.Collections.Generic;
using System.Data.Entity;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;
namespace DOCVIDEO.UserServiceBoundedContext
{
    public class ChatMessageServiceContext : BaseContext<ChatMessageServiceContext>
    {

        public DbSet<CHATMESSAGE> ChatMessages { get; set; }
        public DbSet<USERSINFORMATION> UserInformations { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
