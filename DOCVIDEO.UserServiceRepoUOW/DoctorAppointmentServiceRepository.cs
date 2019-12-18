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
    public class DoctorAppointmentServiceRepository : IDoctorAppointmentStatus
    {
        private readonly DoctorAppointmentStatusServiceContext _context;

        public DoctorAppointmentServiceRepository(AppointmentStatusUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<CANCELLEDAPPOINTMENT> All
        {
            get { return _context.CancelledAppointments; }
        }

        public List<CANCELLEDAPPOINTMENT> Allworks
        {
            get { return _context.CancelledAppointments.ToList(); }
        }


        public IQueryable<CANCELLEDAPPOINTMENT> AllIncluding(params Expression<Func<CANCELLEDAPPOINTMENT, object>>[] includeProperties)
        {
            IQueryable<CANCELLEDAPPOINTMENT> query = _context.CancelledAppointments;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public CANCELLEDAPPOINTMENT Find(string id)
        {
            return _context.CancelledAppointments.Find(id);
        }


        public void InsertOrUpdateGraph(CANCELLEDAPPOINTMENT workGraph)
        {
            if (workGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.CancelledAppointments.Add(workGraph);
            }
            else
            {
                _context.CancelledAppointments.Add(workGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(CANCELLEDAPPOINTMENT cap)
        {
            if (cap.CANCELLEDAPPOINTMENTID == default(int)) // New entity
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


        public CANCELLEDAPPOINTMENT Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}