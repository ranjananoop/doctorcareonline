using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.PatientInformationServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;

namespace PatientInformationService.Repositories.Disconnected
{
    public class PatientInformationRepository : IPatientInformationRepository
    {
         private readonly PatientInformationServiceContext _context;

         public PatientInformationRepository(UnitOfWork uow)
        {
            _context = uow.Context;
        }

         public IQueryable<USERSINFORMATION> AllUsersInformations
        {
            get { return _context.UsersInformations; }
        }

         public List<USERSINFORMATION> All
        {
            get { return _context.UsersInformations.ToList(); }
        }

         public IQueryable<USERSINFORMATION> AllIncluding(params Expression<Func<USERSINFORMATION, object>>[] includeProperties)
        {
            IQueryable<USERSINFORMATION> query = _context.UsersInformations;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

         public USERSINFORMATION Find(string id)
        {
            return _context.UsersInformations.Find(id);
        }

         public void InsertOrUpdateGraph(USERSINFORMATION  usersinformationGraph)
        {
            if (usersinformationGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.UsersInformations.Add(usersinformationGraph);
            }
            else
            {
                _context.UsersInformations.Add(usersinformationGraph);
                _context.ApplyStateChanges();
            }
        }

         public void InsertOrUpdate(USERSINFORMATION usersinformation)
         {
             if (usersinformation.UserName == default(string)) // New entity
             {
                 _context.Entry(usersinformation).State = EntityState.Added;
             }
             else        // Existing entity
             {
                 _context.Entry(usersinformation).State = EntityState.Modified;

             }
         }

         //public void Insert(USERSINFORMATION usersinformation)
         //{
         //    _context.Entry(usersinformation).State = EntityState.Added;
         //}

         //public void Update(USERSINFORMATION usersinformation)
         //{
         //    _context.Entry(usersinformation).State = EntityState.Modified;
         //}

        public void Delete(string id)
        {
            var usersinformation  = _context.UsersInformations.Find(id);
           // var addresses = _context.ConsultantAddresses.Where(a => a.ConsultantId == consultant.ConsultantId);
           // foreach (var address in addresses)
            //{
            //    _context.ConsultantAddresses.Remove(address);
            //}
            _context.UsersInformations.Remove(usersinformation);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        public USERSINFORMATION Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
