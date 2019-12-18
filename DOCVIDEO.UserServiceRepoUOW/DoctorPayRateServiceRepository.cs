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
    public class DoctorPayRateServiceRepository :IDoctorPayRateRepository
    {
        private readonly DoctorPayRateServiceContext _context;

        public DoctorPayRateServiceRepository(PayRateUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCTORPAYRATE> All           
        { 
            get { return _context.DoctorPayRates; }
        }

        public List<DOCTORPAYRATE> AllUsers
        {
            get { return _context.DoctorPayRates.ToList(); }
        }


        public IQueryable<DOCTORPAYRATE> AllIncluding(params Expression<Func<DOCTORPAYRATE, object>>[] includeProperties)
        {
            IQueryable<DOCTORPAYRATE> query = _context.DoctorPayRates;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORPAYRATE Find(string id)
        {
            return _context.DoctorPayRates.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORPAYRATE userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DoctorPayRates.Add(userGraph);
            }
            else
            {
                _context.DoctorPayRates.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORPAYRATE rate)
        {
            if (rate.DOCTORPAYRATEID== default(int)) // New entity
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
            var rate = _context.DoctorPayRates.Find(id);           

            _context.DoctorPayRates.Remove(rate);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public DOCTORPAYRATE Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}