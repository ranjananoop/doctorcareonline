using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class PayRateUnitOfWork : IUnitOfWork<DoctorPayRateServiceContext>
    {
        private readonly DoctorPayRateServiceContext _context;


        public PayRateUnitOfWork()
        {
            _context = new DoctorPayRateServiceContext();
        }

        public PayRateUnitOfWork(DoctorPayRateServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorPayRateServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
