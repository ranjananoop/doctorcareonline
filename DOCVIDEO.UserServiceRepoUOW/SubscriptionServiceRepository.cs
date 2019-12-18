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
    public class SubscriptionServiceRepository :ISubscriptionRepository
    {
        private readonly SubscriptionServiceContext _context;

        public SubscriptionServiceRepository(SubscriptionUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<SUBSCRIPTION> All
        {
            get { return _context.SUBSCRIPTIONS; }
        }

        public List<SUBSCRIPTION> Allworks
        {
            get { return _context.SUBSCRIPTIONS.ToList(); }
        }


        public IQueryable<SUBSCRIPTION> AllIncluding(params Expression<Func<SUBSCRIPTION, object>>[] includeProperties)
        {
            IQueryable<SUBSCRIPTION> query = _context.SUBSCRIPTIONS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SUBSCRIPTION Find(string id)
        {
            return _context.SUBSCRIPTIONS.Find(id);
        }


        public void InsertOrUpdateGraph(SUBSCRIPTION specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.SUBSCRIPTIONS.Add(specGraph);
            }
            else
            {
                _context.SUBSCRIPTIONS.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(SUBSCRIPTION spec)
        {
            if (spec.SUBSCRIPTIONID  == default(int)) // New entity
            {
                _context.Entry(spec).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(spec).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            try
            {
                var subscriptions = _context.SUBSCRIPTIONS.Where(a => a.UserName == id);
                if (subscriptions != null)
                {
                    foreach (var subscription in subscriptions)
                    {
                        _context.SUBSCRIPTIONS.Remove(subscription);
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


        public SUBSCRIPTION Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}