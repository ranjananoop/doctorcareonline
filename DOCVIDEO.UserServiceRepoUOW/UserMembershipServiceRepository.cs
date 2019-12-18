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
    public class UserMembershipRepository : IUserMembershipRepository
    {
        private readonly UserMembershipServiceContext _context;

        public UserMembershipRepository(MembershipUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<USERSINFORMATION> All
        {
            get { return _context.Users; }
        }

        public List<USERSINFORMATION> AllUsers
        {
            get { return _context.Users.ToList(); }
        }
        
        public IQueryable<USERSINFORMATION> AllIncluding(params Expression<Func<USERSINFORMATION, object>>[] includeProperties)
        {
            IQueryable<USERSINFORMATION> query = _context.Users;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public USERSINFORMATION Find(string userName)
        {
            return _context.Users.Find(userName);
        }

        public void InsertOrUpdate(USERSINFORMATION user)
        {
            if (user.UserName == default(string))
            {
                // New entity
                //_context.Users.Add(user);
                _context.Entry(user).State = EntityState.Added;
            }
            else
            {
                // Existing entity
                //_context.Users.Attach(user);
                _context.Entry(user).State = EntityState.Modified;
            }
        }

        public void Delete(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public void AddRole(string id, string roleName)
        {
            var user = _context.Users.Find(id);
            var role = _context.Roles.Find(roleName);
            user.Roles.Add(role);
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
        }

        public void RemoveRole(string id, string roleName)
        {
            var user = _context.Users.Find(id);
            var role = _context.Roles.Find(roleName);
            user.Roles.Remove(role);
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
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