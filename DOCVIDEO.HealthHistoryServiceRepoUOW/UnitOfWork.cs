using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public class UnitOfWork : IUnitOfWork<HealthHistoryServiceContext>
    {
        private readonly HealthHistoryServiceContext _context;


        public UnitOfWork()
        {
            _context = new HealthHistoryServiceContext();
        }

        public UnitOfWork(HealthHistoryServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public HealthHistoryServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
