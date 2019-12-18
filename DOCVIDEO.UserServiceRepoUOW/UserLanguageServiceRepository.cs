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
    public class UserLanguageServiceRepository :IUserLanguageRepository
    {
        private readonly UserLanguageServiceContext _context;

        public UserLanguageServiceRepository(UserLanguageUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<USERSLANGUAGE> All
        {
            get { return _context.UserLanguages; }
        }

        public List<USERSLANGUAGE> Allworks
        {
            get { return _context.UserLanguages.ToList(); }
        }


        public IQueryable<USERSLANGUAGE> AllIncluding(params Expression<Func<USERSLANGUAGE, object>>[] includeProperties)
        {
            IQueryable<USERSLANGUAGE> query = _context.UserLanguages;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public USERSLANGUAGE Find(string id)
        {
            return _context.UserLanguages.Find(id);
        }


        public void InsertOrUpdateGraph(USERSLANGUAGE langGraph)
        {
            if (langGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.UserLanguages.Add(langGraph);
            }
            else
            {
                _context.UserLanguages.Add(langGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(USERSLANGUAGE lang)
        {
            if (lang.USERLANGUAGEID  == default(int)) // New entity
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
                var doctorlanguages = _context.UserLanguages.Where(a => a.UserName == id);
                if (doctorlanguages != null)
                {
                    foreach (var lang in doctorlanguages)
                    {
                        _context.UserLanguages.Remove(lang);
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


        public USERSLANGUAGE Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}