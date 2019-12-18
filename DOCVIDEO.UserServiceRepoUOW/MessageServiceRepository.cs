using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.Domain;
using DOCVIDEO.UserServiceRepoUOW;
using DOCVIDEO.UserServiceRepoUOW;

namespace DOCVIDEO.UserServiceRepoUOW.Disconnected
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageServiceContext _context;

         public MessageRepository(MessageUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<MESSAGE> All
        {
            get { return _context.Messages; }
        }

        public List<MESSAGE> AllMessages
        {
            get { return _context.Messages.ToList(); }
        }
        
        public IQueryable<MESSAGE> AllIncluding(params Expression<Func<MESSAGE, object>>[] includeProperties)
        {
            IQueryable<MESSAGE> query = _context.Messages;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public MESSAGE Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdateGraph(MESSAGE messageGraph)
        {
            if (messageGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Messages.Add(messageGraph);
            }
            else
            {
                _context.Messages.Add(messageGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(MESSAGE message)
        {
            if (message.MESSAGEID == default(int)) // New entity
            {
                _context.Entry(message).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(message).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            var message = _context.Messages.Find(id);
            _context.Messages.Remove(message);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public MESSAGE Find(string message)
        {
            return _context.Messages.Find(message);
            
        }


        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
