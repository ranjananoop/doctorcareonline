using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class MessageUnitOfWork : IUnitOfWork<MessageServiceContext>
    {
        private readonly MessageServiceContext _context;

          public MessageUnitOfWork()
        {
            _context = new MessageServiceContext();
        }

          public MessageUnitOfWork(MessageServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public MessageServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
