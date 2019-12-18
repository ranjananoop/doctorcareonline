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
    public class PrefferdDoctorServiceRepository :IPreferredDoctorRepository
    {
        private readonly PrefferedDoctorServiceContext _context;

        public PrefferdDoctorServiceRepository(PrefferedDoctorUnitOfWork uow)
        {
            _context = uow.Context;
        }
        

        public IQueryable<PREFERREDDOCTOR> All
        {
            get { return _context.PrefferedDoctors; }
        }

        public List<PREFERREDDOCTOR> Allworks
        {
            get { return _context.PrefferedDoctors.ToList(); }
        }


        public IQueryable<PREFERREDDOCTOR> AllIncluding(params Expression<Func<PREFERREDDOCTOR, object>>[] includeProperties)
        {
            IQueryable<PREFERREDDOCTOR> query = _context.PrefferedDoctors;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PREFERREDDOCTOR Find(int id)
        {
            return _context.PrefferedDoctors.Find(id);
        }


        public void InsertOrUpdateGraph(PREFERREDDOCTOR specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.PrefferedDoctors.Add(specGraph);
            }
            else
            {
                _context.PrefferedDoctors.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(PREFERREDDOCTOR spec)
        {
            if (spec.PREFERREDDOCTORID  == default(int)) // New entity
            {
                _context.Entry(spec).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(spec).State = EntityState.Modified;

            }
        }

        public void Delete(int id)
        {
            try
            {
                var doctorspecialities = _context.PrefferedDoctors.Find(id);

                _context.PrefferedDoctors.Remove(doctorspecialities);
                
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public PREFERREDDOCTOR Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}