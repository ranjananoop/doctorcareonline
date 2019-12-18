using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;
using DOCVIDEO.HealthHistoryServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW.Repositories.Disconnected
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly ProcedureServiceContext _context;
        public ProcedureRepository(ProcedureUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<PROCEDURE> All
        {
            get { return _context.Procedures; }
        }

        public List<PROCEDURE> AllProcedures
        {
            get { return _context.Procedures.ToList(); }
        }

        public IQueryable<PROCEDURE> AllIncluding(params Expression<Func<PROCEDURE, object>>[] includeProperties)
        {
            IQueryable<PROCEDURE> query = _context.Procedures;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PROCEDURE Find(int id)
        {
            return _context.Procedures.Find(id);
        }

        public void InsertOrUpdateGraph(PROCEDURE procedurereportGraph)
        {
            if (procedurereportGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Procedures.Add(procedurereportGraph);
            }
            else
            {
                _context.Procedures.Add(procedurereportGraph);
                _context.ApplyStateChanges();
            }
        }


        public void InsertOrUpdate(PROCEDURE procedure)
        {
            if (procedure.PROCEDUREID == default(int)) // New entity
            {
                _context.Entry(procedure).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(procedure).State = EntityState.Modified;

            }

        }
        public void Delete(int id)
        {
            var procedure = _context.Procedures.Find(id);

            _context.Procedures.Remove(procedure);
        }



        public void Dispose()
        {
            _context.Dispose();
        }

        public PROCEDURE Find(string message)
        {
            throw new NotImplementedException();


        }


        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}