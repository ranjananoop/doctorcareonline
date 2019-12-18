using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public class ImmunizationUnitOfWork : IUnitOfWork<ImmunizationServiceContext>
    {
        private readonly ImmunizationServiceContext _context;


        public ImmunizationUnitOfWork()
        {
            _context = new ImmunizationServiceContext();
        }

        public ImmunizationUnitOfWork(ImmunizationServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public ImmunizationServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
