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
    public class DocSubscriptionRepository :IDocSubscriptionRepository
    {
        private readonly DocSubscriptionServiceContext _context;

        public DocSubscriptionRepository(DocSubscriptionUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCSUBSCRIPTION> All
        {
            get { return _context.DOCSUBSCRIPTIONS; }
        }

        public List<DOCSUBSCRIPTION> Allworks
        {
            get { return _context.DOCSUBSCRIPTIONS.ToList(); }
        }


        public IQueryable<DOCSUBSCRIPTION> AllIncluding(params Expression<Func<DOCSUBSCRIPTION, object>>[] includeProperties)
        {
            IQueryable<DOCSUBSCRIPTION> query = _context.DOCSUBSCRIPTIONS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCSUBSCRIPTION Find(int id)
        {
            return _context.DOCSUBSCRIPTIONS.Find(id);
        }


        public void InsertOrUpdateGraph(DOCSUBSCRIPTION specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DOCSUBSCRIPTIONS.Add(specGraph);
            }
            else
            {
                _context.DOCSUBSCRIPTIONS.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCSUBSCRIPTION spec)
        {
            if (spec.DOCSUBSCRIPTIONID  == default(int)) // New entity
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
                var subscriptions = _context.DOCSUBSCRIPTIONS.Where(a => a.DOCSUBSCRIPTIONID == id);
                if (subscriptions != null)
                {
                    foreach (var docsubscription in subscriptions)
                    {
                        _context.DOCSUBSCRIPTIONS.Remove(docsubscription);
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


        public DOCSUBSCRIPTION Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}