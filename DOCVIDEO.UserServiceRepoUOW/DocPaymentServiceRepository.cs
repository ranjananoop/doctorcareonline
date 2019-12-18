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
    public class DocPaymentServiceRepository :IDocPaymentRepository
    {
        private readonly DocPaymentServiceContext _context;

        public DocPaymentServiceRepository(DocPaymentUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCPAYMENT> All
        {
            get { return _context.DOCPAYMENTS; }
        }

        public List<DOCPAYMENT> Allworks
        {
            get { return _context.DOCPAYMENTS.ToList(); }
        }


        public IQueryable<DOCPAYMENT> AllIncluding(params Expression<Func<DOCPAYMENT, object>>[] includeProperties)
        {
            IQueryable<DOCPAYMENT> query = _context.DOCPAYMENTS;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCPAYMENT Find(int id)
        {
            return _context.DOCPAYMENTS.Find(id);
        }


        public void InsertOrUpdateGraph(DOCPAYMENT specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DOCPAYMENTS.Add(specGraph);
            }
            else
            {
                _context.DOCPAYMENTS.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCPAYMENT spec)
        {
            if (spec.DOCPAYMENTID  == default(int)) // New entity
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
                var DocPayments = _context.DOCPAYMENTS.Where(a => a.DOCPAYMENTID == id);
                if (DocPayments != null)
                {
                    foreach (var DocPayment in DocPayments)
                    {
                        _context.DOCPAYMENTS.Remove(DocPayment);
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


        public DOCPAYMENT Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}