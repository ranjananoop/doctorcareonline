using System;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.Domain;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public interface IHealthHistoryRepository : IEntityRepository<PATIENTREPORT>
    {
    }
    public interface IMedicationRepository : IEntityRepository<MEDICATION>
    {
    }
    public interface IImmunizationRepository : IEntityRepository<IMMUNIZATION>
    {
    }
    public interface IAllergyRepository : IEntityRepository<ALLERGY>
    {
    }
    public interface IProcedureRepository : IEntityRepository<PROCEDURE>
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
    }
}
