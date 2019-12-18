using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class SubscriptionPaymentUnitOfWork : IUnitOfWork<SubscriptionPaymentServiceContext>
    {
        private readonly SubscriptionPaymentServiceContext _context;


        public SubscriptionPaymentUnitOfWork()
        {
            _context = new SubscriptionPaymentServiceContext();
        }

        public SubscriptionPaymentUnitOfWork(SubscriptionPaymentServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public SubscriptionPaymentServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
