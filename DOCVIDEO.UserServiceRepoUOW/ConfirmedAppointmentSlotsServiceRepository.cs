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
    public class ConfirmedAppointmentSlotsServiceRepository : IConfirmedAppointmentSlotsRepository
    {
        private readonly ConfirmedAppointmentsSlotsContext _context;

        public ConfirmedAppointmentSlotsServiceRepository(ConfirmedAppointmentSlotsUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<APPOINTMENTSLOT> All           
        {
            get { return _context.APPOINTMENTSLOTS; }
        }

        public List<APPOINTMENTSLOT> AllUsers
        {
            get { return _context.APPOINTMENTSLOTS.ToList(); }
        }


        public IQueryable<APPOINTMENTSLOT> AllIncluding(params Expression<Func<APPOINTMENTSLOT, object>>[] includeProperties)
        {
            IQueryable<APPOINTMENTSLOT> query = _context.APPOINTMENTSLOTS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public APPOINTMENTSLOT Find(string id)
        {
            return _context.APPOINTMENTSLOTS.Find(id);
        }


        public void InsertOrUpdateGraph(APPOINTMENTSLOT userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.APPOINTMENTSLOTS.Add(userGraph);
            }
            else
            {
                _context.APPOINTMENTSLOTS.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(APPOINTMENTSLOT rate)
        {
            if (rate.APPOINTMENTSLOTID== default(int)) // New entity
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
            var slot = _context.APPOINTMENTSLOTS.SingleOrDefault(f => f.APPOINTMENTSLOTID == id);

            _context.APPOINTMENTSLOTS.Remove(slot);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public APPOINTMENTSLOT Find(int id)
        {
            throw new NotImplementedException();
        }

        
    }
}