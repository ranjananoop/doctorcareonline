using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceBoundedContext;

namespace DOCVIDEO.UserServiceRepoUOW
{
    public class ChatMessageUnitOfWork : IUnitOfWork<ChatMessageServiceContext>
    {
        private readonly ChatMessageServiceContext _context;


        public ChatMessageUnitOfWork()
        {
            _context = new ChatMessageServiceContext();
        }

        public ChatMessageUnitOfWork(ChatMessageServiceContext context)
        {
            _context = context;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public ChatMessageServiceContext Context
        {
            get { return _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
