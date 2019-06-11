using System.Threading.Tasks;

namespace MyBlog.Repository.Data
{
  public interface IUnitOfWork
  {
    IBlogRepository Blogs { get; }
    ITagRepository Tags { get; }
    IBlobRepository Blobs { get; }
    ISubscriberRepository Subscribers { get; }

    Task<bool> SaveAsync();
  }
}
