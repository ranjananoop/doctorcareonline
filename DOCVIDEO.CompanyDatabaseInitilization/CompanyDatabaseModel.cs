using System.Data.Entity;
using DOCVIDEO.Domain;

namespace CompanyDatabaseInitialization
{
    public class CompanyDatabase : DbContext
    {
        public CompanyDatabase()
            : base("name=PROFESSORSONLINEEntities")
        {
        }



        public DbSet<APPOINTMENTFOLLOWUP> APPOINTMENTFOLLOWUPS { get; set; }
        public DbSet<APPOINTMENTMESSAGE> APPOINTMENTMESSAGES { get; set; }
        public DbSet<APPOINTMENTRATING> APPOINTMENTRATINGS { get; set; }
        public DbSet<APPOINTMENT> APPOINTMENTS { get; set; }
        public DbSet<APPOINTMENTSLOT> APPOINTMENTSLOTS { get; set; }
        public DbSet<ATTACHMENTLINK> ATTACHMENTLINKS { get; set; }
        public DbSet<CANCELLEDAPPOINTMENT> CANCELLEDAPPOINTMENTS { get; set; }
        public DbSet<CHATHISTORY> CHATHISTORIES { get; set; }
        public DbSet<CHATSESSION> CHATSESSIONS { get; set; }
        public DbSet<PATIENTREPORT> PATIENTREPORTS { get; set; }
        public DbSet<DOCTORSINFORMATION> DOCTORSINFORMATIONS { get; set; }
        public DbSet<DOCTORSTATUS> DOCTORSTATUS { get; set; }
        public DbSet<DOCUMENTATTACHMENT> DOCUMENTATTACHMENTS { get; set; }
        public DbSet<LANGUAGE> LANGUAGES { get; set; }
        public DbSet<LISTMASTER> LISTMASTERS { get; set; }
        public DbSet<LISTVALUE> LISTVALUES { get; set; }
        public DbSet<LOCATION> LOCATIONS { get; set; }
        public DbSet<MESSAGE> MESSAGES { get; set; }
        public DbSet<PATIENTINFORMATION> PATIENTINFORMATIONS { get; set; }
        public DbSet<USERSLANGUAGE> USERSLANGUAGES { get; set; }
        public DbSet<DOCTORNOTES> DOCTORNOTES { get; set; }
        public DbSet<POLICY> POLICIES { get; set; }
        public DbSet<PREFERREDDOCTOR> PREFERREDDOCTORS { get; set; }
        public DbSet<DOCTORAVAILABILITY> DOCTORAVAILABILITIES { get; set; }        
        public DbSet<MEDICATION> MEDICATIONS { get; set; }
        public DbSet<IMMUNIZATION> IMMUNIZATIONS { get; set; }
        public DbSet<DOCTORPAYRATE> DOCTORPAYRATES { get; set; }
        public DbSet<DOCTORSLOT> DOCTORSLOTS { get; set; }
        public DbSet<DOCTORSPECIALITY> DOCTORSPECIALITIES { get; set; }
        public DbSet<DOCTORWORKINGINSTITUION> DOCTORWORKINGINSTITUIONS { get; set; }
        public DbSet<RECENTSEARCH> RECENTSEARCHES { get; set; }
        public DbSet<SECURITY> SECURITIES { get; set; }
        public DbSet<SUBSCRIPTION> SUBSCRIPTIONS { get; set; }
        public DbSet<SUBSCRIPTIONINSTALLMENT> SUBSCRIPTIONINSTALLMENTS { get; set; }
        public DbSet<USERSINFORMATION> USERSINFORMATIONS { get; set; }
        public DbSet<USERLOGIN> USERLOGINS { get; set; }
        public DbSet<USERPASSWORD> USERPASSWORDS { get; set; }
        public DbSet<WORDING> WORDINGS { get; set; }
        public DbSet<PROCEDURE> PROCEDURES { get; set; }
        public DbSet<ALLERGY> ALLERGIES { get; set; }
        public DbSet<CHATMESSAGE> CHATMESSAGES { get; set; }
        public DbSet<DOCPAYMENT> DOCPAYMENTS { get; set; }
        public DbSet<SUBSCRIPTIONPAYMENT> SUBSCRIPTIONPAYMENT { get; set; }
        public DbSet<DOCSUBSCRIPTION> DOCSUBSCRIPTIONS { get; set; }




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}

