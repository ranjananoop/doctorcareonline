using System;
using System.Data.Entity;
namespace DOCVIDEO.BaseDataLayer
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        int Save();
        TContext Context { get; }

    }

}