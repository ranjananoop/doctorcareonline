using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class PrefferedDoctorUnitOfWork : IUnitOfWork<PrefferedDoctorServiceContext>
    {
        private readonly PrefferedDoctorServiceContext _context;

          public PrefferedDoctorUnitOfWork()
        {
            _context = new PrefferedDoctorServiceContext();
        }

          public PrefferedDoctorUnitOfWork(PrefferedDoctorServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public PrefferedDoctorServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
