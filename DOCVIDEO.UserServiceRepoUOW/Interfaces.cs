using System;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.Domain;
using System.Web;
using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Generic;


namespace DOCVIDEO.UserServiceRepoUOW
{
    public interface IUserRepository : IEntityRepository<USERSINFORMATION>
    {
       
    }
    public interface IUserLoginRepository : IEntityRepository<USERLOGIN>
    {

    }

    public interface IDocSubscriptionRepository : IEntityRepository<DOCSUBSCRIPTION>
    {

    }

    public interface IPayPAlIPNRepository : IEntityRepository<PAYPALIPN>
    {

    }

    public interface ISubscriptionPaymentRepository : IEntityRepository<SUBSCRIPTIONPAYMENT>
    {

    }

    public interface ISubscriptionRepository : IEntityRepository<SUBSCRIPTION>
    {

    }

    public interface IDocPaymentRepository : IEntityRepository<DOCPAYMENT>
    {

    }

    public interface IDoctorNotes : IEntityRepository<DOCTORNOTES>
    {

    }
    public interface IDoctorInformation : IEntityRepository<DOCTORSINFORMATION>
    {

    }   

    public interface IChatMesageRepository : IEntityRepository<CHATMESSAGE>
    {

    }
    public interface IAppointmentRepository : IEntityRepository<APPOINTMENT>
    {

    }
    public interface IAppointmentRatingRepository : IEntityRepository<APPOINTMENTRATING>
    {

    }
    public interface IDoctorAppointmentStatus : IEntityRepository<CANCELLEDAPPOINTMENT>
    {

    }

    public interface IAppointmentSlotsRepository : IEntityRepository<DOCTORSLOT>
    {

    }
    public interface IConfirmedAppointmentSlotsRepository : IEntityRepository<APPOINTMENTSLOT>
    {

    }
    
    public interface IDoctorInFormationRepository : IEntityRepository<DOCTORSINFORMATION>
    {

    }
    public interface IDoctorWorkRepository : IEntityRepository<DOCTORWORKINGINSTITUION>
    {

    }
    public interface IUserPasswordRepository : IEntityRepository<USERPASSWORD>
    {

    }

    public interface IUserLanguageRepository : IEntityRepository<USERSLANGUAGE>
    {

    }
    public interface IDoctorStatusRepository : IEntityRepository<DOCTORSTATUS>
    {

    }

    public interface IDoctorSpecialityRepository : IEntityRepository<DOCTORSPECIALITY>
    {

    }

    public interface IDoctorPayRateRepository : IEntityRepository<DOCTORPAYRATE>
    {

    }
    public interface IMessageRepository : IEntityRepository<MESSAGE>
    {

    }

    public interface IPreferredDoctorRepository : IEntityRepository<PREFERREDDOCTOR>
    {
       
    }
   
    public interface IUserMembershipRepository : IEntityRepository<USERSINFORMATION>
    {
        void AddRole(string id, string roleName);
        void RemoveRole(string id, string roleName);
    }

    public interface IRoleRepository : IEntityRepository<Role>
    {
       
    }

    public interface IEntityRepository<T> : IDisposable
    {
        IQueryable<T> All { get; }
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        T Find(int id);
       
        void InsertOrUpdate(T entity);
        void Delete(int id);
       
        T Find(string userName);
       // static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector);
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);

    }

    
   

}
