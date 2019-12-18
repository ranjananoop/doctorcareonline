using System;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.Domain;

namespace DOCVIDEO.PatientInformationServiceRepoUOW
{
    public interface IPatientInformationRepository : IEntityRepository<USERSINFORMATION>
    {
    }
    public interface IPatientRatingInformationRepository : IEntityRepository<DOCTORSTATUS>
    {
    }
    public interface IEntityRepository<T> : IDisposable
    {
        IQueryable<T> AllUsersInformations { get; }
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        T Find(string id);
        void InsertOrUpdate(T entity);      
        void Delete(string id);

    }
}
