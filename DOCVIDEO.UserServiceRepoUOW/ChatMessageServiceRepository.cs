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
    public class ChatMessageServiceRepository :IChatMesageRepository
    {
        private readonly ChatMessageServiceContext _context;

        public ChatMessageServiceRepository(ChatMessageUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<CHATMESSAGE> All           
        {
            get { return _context.ChatMessages; }
        }

        public List<CHATMESSAGE> AllUsers
        {
            get { return _context.ChatMessages.ToList(); }
        }


        public IQueryable<CHATMESSAGE> AllIncluding(params Expression<Func<CHATMESSAGE, object>>[] includeProperties)
        {
            IQueryable<CHATMESSAGE> query = _context.ChatMessages;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public CHATMESSAGE Find(string id)
        {
            return _context.ChatMessages.Find(id);
        }


        public void InsertOrUpdateGraph(CHATMESSAGE userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.ChatMessages.Add(userGraph);
            }
            else
            {
                _context.ChatMessages.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(CHATMESSAGE rate)
        {
            if (rate.MESSAGEID== default(int)) // New entity
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
            var rate = _context.ChatMessages.Find(id);           

            _context.ChatMessages.Remove(rate);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public CHATMESSAGE Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}