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
    public class DoctorNotesServiceRepository : IDoctorNotes
    {
        private readonly DoctorNotesServiceContext _context;

        public DoctorNotesServiceRepository(AppointmentDoctorNotesUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCTORNOTES> All
        {
            get { return _context.DoctorNotes; }
        }

        public List<DOCTORNOTES> Allworks
        {
            get { return _context.DoctorNotes.ToList(); }
        }


        public IQueryable<DOCTORNOTES> AllIncluding(params Expression<Func<DOCTORNOTES, object>>[] includeProperties)
        {
            IQueryable<DOCTORNOTES> query = _context.DoctorNotes;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORNOTES Find(string id)
        {
            return _context.DoctorNotes.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORNOTES workGraph)
        {
            if (workGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DoctorNotes.Add(workGraph);
            }
            else
            {
                _context.DoctorNotes.Add(workGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORNOTES cap)
        {
            if (cap.DOCTORNOTESID == default(int)) // New entity
            {
                _context.Entry(cap).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(cap).State = EntityState.Modified;

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


        public DOCTORNOTES Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}