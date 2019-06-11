using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Extensions;
using MyBlog.Model;
using MyBlog.Peristence.Data;
using MyBlog.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Peristence
{
	public class BlobRepository : IBlobRepository
  {
    private readonly BlogContext context;
    private readonly IMapper mapper;
    public BlobRepository(
      BlogContext context,
      IMapper mapper)
    {
      this.context = context;
      this.mapper = mapper;
    }

    public async Task AddBlobAsync(BlobTable newBlob)
    {
      await this.context.Blobs
        .AddAsync(newBlob);
    }

    public async Task<bool> BlobExistAsync(int blobId)
    {
      return await context.Blobs
        .Where(b => b.Id == blobId)
        .FirstOrDefaultAsync() != null;
    }

    public async Task<Blob> GetBlobAsync(int blobId)
    {
      var blobEntity = await context.Blobs
        .Where(b => b.Id == blobId)
        .FirstOrDefaultAsync();

      return mapper.Map<Blob>(blobEntity);
    }

    public PaggingResult<Blob> GetBlobPageAsync(IPaggingQuery query)
    {
      var blobs = context.Blobs
        .AsQueryable()
        .ProjectTo<Blob>();

      var totalItems = blobs.Count();
      var blobsAfterPagging = blobs.ApplayPaging(query);
      var paggingResult = new PaggingResult<Blob>
      {
        CurrentPage = query.Page,
        PageSize = query.PageSize,
        TotalItems = totalItems,
        TResult = blobsAfterPagging
      };

      return paggingResult;
    }

    public async Task DeleteBlob(int blobId)
    {
      var blobEntity = await context.Blobs
        .Where(b => b.Id == blobId)
        .FirstOrDefaultAsync();

      context.Blobs.Remove(blobEntity);
    }
  }
}
