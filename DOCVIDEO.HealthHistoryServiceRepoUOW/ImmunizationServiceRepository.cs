using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;
using DOCVIDEO.HealthHistoryServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;



namespace DOCVIDEO.HealthHistoryServiceRepoUOW.Repositories.Disconnected
{
    public class ImmunizationRepository : IImmunizationRepository
    {
        private readonly ImmunizationServiceContext _context;

        public ImmunizationRepository(ImmunizationUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<IMMUNIZATION> All
        {
            get { return _context.Immunizations; }
        }

        public List<IMMUNIZATION> AllImmunizations
        {
            get { return _context.Immunizations.ToList(); }
        }

        public IQueryable<IMMUNIZATION> AllIncluding(params Expression<Func<IMMUNIZATION, object>>[] includeProperties)
        {
            IQueryable<IMMUNIZATION> query = _context.Immunizations;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IMMUNIZATION Find(int id)
        {
            return _context.Immunizations.Find(id);
        }

        public void InsertOrUpdateGraph(IMMUNIZATION immunizationGraph)
        {
            if (immunizationGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Immunizations.Add(immunizationGraph);
            }
            else
            {
                _context.Immunizations.Add(immunizationGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(IMMUNIZATION immunization)
        {
            if (immunization.IMMUNIZATIONID == default(int)) // New entity
            {
                _context.Entry(immunization).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(immunization).State = EntityState.Modified;

            }

        }

        public void Delete(int id)
        {
            var immunization = _context.Immunizations.Find(id);
            _context.Immunizations.Remove(immunization);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IMMUNIZATION Find(string message)
        {
            throw new NotImplementedException();


        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}