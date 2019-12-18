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
    public class SubscriptionPaymentRepository :ISubscriptionPaymentRepository
    {
        private readonly SubscriptionPaymentServiceContext _context;

        public SubscriptionPaymentRepository(SubscriptionPaymentUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<SUBSCRIPTIONPAYMENT> All
        {
            get { return _context.SUBSCRIPTIONPAYMENTS; }
        }

        public List<SUBSCRIPTIONPAYMENT> Allworks
        {
            get { return _context.SUBSCRIPTIONPAYMENTS.ToList(); }
        }


        public IQueryable<SUBSCRIPTIONPAYMENT> AllIncluding(params Expression<Func<SUBSCRIPTIONPAYMENT, object>>[] includeProperties)
        {
            IQueryable<SUBSCRIPTIONPAYMENT> query = _context.SUBSCRIPTIONPAYMENTS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SUBSCRIPTIONPAYMENT Find(int id)
        {
            return _context.SUBSCRIPTIONPAYMENTS.Find(id);
        }


        public void InsertOrUpdateGraph(SUBSCRIPTIONPAYMENT specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.SUBSCRIPTIONPAYMENTS.Add(specGraph);
            }
            else
            {
                _context.SUBSCRIPTIONPAYMENTS.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(SUBSCRIPTIONPAYMENT spec)
        {
            if (spec.SUBSCRIPTIONPAYMENTID  == default(int)) // New entity
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
                var subscriptions = _context.SUBSCRIPTIONPAYMENTS.Where(a => a.SUBSCRIPTIONPAYMENTID == id);
                if (subscriptions != null)
                {
                    foreach (var subscription in subscriptions)
                    {
                        _context.SUBSCRIPTIONPAYMENTS.Remove(subscription);
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


        public SUBSCRIPTIONPAYMENT Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}