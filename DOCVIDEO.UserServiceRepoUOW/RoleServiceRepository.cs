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
namespace UserService.Repositories.Disconnected
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserMembershipServiceContext _context;

        public RoleRepository(MembershipUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<Role> All
        {
            get { return _context.Roles; }
        }

        public List<Role> AllRoles
        {
            get { return _context.Roles.ToList(); }
        }
        
        public IQueryable<Role> AllIncluding(params Expression<Func<Role, object>>[] includeProperties)
        {
            IQueryable<Role> query = _context.Roles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Role Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdateGraph(Role roleGraph)
        {
            if (roleGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Roles.Add(roleGraph);
            }
            else
            {
                _context.Roles.Add(roleGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(Role role)
        {
            if (role.RoleName == default(string)) // New entity
            {
                _context.Entry(role).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(role).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            var role = _context.Roles.Find(id);
            _context.Roles.Remove(role);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public Role Find(string userName)
        {
            return _context.Roles.Find(userName);
            
        }


        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}