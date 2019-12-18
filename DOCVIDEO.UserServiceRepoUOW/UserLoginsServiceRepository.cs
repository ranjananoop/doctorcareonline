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
    public class UserLoginsServiceRepository :IUserLoginRepository
    {
        private readonly UserLoginServiceContext _context;

        public UserLoginsServiceRepository(UserLoginsUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<USERLOGIN> All
        {
            get { return _context.UserLogins; }
        }

        public List<USERLOGIN> Allworks
        {
            get { return _context.UserLogins.ToList(); }
        }


        public IQueryable<USERLOGIN> AllIncluding(params Expression<Func<USERLOGIN, object>>[] includeProperties)
        {
            IQueryable<USERLOGIN> query = _context.UserLogins;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public USERLOGIN Find(string id)
        {
            return _context.UserLogins.Find(id);
        }


        public void InsertOrUpdateGraph(USERLOGIN langGraph)
        {
            if (langGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.UserLogins.Add(langGraph);
            }
            else
            {
                _context.UserLogins.Add(langGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(USERLOGIN lang)
        {
            if (lang.USERLOGINID  == default(int)) // New entity
            {
                _context.Entry(lang).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(lang).State = EntityState.Modified;

            }
        }
       

        public void Delete(string id)
        {
            try
            {
                var doctorlanguages = _context.UserLogins.Where(a => a.UserName == id);
                if (doctorlanguages != null)
                {
                    foreach (var lang in doctorlanguages)
                    {
                        _context.UserLogins.Remove(lang);
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


        public USERLOGIN Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}