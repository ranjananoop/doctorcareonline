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
    public class AppointmentServiceRepository :IAppointmentRepository
    {
        private readonly AppointmentsServiceContext _context;

        public AppointmentServiceRepository(AppointmentsUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<APPOINTMENT> All           
        {
            get { return _context.APPOINTMENTS; }
        }

        public List<APPOINTMENT> AllUsers
        {
            get { return _context.APPOINTMENTS.ToList(); }
        }


        public IQueryable<APPOINTMENT> AllIncluding(params Expression<Func<APPOINTMENT, object>>[] includeProperties)
        {
            IQueryable<APPOINTMENT> query = _context.APPOINTMENTS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public APPOINTMENT Find(string id)
        {
            return _context.APPOINTMENTS.Find(id);
        }


        public void InsertOrUpdateGraph(APPOINTMENT userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.APPOINTMENTS.Add(userGraph);
            }
            else
            {
                _context.APPOINTMENTS.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(APPOINTMENT rate)
        {
            if (rate.APPOINTMENTID== default(int)) // New entity
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
            var rate = _context.APPOINTMENTS.Find(id);           

            _context.APPOINTMENTS.Remove(rate);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public APPOINTMENT Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}