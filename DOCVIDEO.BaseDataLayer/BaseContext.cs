using System.Data.Entity;

namespace DOCVIDEO.BaseDataLayer
{
    public class BaseContext<TContext>
      : DbContext where TContext : DbContext
    {
        static BaseContext()
        {
            Database.SetInitializer<TContext>(null);
        }
        protected BaseContext()
            : base("name=PROFESSORSONLINEEntities")
        { }
    }
}
