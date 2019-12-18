using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class DoctorInformationUnitOfWork : IUnitOfWork<DoctorInformationServiceContext>
    {
        private readonly DoctorInformationServiceContext _context;


        public DoctorInformationUnitOfWork()
        {
            _context = new DoctorInformationServiceContext();
        }

        public DoctorInformationUnitOfWork(DoctorInformationServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorInformationServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
