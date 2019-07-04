using AutoMapper;
using MyBlog.Repository;
using MyBlog.Repository.Data;
using System.Threading.Tasks;

namespace MyBlog.Persistence.Data
{
	public class UnitOfWork : IUnitOfWork
  {
    private readonly BlogContext _context;

    public UnitOfWork(
      BlogContext context,
      IMapper mapper)
    {
      this._context = context;

      Blogs = new BlogRepository(context, mapper);
      Tags = new TagRepository(context);
      Blobs = new BlobRepository(context, mapper);
      Subscribers = new SubscriberRepository(context, mapper);
    }

    public IBlogRepository Blogs { get; private set; }

    public ITagRepository Tags { get; private set; }

    public IBlobRepository Blobs { get; private set; }

    public ISubscriberRepository Subscribers { get; private set; }

    public async Task<bool> SaveAsync()
    {
      return await _context.SaveChangesAsync() >= 0;
    }
  }
}
