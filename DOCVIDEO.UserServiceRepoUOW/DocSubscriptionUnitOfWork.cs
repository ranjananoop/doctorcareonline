using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class DocSubscriptionUnitOfWork : IUnitOfWork<DocSubscriptionServiceContext>
    {
        private readonly DocSubscriptionServiceContext _context;


        public DocSubscriptionUnitOfWork()
        {
            _context = new DocSubscriptionServiceContext();
        }

        public DocSubscriptionUnitOfWork(DocSubscriptionServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DocSubscriptionServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
