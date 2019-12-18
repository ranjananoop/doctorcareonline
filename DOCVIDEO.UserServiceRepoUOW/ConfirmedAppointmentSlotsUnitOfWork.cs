using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class ConfirmedAppointmentSlotsUnitOfWork : IUnitOfWork<ConfirmedAppointmentsSlotsContext>
    {
        private readonly ConfirmedAppointmentsSlotsContext _context;


        public ConfirmedAppointmentSlotsUnitOfWork()
        {
            _context = new ConfirmedAppointmentsSlotsContext();
        }

        public ConfirmedAppointmentSlotsUnitOfWork(ConfirmedAppointmentsSlotsContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public ConfirmedAppointmentsSlotsContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
