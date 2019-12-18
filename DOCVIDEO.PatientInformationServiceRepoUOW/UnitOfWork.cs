using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.PatientInformationServiceBoundedContext;

namespace DOCVIDEO.PatientInformationServiceRepoUOW
{
    public class UnitOfWork : IUnitOfWork<PatientInformationServiceContext>
    {
        private readonly PatientInformationServiceContext _context;


        public UnitOfWork()
        {
            _context = new PatientInformationServiceContext();
        }

        public UnitOfWork(PatientInformationServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
           return _context.SaveChanges();
        }

        public PatientInformationServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
