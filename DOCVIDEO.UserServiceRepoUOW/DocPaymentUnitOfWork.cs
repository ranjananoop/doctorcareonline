using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class DocPaymentUnitOfWork : IUnitOfWork<DocPaymentServiceContext>
    {
        private readonly DocPaymentServiceContext _context;


        public DocPaymentUnitOfWork()
        {
            _context = new DocPaymentServiceContext();
        }

        public DocPaymentUnitOfWork(DocPaymentServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DocPaymentServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
