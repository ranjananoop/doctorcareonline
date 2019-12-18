using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class SpecialityUnitOfWork : IUnitOfWork<DoctorSpecialityServiceContext>
    {
        private readonly DoctorSpecialityServiceContext _context;


        public SpecialityUnitOfWork()
        {
            _context = new DoctorSpecialityServiceContext();
        }

        public SpecialityUnitOfWork(DoctorSpecialityServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorSpecialityServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
