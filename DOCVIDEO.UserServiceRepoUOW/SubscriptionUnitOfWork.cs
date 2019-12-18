using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class SubscriptionUnitOfWork : IUnitOfWork<SubscriptionServiceContext>
    {
        private readonly SubscriptionServiceContext _context;


        public SubscriptionUnitOfWork()
        {
            _context = new SubscriptionServiceContext();
        }

        public SubscriptionUnitOfWork(SubscriptionServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public SubscriptionServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
