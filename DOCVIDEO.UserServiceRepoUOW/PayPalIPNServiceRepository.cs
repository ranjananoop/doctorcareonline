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
    public class PayPalIPNServiceRepository :IPayPAlIPNRepository
    {
        private readonly PayPalIPNServiceContext _context;

        public PayPalIPNServiceRepository(PayPalIPNUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<PAYPALIPN> All
        {
            get { return _context.PAYPALIPNS; }
        }

        public List<PAYPALIPN> Allworks
        {
            get { return _context.PAYPALIPNS.ToList(); }
        }


        public IQueryable<PAYPALIPN> AllIncluding(params Expression<Func<PAYPALIPN, object>>[] includeProperties)
        {
            IQueryable<PAYPALIPN> query = _context.PAYPALIPNS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PAYPALIPN Find(int id)
        {
            return _context.PAYPALIPNS.Find(id);
        }


        public void InsertOrUpdateGraph(PAYPALIPN specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.PAYPALIPNS.Add(specGraph);
            }
            else
            {
                _context.PAYPALIPNS.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(PAYPALIPN spec)
        {
            if (spec.PAYPALIPNID  == default(int)) // New entity
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
                var paypalipns = _context.PAYPALIPNS.Where(a => a.PAYPALIPNID == id);
                if (paypalipns != null)
                {
                    foreach (var paypalipn in paypalipns)
                    {
                        _context.PAYPALIPNS.Remove(paypalipn);
                    }
                }
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


        public PAYPALIPN Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}