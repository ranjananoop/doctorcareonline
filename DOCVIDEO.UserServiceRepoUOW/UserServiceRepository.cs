using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.UserServiceBoundedContext;

namespace UserService.Repositories.Disconnected
{
    public class UserRepository : IUserRepository
    {
        private readonly UserServiceContext _context;

        public UserRepository(UnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<USERSINFORMATION> All
        {
            get { return _context.UserInformations; }
        }

        public List<USERSINFORMATION> AllUsers
        {
            get { return _context.UserInformations.ToList(); }
        }
        

        public IQueryable<USERSINFORMATION> AllIncluding(params Expression<Func<USERSINFORMATION, object>>[] includeProperties)
        {
            IQueryable<USERSINFORMATION> query = _context.UserInformations;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    var knownKeys = new HashSet<TKey>();
        //    return source.Where(element => knownKeys.Add(keySelector(element)));
        //}
        public USERSINFORMATION Find(string id)
        {
            return _context.UserInformations.Find(id);
        }


        public void InsertOrUpdateGraph(USERSINFORMATION userGraph)
        {
            if (userGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.UserInformations.Add(userGraph);
            }
            else
            {
                _context.UserInformations.Add(userGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(USERSINFORMATION user)
        {
            if (user.UserName == default(string)) // New entity
            {
                _context.Entry(user).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(user).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            var user = _context.UserInformations.Find(id);
            //var rolesMemberships = _context.RolesMembseships.Where(r => r.UserId == user.UserId);
            //foreach(var roleMembership in rolesMemberships )
            //{
            //    _context.RolesMembseships.Remove(roleMembership);
            //}
            _context.UserInformations.Remove(user);
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