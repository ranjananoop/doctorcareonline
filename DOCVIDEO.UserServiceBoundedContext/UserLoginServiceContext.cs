﻿using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class UserLoginServiceContext : BaseContext<UserLoginServiceContext>
    {
        public DbSet<USERSINFORMATION > UserInformations { get; set; }
        public DbSet<USERLOGIN> UserLogins { get; set; }
            
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

}

