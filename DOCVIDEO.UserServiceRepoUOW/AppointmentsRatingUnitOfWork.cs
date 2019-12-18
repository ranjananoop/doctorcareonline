using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class AppointmentsRatingUnitOfWork : IUnitOfWork<AppointmentsRatingServiceContext>
    {
        private readonly AppointmentsRatingServiceContext _context;


        public AppointmentsRatingUnitOfWork()
        {
            _context = new AppointmentsRatingServiceContext();
        }

        public AppointmentsRatingUnitOfWork(AppointmentsRatingServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public AppointmentsRatingServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
