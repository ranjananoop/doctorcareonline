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
    public class AppointmentRatingServiceRepository : IAppointmentRatingRepository
    {
        private readonly AppointmentsRatingServiceContext _context;

        public AppointmentRatingServiceRepository(AppointmentsRatingUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<APPOINTMENTRATING> All           
        {
            get { return _context.AppointmentRatings; }
        }

        public List<APPOINTMENTRATING> AllUsers
        {
            get { return _context.AppointmentRatings.ToList(); }
        }


        public IQueryable<APPOINTMENTRATING> AllIncluding(params Expression<Func<APPOINTMENTRATING, object>>[] includeProperties)
        {
            IQueryable<APPOINTMENTRATING> query = _context.AppointmentRatings;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public APPOINTMENTRATING Find(string id)
        {
            throw new NotImplementedException();
            
        }


        public void InsertOrUpdateGraph(APPOINTMENTRATING userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.AppointmentRatings.Add(userGraph);
            }
            else
            {
                _context.AppointmentRatings.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(APPOINTMENTRATING rate)
        {
            if (rate.APPOINTMENTRATINGID== default(int)) // New entity
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
            var rate = _context.AppointmentRatings.Find(id);           

            _context.AppointmentRatings.Remove(rate);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public APPOINTMENTRATING Find(int id)
        {
            return _context.AppointmentRatings.Find(id); 
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}