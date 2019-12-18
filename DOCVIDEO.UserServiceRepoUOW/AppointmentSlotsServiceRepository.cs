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
    public class AppointmentSlotsServiceRepository : IAppointmentSlotsRepository
    {
        private readonly AppointmentsSlotsContext _context;

        public AppointmentSlotsServiceRepository(AppointmentSlotsUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCTORSLOT> All           
        {
            get { return _context.DOCTORSLOTS; }
        }

        public List<DOCTORSLOT> AllUsers
        {
            get { return _context.DOCTORSLOTS.ToList(); }
        }


        public IQueryable<DOCTORSLOT> AllIncluding(params Expression<Func<DOCTORSLOT, object>>[] includeProperties)
        {
            IQueryable<DOCTORSLOT> query = _context.DOCTORSLOTS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORSLOT Find(string id)
        {
            return _context.DOCTORSLOTS.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORSLOT userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DOCTORSLOTS.Add(userGraph);
            }
            else
            {
                _context.DOCTORSLOTS.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORSLOT rate)
        {
            if (rate.DOCTORSLOTID== default(int)) // New entity
            {
                _context.Entry(rate).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(rate).State = EntityState.Modified;

            }
        }

        public void Delete(int id)
        {
            var slot = _context.DOCTORSLOTS.Find(id);

            _context.DOCTORSLOTS.Remove(slot);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public DOCTORSLOT Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id,string type)
        {
            var slots = _context.DOCTORSLOTS.Where(r => r.UserName == id && r.AVAILABILITYMODE==type );
            foreach (var slot in slots)
            {
                _context.DOCTORSLOTS.Remove(slot);
            }
        }
    }
}