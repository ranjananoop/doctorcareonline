using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class WorkInstitutionUnitOfWork : IUnitOfWork<DoctorWorkInstitutionServiceContext>
    {
        private readonly DoctorWorkInstitutionServiceContext _context;


        public WorkInstitutionUnitOfWork()
        {
            _context = new DoctorWorkInstitutionServiceContext();
        }

        public WorkInstitutionUnitOfWork(DoctorWorkInstitutionServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorWorkInstitutionServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
