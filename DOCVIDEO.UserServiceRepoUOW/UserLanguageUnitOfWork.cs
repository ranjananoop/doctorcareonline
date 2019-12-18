using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class UserLanguageUnitOfWork : IUnitOfWork<UserLanguageServiceContext>
    {
        private readonly UserLanguageServiceContext _context;


        public UserLanguageUnitOfWork()
        {
            _context = new UserLanguageServiceContext();
        }

        public UserLanguageUnitOfWork(UserLanguageServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public UserLanguageServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
