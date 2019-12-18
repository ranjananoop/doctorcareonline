using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class PayPalIPNUnitOfWork : IUnitOfWork<PayPalIPNServiceContext>
    {
        private readonly PayPalIPNServiceContext _context;


        public PayPalIPNUnitOfWork()
        {
            _context = new PayPalIPNServiceContext();
        }

        public PayPalIPNUnitOfWork(PayPalIPNServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public PayPalIPNServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
