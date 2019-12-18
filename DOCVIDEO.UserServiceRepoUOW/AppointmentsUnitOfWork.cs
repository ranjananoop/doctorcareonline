using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class AppointmentsUnitOfWork : IUnitOfWork<AppointmentsServiceContext>
    {
        private readonly AppointmentsServiceContext _context;


        public AppointmentsUnitOfWork()
        {
            _context = new AppointmentsServiceContext();
        }

        public AppointmentsUnitOfWork(AppointmentsServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public AppointmentsServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
