using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;

namespace DOCVIDEO.HealthHistoryServiceRepoUOW
{
    public class ProcedureUnitOfWork : IUnitOfWork<ProcedureServiceContext>
    {
        private readonly ProcedureServiceContext _context;
         public ProcedureUnitOfWork()
        {
            _context = new ProcedureServiceContext();
        }

         public ProcedureUnitOfWork(ProcedureServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public ProcedureServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
