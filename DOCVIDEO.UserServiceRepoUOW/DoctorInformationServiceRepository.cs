 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.UserServiceRepoUOW;

namespace UserService.Repositories.Disconnected
{
    public class DoctorInformationServiceRepository : IDoctorInformation
    {
        private readonly DoctorInformationServiceContext _context;

        public DoctorInformationServiceRepository(DoctorInformationUnitOfWork uow)
        {
            _context = uow.Context;
        }
        
        public IQueryable<DOCTORSINFORMATION> All
        {
            get { return _context.DoctorsInformations; }
        }

        public List<DOCTORSINFORMATION> Allworks
        {
            get { return _context.DoctorsInformations.ToList(); }
        }


        public IQueryable<DOCTORSINFORMATION> AllIncluding(params Expression<Func<DOCTORSINFORMATION, object>>[] includeProperties)
        {
            IQueryable<DOCTORSINFORMATION> query = _context.DoctorsInformations;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORSINFORMATION Find(string id)
        {
            return _context.DoctorsInformations.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORSINFORMATION workGraph)
        {
            if (workGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DoctorsInformations.Add(workGraph);
            }
            else
            {
                _context.DoctorsInformations.Add(workGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORSINFORMATION work)
        {
            if (work.DOCTORID == default(int)) // New entity
            {
                _context.Entry(work).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(work).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public DOCTORSINFORMATION Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}