using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class UnitOfWork : IUnitOfWork<UserServiceContext>
    {
        private readonly UserServiceContext _context;


        public UnitOfWork()
        {
            _context = new UserServiceContext();
        }

        public UnitOfWork(UserServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public UserServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
