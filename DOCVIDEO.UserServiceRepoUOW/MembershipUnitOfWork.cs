using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class MembershipUnitOfWork : IUnitOfWork<UserMembershipServiceContext>
    {
        private readonly UserMembershipServiceContext _context;


        public MembershipUnitOfWork()
        {
            _context = new UserMembershipServiceContext();
        }

        public MembershipUnitOfWork(UserMembershipServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public UserMembershipServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
