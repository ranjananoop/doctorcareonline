using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class AppointmentStatusUnitOfWork : IUnitOfWork<DoctorAppointmentStatusServiceContext>
    {
        private readonly DoctorAppointmentStatusServiceContext _context;


        public AppointmentStatusUnitOfWork()
        {
            _context = new DoctorAppointmentStatusServiceContext();
        }

        public AppointmentStatusUnitOfWork(DoctorAppointmentStatusServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public DoctorAppointmentStatusServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
