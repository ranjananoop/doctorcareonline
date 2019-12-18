using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class AppointmentSlotsUnitOfWork : IUnitOfWork<AppointmentsSlotsContext>
    {
        private readonly AppointmentsSlotsContext _context;


        public AppointmentSlotsUnitOfWork()
        {
            _context = new AppointmentsSlotsContext();
        }

        public AppointmentSlotsUnitOfWork(AppointmentsSlotsContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public AppointmentsSlotsContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
