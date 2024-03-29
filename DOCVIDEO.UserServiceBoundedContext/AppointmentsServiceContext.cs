﻿using System.Collections.Generic;
using System.Data.Entity;
//using System.Web.Security;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;

namespace DOCVIDEO.UserServiceBoundedContext
{
    public class AppointmentsServiceContext : BaseContext<AppointmentsServiceContext>
    {

       
        public DbSet<USERSINFORMATION> UserInformations { get; set; }       
        public DbSet<APPOINTMENT > APPOINTMENTS { get; set; }
        public DbSet<APPOINTMENTSLOT> APPOINTMENTSLOTS { get; set; }
        public DbSet<DOCTORSLOT> DOCTORSLOTS { get; set; }
        public DbSet<APPOINTMENTRATING> AppointmentRatings { get; set; }
      


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }

}
