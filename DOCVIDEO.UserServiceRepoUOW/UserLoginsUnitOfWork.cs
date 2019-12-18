using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class UserLoginsUnitOfWork : IUnitOfWork<UserLoginServiceContext>
    {
        private readonly UserLoginServiceContext _context;


        public UserLoginsUnitOfWork()
        {
            _context = new UserLoginServiceContext();
        }

        public UserLoginsUnitOfWork(UserLoginServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public UserLoginServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
