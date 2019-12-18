using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public class AllergyUnitOfWork : IUnitOfWork<AllergiesServiceContext>
    {
        private readonly AllergiesServiceContext _context;


        public AllergyUnitOfWork()
        {
            _context = new AllergiesServiceContext();
        }

        public AllergyUnitOfWork(AllergiesServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public AllergiesServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
