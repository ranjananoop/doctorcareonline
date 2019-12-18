using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public class MedicationUnitOfWork : IUnitOfWork<MedicationServiceContext>
    {
        private readonly MedicationServiceContext _context;


        public MedicationUnitOfWork()
        {
            _context = new MedicationServiceContext();
        }

        public MedicationUnitOfWork(MedicationServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public MedicationServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
