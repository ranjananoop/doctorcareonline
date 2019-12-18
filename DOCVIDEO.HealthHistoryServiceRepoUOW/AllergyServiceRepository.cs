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
    public class AllergyRepository : IAllergyRepository
    {
        private readonly AllergiesServiceContext _context;

        public AllergyRepository(AllergyUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<ALLERGY> All
        {
            get { return _context.Allergies; }
        }

        public List<ALLERGY> AllAllergies
        {
            get { return _context.Allergies.ToList(); }
        }

        public IQueryable<ALLERGY> AllIncluding(params Expression<Func<ALLERGY, object>>[] includeProperties)
        {
            IQueryable<ALLERGY> query = _context.Allergies;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ALLERGY Find(int id)
        {
            return _context.Allergies.Find(id);
        }

        public void InsertOrUpdateGraph(ALLERGY allergyGraph)
        {
            if (allergyGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Allergies.Add(allergyGraph);
            }
            else
            {
                _context.Allergies.Add(allergyGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(ALLERGY allergy)
        {
            if (allergy.ALLERGYID == default(int)) // New entity
            {
                _context.Entry(allergy).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(allergy).State = EntityState.Modified;

            }

        }

        public void Delete(int id)
        {
            var allergy = _context.Allergies.Find(id);
            _context.Allergies.Remove(allergy);
        }


        public void Dispose()
        {
            _context.Dispose();
        }

        public ALLERGY Find(string message)
        {
            throw new NotImplementedException();


        }


        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}