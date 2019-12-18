using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.PatientInformationServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;

namespace PatientInformationService.Repositories.Disconnected
{
    public class PatientRatingInformationRepository : IPatientRatingInformationRepository
    {
        private readonly PatientRatingInformationServiceContext _context;

         public PatientRatingInformationRepository(DoctorRatingUnitOfWork uow)
        {
            _context = uow.Context;
        }

         public IQueryable<DOCTORSTATUS> AllUsersInformations
        {
            get { return _context.DocRatings; }
        }

         public List<DOCTORSTATUS> All
        {
            get { return _context.DocRatings.ToList(); }
        }

         public IQueryable<DOCTORSTATUS> AllIncluding(params Expression<Func<DOCTORSTATUS, object>>[] includeProperties)
        {
            IQueryable<DOCTORSTATUS> query = _context.DocRatings;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

         public DOCTORSTATUS Find(int id)
        {
            return _context.DocRatings.Find(id);
        }

         public void InsertOrUpdateGraph(DOCTORSTATUS  DOCRATINGGraph)
        {
            if (DOCRATINGGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DocRatings.Add(DOCRATINGGraph);
            }
            else
            {
                _context.DocRatings.Add(DOCRATINGGraph);
                _context.ApplyStateChanges();
            }
        }

         public void InsertOrUpdate(DOCTORSTATUS DOCRATING)
         {
             if (DOCRATING.DOCTORSTATUSID == default(int)) // New entity
             {
                 _context.Entry(DOCRATING).State = EntityState.Added;
             }
             else        // Existing entity
             {
                 _context.Entry(DOCRATING).State = EntityState.Modified;

             }
         }

         //public void Insert(DOCRATING DOCRATING)
         //{
         //    _context.Entry(DOCRATING).State = EntityState.Added;
         //}

         //public void Update(DOCRATING DOCRATING)
         //{
         //    _context.Entry(DOCRATING).State = EntityState.Modified;
         //}

        public void Delete(int id)
        {
            var DOCRATING  = _context.DocRatings.Find(id);
           
            _context.DocRatings.Remove(DOCRATING);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        public DOCTORSTATUS Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
