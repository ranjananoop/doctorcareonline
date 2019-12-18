using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.PatientInformationServiceBoundedContext;

namespace DOCVIDEO.PatientInformationServiceRepoUOW
{
    public class DoctorRatingUnitOfWork : IUnitOfWork<PatientRatingInformationServiceContext>
    {
        private readonly PatientRatingInformationServiceContext _context;


        public DoctorRatingUnitOfWork()
        {
            _context = new PatientRatingInformationServiceContext();
        }

        public DoctorRatingUnitOfWork(PatientRatingInformationServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
           return _context.SaveChanges();
        }

        public PatientRatingInformationServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
