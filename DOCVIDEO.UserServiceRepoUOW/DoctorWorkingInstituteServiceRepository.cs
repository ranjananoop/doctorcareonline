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
    public class DoctorWorkingInstituteServiceRepository : IDoctorWorkRepository
    {
        private readonly DoctorWorkInstitutionServiceContext _context;

        public DoctorWorkingInstituteServiceRepository(WorkInstitutionUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCTORWORKINGINSTITUION> All
        {
            get { return _context.DoctorsWorkInstitutions; }
        }

        public List<DOCTORWORKINGINSTITUION> Allworks
        {
            get { return _context.DoctorsWorkInstitutions.ToList(); }
        }


        public IQueryable<DOCTORWORKINGINSTITUION> AllIncluding(params Expression<Func<DOCTORWORKINGINSTITUION, object>>[] includeProperties)
        {
            IQueryable<DOCTORWORKINGINSTITUION> query = _context.DoctorsWorkInstitutions;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORWORKINGINSTITUION Find(string id)
        {
            return _context.DoctorsWorkInstitutions.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORWORKINGINSTITUION workGraph)
        {
            if (workGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DoctorsWorkInstitutions.Add(workGraph);
            }
            else
            {
                _context.DoctorsWorkInstitutions.Add(workGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORWORKINGINSTITUION work)
        {
            if (work.WORKINGINSTITUIONID == default(int)) // New entity
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


        public DOCTORWORKINGINSTITUION Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}