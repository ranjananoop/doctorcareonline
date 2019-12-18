using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class AppointmentDoctorNotesUnitOfWork : IUnitOfWork<DoctorNotesServiceContext>
    {
        private readonly DoctorNotesServiceContext _context;


        public AppointmentDoctorNotesUnitOfWork()
        {
            _context = new DoctorNotesServiceContext();
        }

        public AppointmentDoctorNotesUnitOfWork(DoctorNotesServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorNotesServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
